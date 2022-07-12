using EasyLogger.SqlSugarDbStorage.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EasyLogger.SqlSugarDbStorage.Impl
{
    /// <summary>
    /// SqlSugar连接提供程序存储器的具体实现   1\2\3 皆是底层IAnyStorage的规范
    /// 1、构造方法  -------->>> 拿到所有的Sql实例对象，并放到map里面
    /// 2、GetByName() 根据别名，获取SqlSugarClient
    /// 3、Remove()
    /// </summary>
    public class DefaultSqlSugarProviderStorage : ISqlSugarProviderStorage
    {
        public ConcurrentDictionary<string, ISqlSugarProvider> DataMap { get; private set; }

        public DefaultSqlSugarProviderStorage(IServiceProvider serviceProvider) 
        {
            DataMap = new ConcurrentDictionary<string, ISqlSugarProvider>();

            //获取所有SqlSugar的实例对象，并放到并发字典中
            var tmpDataMap = serviceProvider.GetServices<ISqlSugarProvider>()
                .ToDictionary(item => item.ProviderName);
            foreach (var item in tmpDataMap)
            {
                this.AddOrUpdate(item.Key, item.Value);
            }

        }

        public void AddOrUpdate(string name, ISqlSugarProvider val)
        {
            DataMap[name] = val;
        }

        public void Clear()
        {
            DataMap.Clear();
        }

        public ISqlSugarProvider GetByName(string name, string defaultName)
        {
            ISqlSugarProvider result = null;

            if (name == null)
            {
                if (!DataMap.TryGetValue(defaultName, out result))
                {
                    throw new Exception("没有找到 DefaultName Provider");
                }
                return result;
            }
            else if (DataMap.TryGetValue(name, out result)) 
            {
                return result;
            }

            throw new ArgumentException($"没有找到 {name} Provider");
        }

        public void Remove(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) 
            {
                return;
            }

            this.DataMap.TryRemove(name, out ISqlSugarProvider result);
        }
    }
}
