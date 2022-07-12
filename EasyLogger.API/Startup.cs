using Autofac;
using Autofac.Extras.DynamicProxy;
using EasyLogger.API.AOP;
using EasyLogger.API.AutoMapper;
using EasyLogger.API.EasyTools;
using EasyLogger.API.EasyTools.DynamicLink;
using EasyLogger.DbStorage.Interface;
using EasyLogger.Model;
using EasyLogger.SqlSugarDbStorage;
using EasyLogger.SqlSugarDbStorage.Impl;
using EasyLogger.SqlSugarDbStorage.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SqlSugarProvider = EasyLogger.SqlSugarDbStorage.Impl.SqlSugarProvider;

namespace EasyLogger.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public string SwaggerName = "v1";

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// 作用：项目启动的时候，需要 注册或加载 服务用的。
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {

            #region AutoMapper
            services.AddAutoMapper(typeof(EntityToViewModelMappingProfile), typeof(ViewModelToEntityMappingProfile));
            #endregion

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(SwaggerName, new OpenApiInfo { Title = "EasyLogger", Description = "日志记录教程", Version = "v1" });
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "EasyLogger.Api.xml");
                c.IncludeXmlComments(xmlPath, true);
            });
            #endregion

            #region SqlSugar
            var defaultDbPath = Path.Combine(PathExtenstions.GetApplicationCurrentPath(), $"{Configuration["EasyLogger:DbName"]}.db");
            //封装自定义注入的方法
            services.AddSqlSugarDbStorage(new SqlSugarSetting()
            {
                Name = SqlSugarDbStorageConsts.DefaultProviderName,
                ConnectionString = @$"Data Source={defaultDbPath}",
                //ConnectionString = "Allow User Variables=True;Data Source=localhost;Initial Catalog=Easylogger;User ID=root;Password=root;charset=utf8mb4",
                DatabaseType = DbType.Sqlite,
                LogExecuting = (sql, pars) =>
                {
                    Console.WriteLine($"sql:{sql}");
                }
            });

            #region 默认创建基础数据库 和 表
            if (!File.Exists(defaultDbPath))
            {
                var partition = services.BuildServiceProvider().GetService<IPartitionDbTableFactory>();
                partition.DbTableCreate(defaultDbPath, true);
            }
            var startUpDbPath = Path.Combine(PathExtenstions.GetApplicationCurrentPath(), $"{Configuration["EasyLogger:DbName"]}-{DateTime.Now.ToString("yyyy-MM")}.db");

            if (!File.Exists(startUpDbPath))
            {
                var partition = services.BuildServiceProvider().GetService<IPartitionDbTableFactory>();
                partition.DbTableCreate(startUpDbPath, false);
            }

            #endregion

            #endregion


            services.AddControllers();  //针对WebApi，加载一些api的基础功能，例如：路由、请求处理等

            IocManager.SetConfiguration(Configuration);
            IocManager.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// 作用：调用中间件 
        /// 中间件：可以处理HTTP请求或响应的软件管道。可以处理传入的请求，并将该请求传给下一个中间件处理。
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            IocManager.SetServiceProvider(app.ApplicationServices);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            #region Swagger
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{SwaggerName}/swagger.json", SwaggerName);
                c.RoutePrefix = string.Empty;
            });
            #endregion

            app.UseRouting();

            app.UseAuthorization();

            //调用中间件
            //app.Use(async (context, next) =>
            //{
            //    var sqlStorage = app.ApplicationServices.GetService<ISqlSugarProviderStorage>();
            //    var sugarClient = sqlStorage.GetByName(null, SqlSugarDbStorageConsts.DefaultProviderName).Sugar;

            //    Console.WriteLine("查看sugarClient");
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<SqlSugarDynamicLink>().As<IDynamicLinkBase>().EnableClassInterceptors();
            builder.RegisterType<SqlSugarDynamicLinkAop>();
        }
    }

}
