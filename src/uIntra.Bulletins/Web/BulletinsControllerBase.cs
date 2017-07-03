﻿using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using uIntra.Core;
using uIntra.Core.Activity;
using uIntra.Core.Controls.LightboxGallery;
using uIntra.Core.Extentions;
using uIntra.Core.Media;
using uIntra.Core.TypeProviders;
using uIntra.Core.User;
using uIntra.Core.User.Permissions.Web;
using Umbraco.Core;
using Umbraco.Web.Mvc;

namespace uIntra.Bulletins.Web
{
    [ActivityController(ActivityTypeId)]
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
        private readonly IIntranetUserContentHelper _intranetUserContentHelper;
        private readonly IActivityTypeProvider _activityTypeProvider;

        private const int ActivityTypeId = (int) IntranetActivityTypeEnum.Bulletins;

        protected BulletinsControllerBase(
            IBulletinsService<BulletinBase> bulletinsService,
            IMediaHelper mediaHelper,
            IIntranetUserService<IIntranetUser> userService,
            IIntranetUserContentHelper intranetUserContentHelper, IActivityTypeProvider activityTypeProvider)
        {
            _bulletinsService = bulletinsService;
            _mediaHelper = mediaHelper;
            _userService = userService;
            _intranetUserContentHelper = intranetUserContentHelper;
            _activityTypeProvider = activityTypeProvider;
        }

        public virtual PartialViewResult CreationForm()
        {
            FillLinks();

            var currentUser = _userService.GetCurrentUser();
            var mediaSettings = _bulletinsService.GetMediaSettings();

            var result = new BulletinListCreateFormModel
            {
                HeaderInfo = new IntranetActivityItemHeaderViewModel
                {
                    Title = currentUser.DisplayedName,
                    Type = _activityTypeProvider.Get(ActivityTypeId),
                    Dates = DateTime.UtcNow
                        .ToString(IntranetConstants.Common.DefaultDateFormat)
                        .ToEnumerableOfOne(),
                    Creator = currentUser
                },
                AllowedMediaExtentions = mediaSettings.AllowedMediaExtentions
            };

            return PartialView(CreationFormPath, result);
        }

        public virtual ActionResult Details(Guid id)
        {
            FillLinks();
            var bulletin = _bulletinsService.Get(id);
            if (bulletin.IsHidden)
            {
                HttpContext.Response.Redirect(ViewData.GetActivityOverviewPageUrl(ActivityTypeId));
            }

            var model = GetViewModel(bulletin);

            return PartialView(DetailsViewPath, model);
        }

        [RestrictedAction(ActivityTypeId, IntranetActivityActionEnum.Edit)]
        public virtual ActionResult Edit(Guid id)
        {
            FillLinks();

            var bulletin = _bulletinsService.Get(id);
            if (bulletin.IsHidden)
            {
                HttpContext.Response.Redirect(ViewData.GetActivityOverviewPageUrl(ActivityTypeId));
            }

            var model = GetEditViewModel(bulletin);
            return PartialView(EditViewPath, model);
        }

        [HttpPost]
        [RestrictedAction(ActivityTypeId, IntranetActivityActionEnum.Edit)]
        public virtual ActionResult Edit(BulletinEditModel editModel)
        {
            FillLinks();

            if (!ModelState.IsValid)
            {
                return RedirectToCurrentUmbracoPage(Request.QueryString);
            }

            var activity = UpdateBulletin(editModel);
            OnBulletinEdited(activity, editModel);
            return Redirect(ViewData.GetActivityDetailsPageUrl(ActivityTypeId, editModel.Id));
        }

        [HttpPost]
        [RestrictedAction(ActivityTypeId, IntranetActivityActionEnum.Delete)]
        public virtual JsonResult Delete(Guid id)
        {
            _bulletinsService.Delete(id);
            OnBulletinDeleted(id);

            FillLinks();
            return Json(new { Url = ViewData.GetActivityOverviewPageUrl(ActivityTypeId) });
        }

        protected virtual void FillMediaEditData(IContentWithMediaCreateEditModel model)
        {
            var mediaSettings = _bulletinsService.GetMediaSettings();

            ViewData["AllowedMediaExtentions"] = mediaSettings.AllowedMediaExtentions;

            model.MediaRootId = mediaSettings.MediaRootId;
        }

        protected virtual BulletinEditModel GetEditViewModel(BulletinBase bulletin)
        {
            var model = bulletin.Map<BulletinEditModel>();
            FillMediaEditData(model);
            return model;
        }

        protected virtual BulletinViewModel GetViewModel(BulletinBase bulletin)
        {
            var model = bulletin.Map<BulletinViewModel>();
            model.HeaderInfo = bulletin.Map<IntranetActivityDetailsHeaderViewModel>();
            model.HeaderInfo.Dates = bulletin.PublishDate.ToDateTimeFormat().ToEnumerableOfOne();

            var creator = _userService.Get(bulletin);
            model.HeaderInfo.Creator = _userService.Get(bulletin);
            model.HeaderInfo.Title = creator.DisplayedName;

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

            var creator = _userService.Get(bulletin);
            model.HeaderInfo.Creator = _userService.Get(bulletin);
            model.HeaderInfo.Title = creator.DisplayedName;

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
            var overviewPageUrl = _bulletinsService.GetOverviewPage().Url;
            var detailsPageUrl = _bulletinsService.GetDetailsPage().Url;
            var editPageUrl = _bulletinsService.GetEditPage().Url;
            var profilePageUrl = _intranetUserContentHelper.GetProfilePage().Url;

            ViewData.SetActivityOverviewPageUrl(ActivityTypeId, overviewPageUrl);
            ViewData.SetActivityDetailsPageUrl(ActivityTypeId, detailsPageUrl);
            ViewData.SetActivityEditPageUrl(ActivityTypeId, editPageUrl);
            ViewData.SetProfilePageUrl(profilePageUrl);
        }

        protected virtual void OnBulletinEdited(BulletinBase bulletin, BulletinEditModel model)
        {
        }

        protected virtual void OnBulletinDeleted(Guid id)
        {
        }
    }
}