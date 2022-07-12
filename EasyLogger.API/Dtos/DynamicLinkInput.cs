using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyLogger.API.Dtos
{
    public class DynamicLinkInput: PagedInput
    {
        public DateTime TimeStart { get; set; }

        public DateTime TimeEnd { get; set; }
    }
}
