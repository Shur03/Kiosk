using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCSC;

namespace KioskSkytel.KioskApp.Services.HardwareMock
{

    public class SmartCardService
    {
        public string[] GetReaders()
        {
            using var context =
                ContextFactory.Instance.Establish(SCardScope.System);

            return context.GetReaders();
        }
    }
}
