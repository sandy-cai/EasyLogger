﻿using AutoMapper;
using EasyLogger.API.Dtos.EasyLoggerProjectDto;
using EasyLogger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyLogger.API.AutoMapper
{
    public class ViewModelToEntityMappingProfile : Profile
    {
        public ViewModelToEntityMappingProfile()
        {
            CreateMap<EasyLoggerProjectListDto, EasyLoggerProject>();
            CreateMap<EasyLoggerProjectEditDto, EasyLoggerProject>();

        }
    }
}
