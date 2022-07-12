using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyLogger.API.EasyTools
{
    public class PathExtenstions
    {
        public static string GetApplicationCurrentPath() 
        {
            return AppDomain.CurrentDomain.BaseDirectory + "../";
        }
    }
}
