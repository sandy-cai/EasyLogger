using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyLogger.API.EasyTools
{
    public class IocManager
    {
        public static IServiceCollection _services { get; private set; }

        public static IServiceProvider _serviceProvider { get; private set; }

        public static IConfiguration _configuration { get; private set; }

        static IocManager() 
        {
            _services = new ServiceCollection();
        }

        public static IServiceProvider Build() 
        {
            _serviceProvider = _services.BuildServiceProvider();
            return _serviceProvider;
        }

        public static void SetConfiguration(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public static void SetServiceProvider(IServiceProvider serviceProvider) 
        {
            if (_serviceProvider == null) 
            {
                return;
            }
            _serviceProvider = serviceProvider;
        }


    }
}
