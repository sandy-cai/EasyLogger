using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyLogger.API.Dtos
{
    public class PagedInput
    {
        public Int32 PageSize { get; set; }
        public Int32 PageIndex { get; set; }
    }
}
