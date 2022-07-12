using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLogger.SqlSugarDbStorage.Interface
{
    /// <summary>
    /// SqlSugar连接提供程序 : 1 别名  2 SqlSugarClient
    /// </summary>
    public interface ISqlSugarProvider:IDisposable
    {
        /// <summary>
        /// 针对这个连接起的别名
        /// </summary>
        string ProviderName { get; }
        /// <summary>
        /// SqlSugar实例
        /// </summary>
        SqlSugarClient Sugar { get; }


    }
}
