﻿using AutoMapper;
using uIntra.Core.Activity;

namespace uIntra.Search
{
    public class SearchableActivityAutoMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<IntranetActivity, SearchableActivity>()
                .ForMember(dst => dst.Url, o => o.Ignore())
                .ForMember(dst => dst.StartDate, o => o.Ignore())
                .ForMember(dst => dst.EndDate, o => o.Ignore())
                .ForMember(dst => dst.PublishedDate, o => o.Ignore())
                .ForMember(dst => dst.Type, o => o.MapFrom(el => el.Type.Id))
                .ForMember(dst => dst.Description, o => o.MapFrom(el => el.Description));

            base.Configure();
        }
    }
}