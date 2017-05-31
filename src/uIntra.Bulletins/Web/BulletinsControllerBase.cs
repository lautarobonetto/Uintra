﻿using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using uIntra.Core;
using uIntra.Core.Activity;
using uIntra.Core.Controls.LightboxGallery;
using uIntra.Core.Extentions;
using uIntra.Core.Media;
using uIntra.Core.User;
using uIntra.Core.User.Permissions.Web;
using Umbraco.Core;
using Umbraco.Web.Mvc;

namespace uIntra.Bulletins.Web
{
    [ActivityController(IntranetActivityTypeEnum.Bulletins)]
    public abstract class BulletinsControllerBase : SurfaceController
    {
        protected virtual string ItemViewPath { get; } = "~/App_Plugins/Bulletins/Item/ItemView.cshtml";
        protected virtual string CreationFormPath { get; } = "~/App_Plugins/Bulletins/Create/CreationForm.cshtml";
        protected virtual string DetailsViewPath { get; } = "~/App_Plugins/Bulletins/Details/DetailsView.cshtml";
        protected virtual string EditViewPath { get; } = "~/App_Plugins/Bulletins/Edit/EditView.cshtml";
        protected virtual int ShortDescriptionLength { get; } = 500;
        protected virtual int DisplayedImagesCount { get; } = 3;

        private readonly IBulletinsService<BulletinBase> _bulletinsService;
        private readonly IMediaHelper _mediaHelper;
        private readonly IIntranetUserService<IIntranetUser> _userService;

        protected BulletinsControllerBase(
            IBulletinsService<BulletinBase> bulletinsService,
            IMediaHelper mediaHelper,
            IIntranetUserService<IIntranetUser> userService)
        {
            _bulletinsService = bulletinsService;
            _mediaHelper = mediaHelper;
            _userService = userService;
        }

        public virtual PartialViewResult CreationForm()
        {
            var currentUser = _userService.GetCurrentUser();
            var result = new BulletinListCreateFormModel
            {
                HeaderInfo = new IntranetActivityItemHeaderViewModel
                {
                    Title = currentUser.DisplayedName,
                    Type = IntranetActivityTypeEnum.Bulletins,
                    Dates = DateTime.UtcNow
                        .ToString(IntranetConstants.Common.DefaultDateFormat)
                        .ToEnumerableOfOne(),
                    Creator = currentUser
                }
            };

            return PartialView(CreationFormPath, result);
        }

        public virtual ActionResult Details(Guid id)
        {
            FillLinks();
            var bulletin = _bulletinsService.Get(id);
            if (bulletin.IsHidden)
            {
                HttpContext.Response.Redirect(ViewData.GetActivityOverviewPageUrl(IntranetActivityTypeEnum.Bulletins));
            }

            var model = GetViewModel(bulletin);

            return PartialView(DetailsViewPath, model);
        }

        [RestrictedAction(IntranetActivityActionEnum.Edit)]
        public virtual ActionResult Edit(Guid id)
        {
            FillLinks();

            var bulletin = _bulletinsService.Get(id);
            if (bulletin.IsHidden)
            {
                HttpContext.Response.Redirect(ViewData.GetActivityOverviewPageUrl(IntranetActivityTypeEnum.Bulletins));
            }

            if (!_bulletinsService.CanEdit(bulletin))
            {
                HttpContext.Response.Redirect(ViewData.GetActivityDetailsPageUrl(IntranetActivityTypeEnum.Bulletins, id));
            }

            var model = GetEditViewModel(bulletin);
            return PartialView(EditViewPath, model);
        }

        [HttpPost]
        [RestrictedAction(IntranetActivityActionEnum.Edit)]
        public virtual ActionResult Edit(BulletinEditModel editModel)
        {
            FillLinks();

            if (!ModelState.IsValid)
            {
                FillCreateEditData(editModel);
                return PartialView(EditViewPath, editModel);
            }

            var activity = UpdateBulletin(editModel);
            OnBulletinEdited(activity, editModel);
            return Redirect(ViewData.GetActivityDetailsPageUrl(IntranetActivityTypeEnum.Bulletins, editModel.Id));
        }

        protected virtual void FillCreateEditData(IContentWithMediaCreateEditModel model)
        {
            var mediaSettings = _bulletinsService.GetMediaSettings();

            ViewData["AllowedMediaExtentions"] = mediaSettings.AllowedMediaExtentions;

            model.MediaRootId = mediaSettings.MediaRootId;
        }

        protected virtual BulletinEditModel GetEditViewModel(BulletinBase bulletin)
        {
            var model = bulletin.Map<BulletinEditModel>();
            FillCreateEditData(model);
            return model;
        }

        protected virtual BulletinViewModel GetViewModel(BulletinBase bulletin)
        {
            var model = bulletin.Map<BulletinViewModel>();
            model.HeaderInfo = bulletin.Map<IntranetActivityDetailsHeaderViewModel>();
            model.HeaderInfo.Dates = bulletin.PublishDate.ToDateTimeFormat().ToEnumerableOfOne();
            model.CanEdit = _bulletinsService.CanEdit(bulletin);
            return model;
        }

        protected virtual BulletinBase UpdateBulletin(BulletinEditModel editModel)
        {
            var activity = _bulletinsService.Get(editModel.Id);
            activity = Mapper.Map(editModel, activity);
            activity.MediaIds = activity.MediaIds.Concat(_mediaHelper.CreateMedia(editModel));

            _bulletinsService.Save(activity);
            return activity;
        }

        protected virtual BulletinItemViewModel GetItemViewModel(BulletinBase bulletin)
        {
            var model = bulletin.Map<BulletinItemViewModel>();

            model.ShortDescription = bulletin.Description.Truncate(ShortDescriptionLength);
            model.MediaIds = bulletin.MediaIds;
            model.HeaderInfo = bulletin.Map<IntranetActivityItemHeaderViewModel>();

            model.LightboxGalleryPreviewInfo = new LightboxGalleryPreviewModel
            {
                MediaIds = bulletin.MediaIds,
                DisplayedImagesCount = DisplayedImagesCount,
                ActivityId = bulletin.Id,
                ActivityType = bulletin.Type
            };
            return model;
        }

        protected virtual void FillLinks()
        {
            var overviewPageUrl = _bulletinsService.GetOverviewPage(CurrentPage).Url;
            var detailsPageUrl = _bulletinsService.GetDetailsPage(CurrentPage).Url;
            var editPageUrl = _bulletinsService.GetEditPage(CurrentPage).Url;

            ViewData.SetActivityOverviewPageUrl(IntranetActivityTypeEnum.Bulletins, overviewPageUrl);
            ViewData.SetActivityDetailsPageUrl(IntranetActivityTypeEnum.Bulletins, detailsPageUrl);
            ViewData.SetActivityEditPageUrl(IntranetActivityTypeEnum.Bulletins, editPageUrl);
        }

        protected virtual void OnBulletinEdited(BulletinBase bulletin, BulletinEditModel model)
        {
        }
    }
}