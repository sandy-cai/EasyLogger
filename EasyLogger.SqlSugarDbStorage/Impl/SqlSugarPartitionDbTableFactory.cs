using EasyLogger.DbStorage.Interface;
using EasyLogger.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLogger.SqlSugarDbStorage.Impl
{
    public class SqlSugarPartitionDbTableFactory : IPartitionDbTableFactory
    {
        public void DbTableCreate(string path, bool isBaseDb)
        {   
            var db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = $@"Data Source={path}",
                    DbType = DbType.Sqlite, //设置数据库类型
                    IsAutoCloseConnection = true, //自动释放数据库，如果存在事务，在事务结束后释放
                    InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
                });

            if (isBaseDb)
            {
                db.CodeFirst.BackupTable().InitTables<EasyLoggerProject>();
                db.CodeFirst.InitTables(typeof(EasyLoggerRecord));
            }
            else 
            {
                CreateLoggerTable(db);
            }
        }

        private static void CreateLoggerTable(SqlSugarClient db) 
        {
            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            for (int i = 1; i <= days; i++)
            {
                // 自定义生成表的别名
                db.MappingTables.Add(nameof(EasyLoggerRecord), $"{nameof(EasyLoggerRecord)}_{i}");
                db.CodeFirst.InitTables(typeof(EasyLoggerRecord));
            }
        }
    }
}
