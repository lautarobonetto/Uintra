﻿using System;
using System.Linq;
using System.Web.Mvc;
using uCommunity.Core.Activity;
using uCommunity.Core.User;
using uCommunity.Subscribe.Model;
using Umbraco.Web.Mvc;

namespace uCommunity.Subscribe
{
    public class SubscribeController : SurfaceController
    {
        private readonly ISubscribeService _subscribeService;
        private readonly IIntranetUserService _intranetUserService;
        private readonly IActivitiesServiceFactory _activitiesServiceFactory;

        public SubscribeController(
            ISubscribeService subscribeService,
            IIntranetUserService intranetUserService,
            IActivitiesServiceFactory activitiesServiceFactory)
        {
            _subscribeService = subscribeService;
            _intranetUserService = intranetUserService;
            _activitiesServiceFactory = activitiesServiceFactory;
        }

        public PartialViewResult Index(ISubscribable subscribe, Guid activityId)
        {
            var userId = _intranetUserService.GetCurrentUserId();
            var subscriber = subscribe.Subscribers.SingleOrDefault(s => s.UserId == userId);

            var model = new SubscribeViewModel
            {
                Id = subscriber?.Id,
                UserId = userId,
                ActivityId = activityId,
                IsSubscribed = subscriber != null,
                Type = subscribe.Type,
                HasNotification = HasNotification(subscribe.Type),
                IsNotificationDisabled = subscriber?.IsNotificationDisabled ?? false
            };

            return PartialView("~/App_Plugins/Subscribe/View/SubscribeView.cshtml", model);
        }

        [HttpPost]
        public PartialViewResult Subscribe(Guid activityId, IntranetActivityTypeEnum type)
        {
            var userId = _intranetUserService.GetCurrentUserId();
            var service = _activitiesServiceFactory.GetService(type);
            var subscribeService = (ISubscribableService)service;
            var subscribe = subscribeService.Subscribe(userId, activityId);
            var model = new SubscribeViewModel
            {
                Id = subscribe.Id,
                UserId = userId,
                ActivityId = activityId,
                IsSubscribed = true,
                Type = type,
                HasNotification = HasNotification(type),
                IsNotificationDisabled = subscribe.IsNotificationDisabled
            };

            return PartialView("~/App_Plugins/Subscribe/View/SubscribeView.cshtml", model);
        }

        [HttpPost]
        public PartialViewResult Unsubscribe(Guid activityId, IntranetActivityTypeEnum type)
        {
            var userId = _intranetUserService.GetCurrentUserId();
            var service = _activitiesServiceFactory.GetService(type);
            var subscribeService = (ISubscribableService)service;
            subscribeService.UnSubscribe(userId, activityId);
            var model = new SubscribeViewModel
            {
                UserId = userId,
                ActivityId = activityId,
                IsSubscribed = false,
                Type = type,
                HasNotification = HasNotification(type)
            };

            return PartialView("~/App_Plugins/Subscribe/View/SubscribeView.cshtml", model);
        }

        public PartialViewResult Overview(Guid activityId)
        {
            return PartialView("~/App_Plugins/Subscribe/View/SubscribersOverView.cshtml", new SubscribeOverviewModel { ActivityId = activityId });
        }

        public PartialViewResult List(Guid activityId)
        {
            var subscribs = _subscribeService.Get(activityId).ToList();

            var subscribersNames = subscribs.Count > 0
                ? _intranetUserService.GetManyNames(subscribs.Select(s => s.UserId)).Select(u => u.Item2)
                : Enumerable.Empty<string>();

            return PartialView("~/App_Plugins/Subscribe/View/SubscribersList.cshtml", subscribersNames);
        }

        [HttpPost]
        public void ChangeNotificationDisabled(SubscribeNotificationDisableUpdateModel model)
        {
            var service = _activitiesServiceFactory.GetService(model.Type);
            var subscribeService = (ISubscribableService)service;
            subscribeService.UpdateNotification(model.Id, model.NewValue);
        }

        public JsonResult Version(Guid activityId)
        {
            var version = _subscribeService.GetVersion(activityId);
            return Json(new { Result = version }, JsonRequestBehavior.AllowGet);
        }

        private bool HasNotification(IntranetActivityTypeEnum type)
        {
            return type == IntranetActivityTypeEnum.Events;
        }
    }
}