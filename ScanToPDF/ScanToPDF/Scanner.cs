using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIA;

namespace ScanToPDF
{
    class Scanner
    {

        private DeviceInfo deviceInfo;

        public Scanner(DeviceInfo deviceInfo)
        {
            this.deviceInfo = deviceInfo;

        }
        public override string ToString()
        {
            return (string) deviceInfo.Properties["Name"].get_Value();
        }

        public DeviceInfo GetDeviceInfo()
        {
            return deviceInfo;
        }
    }
}
