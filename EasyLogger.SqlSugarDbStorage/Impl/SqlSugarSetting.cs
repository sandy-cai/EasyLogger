using EasyLogger.SqlSugarDbStorage.Interface;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLogger.SqlSugarDbStorage.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlSugarSetting : ISqlSugarSetting
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbType DatabaseType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Action<string, SugarParameter[]> LogExecuting { get; set; }
    }
}
