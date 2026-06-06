using System;
using System.Linq;
using System.Text;
using System.Windows;
using PCSC;
using PCSC.Exceptions;
using PCSC.Iso7816;

namespace KioskSkytel.KioskApp.Services.HardwareMock
{
    public class SmartCardDebug
    {
        private StringBuilder _log = new StringBuilder();

        private void Log(string message)
        {
            _log.AppendLine(message);
        }

        public void LogDebugInfo()
        {
            _log.Clear();
            Log("=== SmartCardDebug start ===");

            try
            {
                using var context = ContextFactory.Instance.Establish(SCardScope.System);
                var readers = context.GetReaders();

                if (readers == null || readers.Length == 0)
                {
                    Log("No smart card readers found.");
                    return;
                }

                var omnikeyReaders = readers
                    .Where(r => r.Contains("OMNIKEY", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (omnikeyReaders.Length == 0)
                {
                    Log("No OMNIKEY reader found.");
                    Log($"Total readers on system: {readers.Length}");
                    return;
                }

                Log($"OMNIKEY readers: {omnikeyReaders.Length}");
                foreach (var reader in omnikeyReaders)
                {
                    Log($"Reader: {reader}");
                    DebugReader(context, reader);
                }
            }
            catch (PCSCException ex)
            {
                Log($"PC/SC error: {ex.Message} (0x{ex.SCardError:X})");
            }
            catch (Exception ex)
            {
                Log("Unexpected error: " + ex);
            }
            finally
            {
                Log("=== SmartCardDebug end ===");
                MessageBox.Show(_log.ToString(), "SmartCard Debug", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DebugReader(ISCardContext context, string readerName)
        {
            try
            {
                using var reader = new SCardReader(context);
                var rc = reader.Connect(readerName, SCardShareMode.Shared, SCardProtocol.Any);
                if (rc != SCardError.Success)
                {
                    Log($"  Connect failed: {rc}");
                    return;
                }

                rc = reader.GetAttrib(SCardAttribute.AtrString, out byte[] atr);
                if (rc == SCardError.Success && atr?.Length > 0)
                    Log("  ATR: " + BitConverter.ToString(atr));
                else
                    Log("  ATR: <unavailable>");

                TryLogUid(reader);
            }
            catch (PCSCException ex)
            {
                Log($"  Reader error: {ex.Message} (0x{ex.SCardError:X})");
            }
        }

        private void TryLogUid(SCardReader reader)
        {
            try
            {
                byte[] sendBuffer = { 0xFF, 0xCA, 0x00, 0x00, 0x00 };
                byte[] recvBuffer = new byte[10];

                var rc = reader.Transmit(sendBuffer, ref recvBuffer);
                if (rc == SCardError.Success)
                    Log("  UID: " + BitConverter.ToString(recvBuffer));
                else
                    Log($"  UID read failed: {rc}");
            }
            catch (Exception ex)
            {
                Log("  UID read failed: " + ex.Message);
            }
        }
    }
}