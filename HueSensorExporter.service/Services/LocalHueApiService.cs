using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueApi;
using HueSensorExporter.service.Options;
using Microsoft.Extensions.Options;

namespace HueSensorExporter.service.Services
{
    public class LocalHueApiService : LocalHueApi
    {
        public LocalHueApiService(IOptions<HueApiOptions> options) : base(options.Value.Ip, options.Value.ApiKey, null)
        {
        }
    }
}
