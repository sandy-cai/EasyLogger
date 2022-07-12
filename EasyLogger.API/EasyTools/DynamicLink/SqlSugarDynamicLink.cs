using Autofac.Extras.DynamicProxy;
using EasyLogger.API.AOP;
using EasyLogger.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyLogger.API.EasyTools.DynamicLink
{
    [Intercept(typeof(SqlSugarDynamicLinkAop))]
    public class SqlSugarDynamicLink : IDynamicLinkBase
    {
        [DynamicLink]
        public virtual List<DateTime> DynamicLinkOrm(DynamicLinkInput input)
        {
            return TimeTools.GetMonthByList(input.TimeStart.ToString("yyyy-MM"), input.TimeEnd.ToString("yyyy-MM"));
        }
    }
}
