using AutoMapper;
using uIntra.Core.Activity;
using uIntra.Core.Extentions;

namespace uIntra.Bulletins
{
    public class BulletinsAutoMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<BulletinBase, BulletinItemViewModel>()
                .ForMember(dst => dst.MediaIds, o => o.Ignore())
                .ForMember(dst => dst.LightboxGalleryPreviewInfo, o => o.Ignore())
                .ForMember(dst => dst.ActivityType, o => o.MapFrom(el => el.Type))
                .ForMember(dst => dst.HeaderInfo, o => o.Ignore());

            Mapper.CreateMap<BulletinBase, BulletinEditModel>()
              .ForMember(dst => dst.MediaRootId, o => o.Ignore())
              .ForMember(dst => dst.NewMedia, o => o.Ignore())
              .ForMember(dst => dst.Media, o => o.MapFrom(el => el.MediaIds.JoinToString(",")));

            Mapper.CreateMap<BulletinCreateModel, BulletinBase>()
                .ForMember(dst => dst.Id, o => o.Ignore())
                .ForMember(dst => dst.MediaIds, o => o.Ignore())
                .ForMember(dst => dst.IsHidden, o => o.Ignore())
                .ForMember(dst => dst.UmbracoCreatorId, o => o.Ignore())
                .ForMember(dst => dst.CreatedDate, o => o.Ignore())
                .ForMember(dst => dst.ModifyDate, o => o.Ignore())
                .ForMember(dst => dst.Type, o => o.Ignore())
                .ForMember(dst => dst.EndPinDate, o => o.Ignore())
                .ForMember(dst => dst.CreatorId, o => o.Ignore())
                .ForMember(dst => dst.PublishDate, o => o.Ignore())
                .ForMember(dst => dst.IsPinned, o => o.Ignore())
                .ForMember(dst => dst.Title, o => o.Ignore())
                .ForMember(dst => dst.IsPinActual, o => o.Ignore());

            Mapper.CreateMap<BulletinEditModel, BulletinBase>()
                .ForMember(dst => dst.Title, o => o.Ignore())
                .ForMember(dst => dst.IsPinned, o => o.Ignore())
                .ForMember(dst => dst.EndPinDate, o => o.Ignore())
                .ForMember(dst => dst.MediaIds, o => o.Ignore())
                .ForMember(dst => dst.IsHidden, o => o.Ignore())
                .ForMember(dst => dst.UmbracoCreatorId, o => o.Ignore())
                .ForMember(dst => dst.CreatedDate, o => o.Ignore())
                .ForMember(dst => dst.ModifyDate, o => o.Ignore())
                .ForMember(dst => dst.PublishDate, o => o.Ignore())
                .ForMember(dst => dst.Type, o => o.Ignore())
                .ForMember(dst => dst.CreatorId, o => o.Ignore())
                .ForMember(dst => dst.IsPinActual, o => o.Ignore())
                .AfterMap((src, dst) =>
                {
                    dst.MediaIds = src.Media.ToIntCollection();
                });

            Mapper.CreateMap<BulletinBase, BulletinViewModel>()
                .ForMember(dst => dst.CanEdit, o => o.Ignore())
                .ForMember(dst => dst.HeaderInfo, o => o.Ignore())
                .ForMember(dst => dst.ActivityType, o => o.MapFrom(el => el.Type))
                .ForMember(dst => dst.Media, o => o.MapFrom(el => el.MediaIds.JoinToString(",")));

            Mapper.CreateMap<BulletinBase, BulletinsBackofficeViewModel>()
                .ForMember(d => d.PublishDate, o => o.MapFrom(s => s.PublishDate.ToIsoUtcString()))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedDate.ToIsoUtcString()))
                .ForMember(d => d.ModifyDate, o => o.MapFrom(s => s.ModifyDate.ToIsoUtcString()))
                .ForMember(d => d.Media, o => o.MapFrom(s => s.MediaIds.JoinToString(",")));

            Mapper.CreateMap<BulletinBase, IntranetActivityDetailsHeaderViewModel>()
                .ForMember(dst => dst.Creator, o => o.Ignore())
                .ForMember(dst => dst.Dates, o => o.MapFrom(el => el.PublishDate.ToDateFormat().ToEnumerableOfOne()));

            Mapper.CreateMap<BulletinBase, IntranetActivityItemHeaderViewModel>()
                .IncludeBase<BulletinBase, IntranetActivityDetailsHeaderViewModel>()
                .ForMember(dst => dst.ActivityId, o => o.MapFrom(el => el.Id));

            Mapper.CreateMap<BulletinsBackofficeCreateModel, BulletinBase>()
                .ForMember(d => d.MediaIds, o => o.Ignore())
                .ForMember(d => d.Type, o => o.Ignore())
                .ForMember(d => d.CreatorId, o => o.Ignore())
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedDate, o => o.Ignore())
                .ForMember(d => d.ModifyDate, o => o.Ignore())
                .ForMember(d => d.IsPinned, o => o.Ignore())
                .ForMember(d => d.EndPinDate, o => o.Ignore())
                .ForMember(d => d.IsHidden, o => o.Ignore())
                .ForMember(dst => dst.IsPinActual, o => o.Ignore())
                .AfterMap((dst, src) =>
                {
                    src.MediaIds = dst.Media.ToIntCollection();
                });

            Mapper.CreateMap<BulletinsBackofficeSaveModel, BulletinBase>()
                .ForMember(d => d.MediaIds, o => o.Ignore())
                .ForMember(d => d.Type, o => o.Ignore())
                .ForMember(d => d.CreatorId, o => o.Ignore())
                .ForMember(d => d.CreatedDate, o => o.Ignore())
                .ForMember(d => d.ModifyDate, o => o.Ignore())
                .ForMember(d => d.IsPinned, o => o.Ignore())
                .ForMember(d => d.EndPinDate, o => o.Ignore())
                .ForMember(d => d.IsHidden, o => o.Ignore())
                .ForMember(dst => dst.IsPinActual, o => o.Ignore())
                .AfterMap((dst, src) =>
                {
                    src.MediaIds = dst.Media.ToIntCollection();
                });
        }
    }
}