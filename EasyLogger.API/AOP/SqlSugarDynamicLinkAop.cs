using Castle.DynamicProxy;
using EasyLogger.API.EasyTools;
using EasyLogger.SqlSugarDbStorage;
using EasyLogger.SqlSugarDbStorage.Impl;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyLogger.API.AOP
{
    public class SqlSugarDynamicLinkAop : DynamicLinkAopBase
    {
        private readonly IServiceProvider _serviceProvider;

        public override void Intercept(IInvocation invocation)
        {
            MethodInfo methodd;
            try
            {
                methodd = invocation.MethodInvocationTarget;  //获取拦截目标
            }
            catch (Exception ex)
            {
                methodd = invocation.GetConcreteMethod();
            }

            var dynamicLinkAttr = GetDynamicLinkAttributeOrNull(methodd);
            if (dynamicLinkAttr == null || dynamicLinkAttr.IsDisabled)
            {
                invocation.Proceed();   //方法执行被拦截
            }
            else
            {
                var input = this.GetTimeRange(invocation);
                var dateList = TimeTools.GetMonthByList(input.TimeStart.ToString("yyyy-MM"), input.TimeEnd.ToString("yyyy-MM"));

                foreach (var item in dateList)
                {
                    var DbName = $"{IocManager._configuration["EasyLogger:DbName"]}-{item.ToString("yyyy-MM")}";
                    var dbPathName = Path.Combine(PathExtenstions.GetApplicationCurrentPath(), DbName + ".db");

                    IocManager._serviceProvider.AddSqlSugarDatabaseProvider(new SqlSugarSetting()
                    {
                        Name = DbName,
                        ConnectionString = @$"Data Source={dbPathName}",
                        DatabaseType = DbType.Sqlite,
                        LogExecuting = (sql, pars) =>
                        {
                            Console.WriteLine($"sql:{sql}");
                        }
                    });
                }
                invocation.Proceed();  //执行被拦截方法
            }
        }
    }
}
