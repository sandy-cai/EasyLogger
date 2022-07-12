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
        /// ���ã���Ŀ������ʱ����Ҫ ע������ �����õġ�
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
                c.SwaggerDoc(SwaggerName, new OpenApiInfo { Title = "EasyLogger", Description = "��־��¼�̳�", Version = "v1" });
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "EasyLogger.Api.xml");
                c.IncludeXmlComments(xmlPath, true);
            });
            #endregion

            #region SqlSugar
            var defaultDbPath = Path.Combine(PathExtenstions.GetApplicationCurrentPath(), $"{Configuration["EasyLogger:DbName"]}.db");
            //��װ�Զ���ע��ķ���
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

            #region Ĭ�ϴ����������ݿ� �� ��
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


            services.AddControllers();  //���WebApi������һЩapi�Ļ������ܣ����磺·�ɡ��������

            IocManager.SetConfiguration(Configuration);
            IocManager.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// ���ã������м�� 
        /// �м�������Դ���HTTP�������Ӧ������ܵ������Դ���������󣬲��������󴫸���һ���м������
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

            //�����м��
            //app.Use(async (context, next) =>
            //{
            //    var sqlStorage = app.ApplicationServices.GetService<ISqlSugarProviderStorage>();
            //    var sugarClient = sqlStorage.GetByName(null, SqlSugarDbStorageConsts.DefaultProviderName).Sugar;

            //    Console.WriteLine("�鿴sugarClient");
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
