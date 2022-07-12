using EasyLogger.DbStorage;
using EasyLogger.DbStorage.Interface;
using EasyLogger.SqlSugarDbStorage.Interface;
using SqlSugar;
using SqlSugar.DistributedSystem.Snowflake;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EasyLogger.SqlSugarDbStorage.Impl
{
    /// <summary>
    /// SqlSugar的Repo： 1、获取当前运行的Sugar（storage.getName()）  2、切换数据库Provider  3、插入  4、查询
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public class SqlSugarRepository<TEntity, TPrimaryKey> : ISqlSugarRepository<TEntity, TPrimaryKey>
        where TEntity : class, IDbEntity<TPrimaryKey>, new()
    {
        public string ProviderName { get; private set; }

        public string OldProviderName { get; private set; }

        protected readonly ISqlSugarProviderStorage _sqlSugarProviderStorage;

        public SqlSugarRepository(ISqlSugarProviderStorage sqlSugarProviderStorage)
        {
            _sqlSugarProviderStorage = sqlSugarProviderStorage;
        }

        public SqlSugarClient GetCurrentSqlSugar()
        {
            return _sqlSugarProviderStorage.GetByName(this.ProviderName, SqlSugarDbStorageConsts.DefaultProviderName).Sugar;
        }

        /// <summary>
        /// 切换数据库
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDisposable ChangeProvider(string name)
        {
            OldProviderName = ProviderName;
            ProviderName = name;
            return new DisposeAction(() =>
            {
                ProviderName = OldProviderName;
                OldProviderName = null;
            });
        }

        public int Insert(TEntity entity)
        {
            return this.GetCurrentSqlSugar().Insertable<TEntity>(entity).ExecuteCommand();
        }

        public List<TEntity> GetQuery(Expression<Func<TEntity, bool>> expression = null)
        {
            return this.GetCurrentSqlSugar().Queryable<TEntity>().Where(expression).ToList();
        }

        public void Dispose()
        {

        }

    }
}
