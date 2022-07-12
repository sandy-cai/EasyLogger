using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLogger.DbStorage.Interface
{
    /// <summary>
    /// 约束ORM连接数据库
    /// </summary>
    public interface IPartitionDbTableFactory
    {
        void DbTableCreate(string path, bool isBaseDb);
    }
}
