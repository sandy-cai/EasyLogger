using EasyLogger.SqlSugarDbStorage.Interface;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLogger.SqlSugarDbStorage.Impl
{
    /// <summary>
    /// SqlSugar连接提供程序  ： 1 别名 2 SqlSugarClient 
    /// 3、构造方法 ------>>>创建数据库连接
    /// </summary>
    public class SqlSugarProvider : ISqlSugarProvider
    {
        public string ProviderName { get; set; }

        public SqlSugarClient Sugar { get; set; }

        public SqlSugarProvider(ISqlSugarSetting SqlSugarSetting) 
        {
            this.Sugar = this.CreateSqlSugar(SqlSugarSetting);
            this.ProviderName = SqlSugarSetting.Name;
        }

        private SqlSugarClient CreateSqlSugar(ISqlSugarSetting SqlSugarSetting) 
        {
            // todo 临时
            var db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = SqlSugarSetting.ConnectionString,
                    DbType = SqlSugarSetting.DatabaseType, //设置数据库类型
                    IsAutoCloseConnection = true, //自动释放数据库，如果存在事务，在事务结束后释放
                    InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
                });

            db.Aop.OnLogExecuting = SqlSugarSetting.LogExecuting;  //日志拦截----->>>>等同于下面的语句块

            ////每次Sql执行前事件
            //db.Aop.OnLogExecuting = (sql, pars) =>
            //{
            //    //我可以在这里面写逻辑
            //};

            return db;
        }

        public void Dispose()
        {
            this.Sugar.Dispose();
        }
    }
}
