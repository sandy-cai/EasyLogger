using AutoMapper;
using EasyLogger.API.Dtos;
using EasyLogger.API.Dtos.EasyLoggerProjectDto;
using EasyLogger.Model;
using EasyLogger.SqlSugarDbStorage.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyLogger.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ISqlSugarRepository<EasyLoggerProject, int> _repository;
        private readonly IMapper _mapper;

        public ProjectController(ISqlSugarRepository<EasyLoggerProject, int> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取项目列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetEasyLoggerProjectAsync")]
        public PagedResultDto<EasyLoggerProjectListDto> GetEasyLoggerProjectAsync(EasyLoggerProjectInput input)
        {
            var result = new PagedResultDto<EasyLoggerProjectListDto>();
            var total = 0;
            var sqlSugarClient = _repository.GetCurrentSqlSugar();
            var entityList = sqlSugarClient.Queryable<EasyLoggerProject>()
                .OrderBy(p => p.Code)
                .ToPageList(input.PageIndex, input.PageSize, ref total);

            result.List = _mapper.Map<List<EasyLoggerProjectListDto>>(entityList);
            result.Total = total;
            return result;
        }

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task PostAsync(CreateOrUpdateEasyLoggerProjectInput input)
        {
            if (input.EasyLoggerProject.Id.HasValue)
            {
                await Update(input.EasyLoggerProject);
            }
            else
            {
                await Create(input.EasyLoggerProject);
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected virtual async Task Create(EasyLoggerProjectEditDto input)
        {
            var entity = _mapper.Map<EasyLoggerProject>(input);
            _repository.GetCurrentSqlSugar().Insertable(entity).ExecuteCommand();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        protected virtual async Task Update(EasyLoggerProjectEditDto input)
        {
            if (input.Id != null)
            {
                var entity = _mapper.Map<EasyLoggerProject>(input);

                var sqlSugarClient = _repository.GetCurrentSqlSugar().Updateable(entity).ExecuteCommand();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            _repository.GetCurrentSqlSugar().Deleteable<EasyLoggerProject>(new { Id = id }).ExecuteCommand();
        }
    }
}
