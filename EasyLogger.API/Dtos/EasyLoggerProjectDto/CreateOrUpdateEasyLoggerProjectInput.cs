using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyLogger.API.Dtos.EasyLoggerProjectDto
{
    public class CreateOrUpdateEasyLoggerProjectInput
    {
        public EasyLoggerProjectEditDto EasyLoggerProject { get; set; }
    }
}
