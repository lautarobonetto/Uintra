﻿using System;
using System.Web.Mvc;
using uIntra.CentralFeed;
using uIntra.Core.Activity;
using uIntra.Core.User;
using uIntra.Groups.Web;
using uIntra.Subscribe;

namespace Compent.uIntra.Controllers
{
    public class GroupController : GroupFeedControllerBase
    {
        public GroupController(ICentralFeedContentHelper centralFeedContentHelper, ISubscribeService subscribeService, ICentralFeedService centralFeedService, IActivitiesServiceFactory activitiesServiceFactory, IIntranetUserContentHelper intranetUserContentHelper, ICentralFeedTypeProvider centralFeedTypeProvider, IIntranetUserService<IIntranetUser> intranetUserService) : base(centralFeedContentHelper, subscribeService, centralFeedService, activitiesServiceFactory, intranetUserContentHelper, centralFeedTypeProvider, intranetUserService)
        {
        }

        protected override string OverviewViewPath { get; }
        protected override string ListViewPath { get; }
        protected override string NavigationViewPath { get; }
        protected override string LatestActivitiesViewPath { get; }
    }
}