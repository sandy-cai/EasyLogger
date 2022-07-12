using EasyLogger.DbStorage.Interface;
using EasyLogger.SqlSugarDbStorage.Impl;
using EasyLogger.SqlSugarDbStorage.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLogger.SqlSugarDbStorage
{
    public static class SqlSugarDbStorageServiceCollectionExtensions
    {
        // 封装自己注入的部分，包括：provide、storage、sugarrepo、dbrepo
        public static IServiceCollection AddSqlSugarDbStorage(this IServiceCollection services, ISqlSugarSetting defaultDbSetting)
        {
            if (defaultDbSetting == null)
            {
                throw new ArgumentNullException(nameof(defaultDbSetting));
            }

            services.AddSingleton<ISqlSugarProvider>(new SqlSugarProvider(defaultDbSetting));
            services.AddTransient(typeof(ISqlSugarRepository<,>), typeof(SqlSugarRepository<,>));
            services.AddTransient(typeof(IDbRepository<,>), typeof(SqlSugarRepository<,>));
            services.AddSingleton<ISqlSugarProviderStorage, DefaultSqlSugarProviderStorage>();
            services.AddSingleton<IPartitionDbTableFactory, SqlSugarPartitionDbTableFactory>();

            return services;

        }

        /// <summary>
        /// 动态保存数据库连接
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static IServiceProvider AddSqlSugarDatabaseProvider(this IServiceProvider serviceProvider, ISqlSugarSetting dbSetting)
        {
            if (dbSetting == null)
            {
                throw new ArgumentNullException(nameof(dbSetting));
            }

            var fSqlProviderStorage = serviceProvider.GetRequiredService<ISqlSugarProviderStorage>();

            fSqlProviderStorage.AddOrUpdate(dbSetting.Name, new SqlSugarProvider(dbSetting));

            return serviceProvider;
        }
    }
}
