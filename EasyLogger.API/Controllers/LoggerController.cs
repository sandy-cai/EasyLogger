using Autofac.Extras.DynamicProxy;
using AutoMapper;
using EasyLogger.API.AOP;
using EasyLogger.API.Dtos;
using EasyLogger.API.Dtos.EasyLoggerProjectDto;
using EasyLogger.API.Dtos.EasyLoggerRecordDto;
using EasyLogger.API.EasyTools;
using EasyLogger.API.EasyTools.DynamicLink;
using EasyLogger.Model;
using EasyLogger.SqlSugarDbStorage.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyLogger.API.Controllers
{
    /// <summary>
    /// 1.首先执行AOP 并且拿到注入了那些连接
    /// 2.从默认库中获取我们的项目信息存储到内存
    /// 3.我们通过 得到的注入连接，来进行日志的查询
    /// 4.我们关联上每个日志所属的项目 返回结果
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Intercept(typeof(SqlSugarDynamicLinkAop))]
    public class LoggerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISqlSugarRepository<EasyLoggerRecord, int> _sqlRepository;
        private readonly IDynamicLinkBase _linkBase;
        public LoggerController(IMapper mapper, ISqlSugarRepository<EasyLoggerRecord, int> sqlRepository, IDynamicLinkBase linkBase)
        {
            _mapper = mapper;
            _sqlRepository = sqlRepository;
            _linkBase = linkBase;
        }

        [HttpPost("GetEasyLoggerAsync")]
        [DynamicLink]
        public async Task<PagedResultDto<EasyLoggerRecordListDto>> GetEasyLoggerAsync(EasyLoggerRecordInput input)
        {
            // 获取查询的时间范围
            var dateList = _linkBase.DynamicLinkOrm(input).OrderByDescending(s => s).ToList();
            var result = new PagedResultDto<EasyLoggerRecordListDto>();
            // 查询初始数据库数据
            var projectList = _sqlRepository.GetCurrentSqlSugar().Queryable<EasyLoggerProject>().ToList();
            var DbName = IocManager._configuration["EasyLogger:DbName"];
            var entityList = new List<EasyLoggerRecord>();
            // 为跨库查询定义的参数
            int Sumtotal = 0;

            #region 跨库跨表查询
            foreach (var item in dateList)
            {
                var dayList = TimeTools.GetDayDiff(item.AddDays(1 - DateTime.Now.Day).Date, item.AddDays(1 - DateTime.Now.Day).Date.AddMonths(1).AddSeconds(-1));
                using (_sqlRepository.ChangeProvider($"{DbName}-" + item.ToString("yyyy-MM")))
                {
                    var sqlSugarClient = _sqlRepository.GetCurrentSqlSugar();
                    var queryables = new List<ISugarQueryable<EasyLoggerRecord>>();
                    _sqlRepository.GetCurrentSqlSugar().Queryable<EasyLoggerRecord>();
                    foreach (var day in dayList)
                    {
                        queryables.Add(sqlSugarClient.Queryable<EasyLoggerRecord>().AS($"EasyLoggerRecord_{day}"));
                    }
                    var sqlSugarLogger = sqlSugarClient.UnionAll(queryables);
                    var data = sqlSugarLogger
                         .Where(s => s.CreateTime >= input.TimeStart)
                         .Where(s => s.CreateTime <= input.TimeEnd)
                         .WhereIF(!string.IsNullOrWhiteSpace(input.LogTitle), s => s.LogTitle == input.LogTitle)
                         .WhereIF(!string.IsNullOrWhiteSpace(input.LogType), s => s.LogType == input.LogType)
                         .WhereIF(input.ProjectId != null, s => s.ProjectId == input.ProjectId)
                         .WhereIF(input.LogState != null, s => s.LogState == input.LogState)
                         .OrderBy(s => s.CreateTime, OrderByType.Desc)
                         .ToPageList(input.PageIndex, input.PageSize, ref Sumtotal);
                    entityList.AddRange(data);
                }
            }

            #endregion

            result.Total = Sumtotal;
            result.List = _mapper.Map<List<EasyLoggerRecordListDto>>(entityList);
            foreach (var item in result.List)
            {
                var project = projectList.Where(s => s.Id == item.ProjectId).FirstOrDefault();
                item.EasyLoggerProject = _mapper.Map<EasyLoggerProjectEditDto>(project);
            }
            return result;
        }

    }
}
