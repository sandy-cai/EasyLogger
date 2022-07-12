using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyLogger.API.Dtos.EasyLoggerRecordDto
{
    public class CreateOrUpdateEasyLoggerRecordInput
    {
        public EasyLoggerRecordEditDto EasyLoggerRecord { get; set; }
    }
}
