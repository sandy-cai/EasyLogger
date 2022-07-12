using EasyLogger.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyLogger.API.EasyTools.DynamicLink
{
    public interface IDynamicLinkBase
    {
        abstract List<DateTime> DynamicLinkOrm(DynamicLinkInput input);
    }
}
