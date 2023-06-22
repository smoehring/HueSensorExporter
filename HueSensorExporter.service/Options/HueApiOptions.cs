using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueSensorExporter.service.Options
{
    public class HueApiOptions
    {
        public static string ConfigSection => "HueApi";
        public string Ip { get; init; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string ApiKey { get; init; }
    }
}
