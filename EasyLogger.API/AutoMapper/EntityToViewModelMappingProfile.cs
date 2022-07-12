using AutoMapper;
using EasyLogger.API.Dtos.EasyLoggerProjectDto;
using EasyLogger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyLogger.API.AutoMapper
{
    public class EntityToViewModelMappingProfile : Profile
    {
        public EntityToViewModelMappingProfile()
        {
            CreateMap<EasyLoggerProject, EasyLoggerProjectListDto>();
            CreateMap<EasyLoggerProject, EasyLoggerProjectEditDto>();
        }
    }
}
