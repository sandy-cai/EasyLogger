using EasyLogger.DbStorage.Interface;
using System;

namespace EasyLogger.Model
{
    public class EasyLoggerProject : IDbEntity<int>
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 系统编码
        /// </summary>
        public string Code { get; set; }
    }
}
