﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Compent.uIntra.Core.News.Entities;
using Compent.uIntra.Core.News.Models;
using uIntra.CentralFeed;
using uIntra.Core.Activity;
using uIntra.Core.Extentions;
using uIntra.Core.Media;
using uIntra.Core.User;
using uIntra.Core.User.Permissions.Web;
using uIntra.News;
using uIntra.News.Web;
using uIntra.Tagging;
using uIntra.Users;

namespace Compent.uIntra.Controllers
{
    public class NewsController : NewsControllerBase
    {
        protected override string DetailsViewPath => "~/Views/News/DetailsView.cshtml";
        protected override string ItemViewPath => "~/Views/News/ItemView.cshtml";
        protected override string CreateViewPath => "~/Views/News/CreateView.cshtml";
        protected override string EditViewPath => "~/Views/News/EditView.cshtml";
        protected override int ShortDescriptionLength { get; } = 500;

        private readonly ITagsService _tagsService;

        public NewsController(
            IIntranetUserService<IntranetUser> intranetUserService,
            INewsService<News> newsService,
            IMediaHelper mediaHelper,
            ITagsService tagsService)
            : base(intranetUserService, newsService, mediaHelper)
        {
            _tagsService = tagsService;
        }

        public ActionResult CentralFeedItem(ICentralFeedItem item)
        {
            FillLinks();

            var activity = item as News;
            var extendedModel = GetItemViewModel(activity).Map<NewsExtendedItemViewModel>();
            extendedModel.LikesInfo = activity;
            return PartialView(ItemViewPath, extendedModel);
        }

        [HttpPost]
        [RestrictedAction(IntranetActivityActionEnum.Create)]
        public ActionResult Create(NewsExtendedCreateModel createModel)
        {
            return base.Create(createModel);
        }

        [HttpPost]
        [RestrictedAction(IntranetActivityActionEnum.Edit)]
        public ActionResult Edit(NewsExtendedEditModel editModel)
        {
            return base.Edit(editModel);
        }

        protected override NewsCreateModel GetCreateModel()
        {
            var extendedModel = base.GetCreateModel().Map<NewsExtendedCreateModel>();
            return extendedModel;
        }

        protected override NewsEditModel GetEditViewModel(NewsBase news)
        {
            var extendedModel = base.GetEditViewModel(news).Map<NewsExtendedEditModel>();
            extendedModel.Tags = _tagsService.GetMany(news.Id).Map<List<TagEditModel>>();
            return extendedModel;
        }

        protected override NewsViewModel GetViewModel(NewsBase news)
        {
            var extendedNews = (News)news;
            var extendedModel = base.GetViewModel(news).Map<NewsExtendedViewModel>();
            extendedModel = Mapper.Map(extendedNews, extendedModel);
            return extendedModel;
        }

        protected override void OnNewsCreated(Guid activityId, NewsCreateModel model)
        {
            var extendedModel = (NewsExtendedCreateModel)model;
            _tagsService.Save(activityId, extendedModel.Tags.Map<IEnumerable<TagDTO>>());
        }

        protected override void OnNewsEdited(NewsBase news, NewsEditModel model)
        {
            var extendedModel = (NewsExtendedEditModel)model;
            _tagsService.Save(news.Id, extendedModel.Tags.Map<IEnumerable<TagDTO>>());
        }

        #region Restricted actions

        [NonAction]
        [HttpPost]
        [RestrictedAction(IntranetActivityActionEnum.Create)]
        public override ActionResult Create(NewsCreateModel createModel)
        {
            return base.Create(createModel);
        }

        [NonAction]
        [HttpPost]
        [RestrictedAction(IntranetActivityActionEnum.Edit)]
        public override ActionResult Edit(NewsEditModel editModel)
        {
            return base.Edit(editModel);
        }

        #endregion
    }
}