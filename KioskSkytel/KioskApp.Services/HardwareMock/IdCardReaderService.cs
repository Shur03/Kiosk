using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using KioskApp.Models;
using PCSC;
using PCSC.Exceptions;

namespace KioskSkytel.KioskApp.Services.HardwareMock
{
    public class IdCardReaderService
    {
        private static readonly byte[][] KnownApplicationIds =
        {
            new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x77, 0x03, 0x0C, 0x00 },
            new byte[] { 0xA0, 0x00, 0x00, 0x02, 0x47, 0x10, 0x01 },
            new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00, 0x01 },
            new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x63, 0x50, 0x4B, 0x43, 0x53, 0x2D, 0x31, 0x35 },
            new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x45, 0x44, 0x44, 0x00, 0x00 },
            new byte[] { 0xD2, 0x76, 0x00, 0x00, 0x25, 0x45, 0x50, 0x01, 0x00 },
        };

        private readonly StringBuilder _log = new();

        /// <summary>
        /// Checks if a card is present in the reader (by attempting to read ATR).
        /// Returns true if a card is detected, false otherwise.
        /// </summary>
        public bool IsCardPresent()
        {
            try
            {
                using var context = ContextFactory.Instance.Establish(SCardScope.System);
                var readerName = context.GetReaders()
                    ?.FirstOrDefault(r => r.Contains("OMNIKEY", StringComparison.OrdinalIgnoreCase));

                if (readerName == null)
                    return false;

                using var reader = new SCardReader(context);
                var rc = reader.Connect(readerName, SCardShareMode.Shared, SCardProtocol.Any);
                if (rc != SCardError.Success)
                    return false;

                var atr = ReadAtr(reader);
                return !string.IsNullOrEmpty(atr);
            }
            catch
            {
                return false;
            }
        }

        public IdCardInfo ReadCard(string? pin = null)
        {
            _log.Clear();
            var info = new IdCardInfo();

            try
            {
                using var context = ContextFactory.Instance.Establish(SCardScope.System);
                var readerName = context.GetReaders()
                    ?.FirstOrDefault(r => r.Contains("OMNIKEY", StringComparison.OrdinalIgnoreCase));

                if (readerName == null)
                {
                    info.ErrorMessage = "OMNIKEY reader not found.";
                    info.Log = _log.ToString();
                    return info;
                }

                Log($"Reader: {readerName}");

                using var reader = new SCardReader(context);
                var rc = reader.Connect(readerName, SCardShareMode.Shared, SCardProtocol.Any);
                if (rc != SCardError.Success)
                {
                    info.ErrorMessage = $"Connect failed: {rc}";
                    info.Log = _log.ToString();
                    return info;
                }

                info.Atr = ReadAtr(reader);
                Log($"ATR: {info.Atr}");

                var discoveredApps = new List<(byte[] Aid, string Label)>();
                foreach (var aid in KnownApplicationIds)
                {
                    var label = TrySelectApplication(reader, aid);
                    if (label == null)
                        continue;

                    discoveredApps.Add((aid, label));
                    info.Applications.Add(label);
                    Log($"Found application: {label}");
                }

                if (discoveredApps.Count == 0)
                {
                    info.ErrorMessage = "No known ID application found on card.";
                    info.Log = _log.ToString();
                    return info;
                }

                var selected = discoveredApps[0];
                info.SelectedApplication = selected.Label;
                Log($"Using application: {selected.Label}");
                TrySelectApplication(reader, selected.Aid);

                if (!string.IsNullOrEmpty(pin))
                {
                    if (!VerifyPin(reader, pin))
                    {
                        info.ErrorMessage = "PIN verification failed.";
                        info.Log = _log.ToString();
                        return info;
                    }

                    Log("PIN verified.");
                }
                else
                {
                    Log("No PIN provided — reading only public data.");
                }

                var data = TryReadPersonalData(reader);
                if (data.Length > 0)
                {
                    info.RawData = BitConverter.ToString(data);
                    ParsePersonalData(info, data);
                }

                if (string.IsNullOrEmpty(info.RegisterNumber)
                    && string.IsNullOrEmpty(info.LastName)
                    && string.IsNullOrEmpty(info.FirstName))
                {
                    info.ErrorMessage = string.IsNullOrEmpty(pin)
                        ? "Personal data requires ID card PIN."
                        : "Could not parse personal data from card.";
                }
                else
                {
                    info.Success = true;
                }
            }
            catch (PCSCException ex)
            {
                info.ErrorMessage = $"PC/SC error: {ex.Message} (0x{ex.SCardError:X})";
                Log(info.ErrorMessage);
            }
            catch (Exception ex)
            {
                info.ErrorMessage = ex.Message;
                Log(info.ErrorMessage);
            }

            info.Log = _log.ToString();
            return info;
        }

        private static string? ReadAtr(SCardReader reader)
        {
            var rc = reader.GetAttrib(SCardAttribute.AtrString, out byte[] atr);
            return rc == SCardError.Success && atr?.Length > 0
                ? BitConverter.ToString(atr)
                : null;
        }

        private string? TrySelectApplication(SCardReader reader, byte[] aid)
        {
            var command = new byte[5 + aid.Length];
            command[0] = 0x00;
            command[1] = 0xA4;
            command[2] = 0x04;
            command[3] = 0x00;
            command[4] = (byte)aid.Length;
            Array.Copy(aid, 0, command, 5, aid.Length);

            var response = Transmit(reader, command);
            if (response.Sw1 != 0x90)
                return null;

            var aidHex = BitConverter.ToString(aid);
            var label = ParseApplicationLabel(response.Data) ?? aidHex;
            return $"{label} ({aidHex})";
        }

        private bool VerifyPin(SCardReader reader, string pin)
        {
            var pinBytes = Encoding.ASCII.GetBytes(pin);
            var command = new byte[5 + pinBytes.Length];
            command[0] = 0x00;
            command[1] = 0x20;
            command[2] = 0x00;
            command[3] = 0x00;
            command[4] = (byte)pinBytes.Length;
            Array.Copy(pinBytes, 0, command, 5, pinBytes.Length);

            var response = Transmit(reader, command);
            Log($"VERIFY PIN -> SW={response.Sw1:X2}{response.Sw2:X2}");
            return response.Sw1 == 0x90 && response.Sw2 == 0x00;
        }

        private byte[] TryReadPersonalData(SCardReader reader)
        {
            foreach (var (p1, p2, le) in PersonalDataCandidates())
            {
                var command = new byte[] { 0x00, 0xB0, p1, p2, (byte)le };
                var response = Transmit(reader, command);
                Log($"READ BINARY {p1:X2}{p2:X2} -> SW={response.Sw1:X2}{response.Sw2:X2}");

                if (response.Sw1 == 0x90 && response.Data.Length > 0)
                    return response.Data;

                if (response.Sw1 == 0x69 && response.Sw2 == 0x82)
                {
                    Log("Security status not satisfied (PIN required).");
                    break;
                }
            }

            return TryReadRecords(reader);
        }

        private byte[] TryReadRecords(SCardReader reader)
        {
            for (byte record = 1; record <= 5; record++)
            {
                var command = new byte[] { 0x00, 0xB2, record, 0x04, 0x00 };
                var response = Transmit(reader, command);
                Log($"READ RECORD {record} -> SW={response.Sw1:X2}{response.Sw2:X2}");

                if (response.Sw1 == 0x90 && response.Data.Length > 0)
                    return response.Data;
            }

            return Array.Empty<byte>();
        }

        private static IEnumerable<(byte P1, byte P2, int Le)> PersonalDataCandidates()
        {
            yield return (0x00, 0x00, 0xFF);
            yield return (0x00, 0x00, 0x80);
            yield return (0x00, 0x00, 0x40);
        }

        private static ApduResponse Transmit(SCardReader reader, byte[] command)
        {
            var receive = new byte[512];
            var rc = reader.Transmit(command, ref receive);
            if (rc != SCardError.Success)
                throw new InvalidOperationException($"Transmit failed: {rc}");

            var response = ParseResponse(receive);

            if (response.Sw1 == 0x6C)
            {
                var retry = command.ToArray();
                retry[^1] = response.Sw2;
                receive = new byte[512];
                rc = reader.Transmit(retry, ref receive);
                if (rc != SCardError.Success)
                    throw new InvalidOperationException($"Transmit failed: {rc}");
                response = ParseResponse(receive);
            }

            while (response.Sw1 == 0x61)
            {
                var getResponse = new byte[] { 0x00, 0xC0, 0x00, 0x00, response.Sw2 };
                receive = new byte[512];
                rc = reader.Transmit(getResponse, ref receive);
                if (rc != SCardError.Success)
                    throw new InvalidOperationException($"GET RESPONSE failed: {rc}");

                var more = ParseResponse(receive);
                response = new ApduResponse
                {
                    Data = response.Data.Concat(more.Data).ToArray(),
                    Sw1 = more.Sw1,
                    Sw2 = more.Sw2,
                };
            }

            return response;
        }

        private static ApduResponse ParseResponse(byte[] buffer)
        {
            var length = buffer.Length;
            while (length > 2 && buffer[length - 1] == 0x00)
                length--;

            if (length < 2)
                return new ApduResponse();

            return new ApduResponse
            {
                Data = buffer.Take(length - 2).ToArray(),
                Sw1 = buffer[length - 2],
                Sw2 = buffer[length - 1],
            };
        }

        private static void ParsePersonalData(IdCardInfo info, byte[] data)
        {
            var ascii = ExtractPrintableAscii(data);
            if (string.IsNullOrWhiteSpace(ascii))
                return;

            var registerMatch = Regex.Match(ascii, @"[A-Za-zА-Яа-яӨөҮү]{2}\d{8}");
            if (registerMatch.Success)
                info.RegisterNumber = registerMatch.Value;

            var parts = ascii.Split(new[] { '|', ';', ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                info.LastName ??= parts[0].Trim();
                info.FirstName ??= parts[1].Trim();
            }
        }

        private static string? ParseApplicationLabel(byte[]? fci)
        {
            if (fci == null || fci.Length == 0)
                return null;

            foreach (var (_, value) in ParseTlv(fci))
            {
                if (value.Length == 0)
                    continue;

                if (value.All(b => b >= 0x20 && b <= 0x7E))
                    return Encoding.ASCII.GetString(value);
            }

            return null;
        }

        private static IEnumerable<(byte Tag, byte[] Value)> ParseTlv(byte[] data)
        {
            var index = 0;
            while (index < data.Length)
            {
                if (index + 1 >= data.Length)
                    yield break;

                var tag = data[index++];
                var length = (int)data[index++];

                if (length == 0x81 && index < data.Length)
                    length = data[index++];
                else if (length == 0x82 && index + 1 < data.Length)
                    length = (data[index++] << 8) | data[index++];

                if (index + length > data.Length)
                    yield break;

                var value = data.Skip(index).Take(length).ToArray();
                index += length;
                yield return (tag, value);

                if ((tag & 0x20) != 0)
                {
                    foreach (var nested in ParseTlv(value))
                        yield return nested;
                }
            }
        }

        private static string ExtractPrintableAscii(byte[] data)
        {
            var sb = new StringBuilder();
            foreach (var b in data)
            {
                if (b >= 0x20 && b <= 0x7E)
                    sb.Append((char)b);
                else if (sb.Length > 0 && sb[^1] != ' ')
                    sb.Append(' ');
            }

            return sb.ToString().Trim();
        }

        private void Log(string message) => _log.AppendLine(message);

        private sealed class ApduResponse
        {
            public byte[] Data { get; init; } = Array.Empty<byte>();
            public byte Sw1 { get; init; }
            public byte Sw2 { get; init; }
        }
    }
}
