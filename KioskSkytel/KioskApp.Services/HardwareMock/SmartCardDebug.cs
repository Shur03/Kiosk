using System;
using PCSC;
using PCSC.Exceptions;
using PCSC.Iso7816;

namespace KioskSkytel.KioskApp.Services.HardwareMock
{
    public class SmartCardDebug
    {
        public void LogDebugInfo()
        {
            Console.WriteLine("=== SmartCardDebug start ===");

            try
            {
                using var context = ContextFactory.Instance.Establish(SCardScope.System);
                var readers = context.GetReaders();

                if (readers == null || readers.Length == 0)
                {
                    Console.WriteLine("No smart card readers found.");
                    return;
                }

                Console.WriteLine($"Readers found: {readers.Length}");
                foreach (var reader in readers)
                {
                    Console.WriteLine($"Reader: {reader}");
                    DebugReader(context, reader);
                }
            }
            catch (PCSCException ex)
            {
                Console.WriteLine($"PC/SC error: {ex.Message} (0x{ex.SCardError:X})");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error: " + ex);
            }
            finally
            {
                Console.WriteLine("=== SmartCardDebug end ===");
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
                    Console.WriteLine($"  Connect failed: {rc}");
                    return;
                }

                rc = reader.GetAttrib(SCardAttribute.AtrString, out byte[] atr);
                if (rc == SCardError.Success && atr?.Length > 0)
                    Console.WriteLine("  ATR: " + BitConverter.ToString(atr));
                else
                    Console.WriteLine("  ATR: <unavailable>");

                TryLogUid(reader);
            }
            catch (PCSCException ex)
            {
                Console.WriteLine($"  Reader error: {ex.Message} (0x{ex.SCardError:X})");
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
                    Console.WriteLine("  UID: " + BitConverter.ToString(recvBuffer));
                else
                    Console.WriteLine($"  UID read failed: {rc}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("  UID read failed: " + ex.Message);
            }
        }
        private void TryLogUid(IsoReader isoReader)
        {
            try
            {
                var getUid = new CommandApdu(IsoCase.Case2Short, isoReader.ActiveProtocol)
                {
                    CLA = 0xFF,
                    INS = 0xCA,
                    P1 = 0x00,
                    P2 = 0x00,
                    Le = 0x00
                };

                var response = isoReader.Transmit(getUid);
                Console.WriteLine($"  UID SW1SW2: {response.SW1:X2}{response.SW2:X2}");
                Console.WriteLine("  UID Data: " + BitConverter.ToString(response.GetData()));
            }
            catch (Exception ex)
            {
                Console.WriteLine("  UID read failed: " + ex.Message);
            }
        }
    }
}
