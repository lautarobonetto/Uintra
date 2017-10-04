﻿using System;
using System.Collections.Generic;
using uIntra.CentralFeed;
using uIntra.Core.Activity;
using uIntra.Core.Extentions;
using uIntra.Core.Grid;
using uIntra.Core.TypeProviders;
using uIntra.Core.User;
using uIntra.Groups.Constants;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace uIntra.Groups
{
    public class GroupHelper : IGroupHelper
    {
        private readonly IGroupService _groupService;
        private readonly IGridHelper _gridHelper;
        private readonly IFeedTypeProvider _centralFeedTypeProvider;
        private readonly IGroupFeedLinkService _groupFeedLinkService;
        private readonly IFeedTypeProvider _feedTypeProvider;
        private readonly IGroupContentHelper _contentHelper;

        public GroupHelper(
            IGroupService groupService,
            IGridHelper gridHelper,
            IFeedTypeProvider centralFeedTypeProvider,
            IGroupFeedLinkService groupFeedLinkService,
            IFeedTypeProvider feedTypeProvider,
            IGroupContentHelper contentHelper)
        {
            _groupService = groupService;
            _gridHelper = gridHelper;
            _centralFeedTypeProvider = centralFeedTypeProvider;
            _groupFeedLinkService = groupFeedLinkService;
            _feedTypeProvider = feedTypeProvider;
            _contentHelper = contentHelper;
        }

        public bool IsGroupPage(IPublishedContent currentPage)
        {
            return _contentHelper.GetOverviewPage().IsAncestorOrSelf(currentPage);
        }

        public bool IsGroupRoomPage(IPublishedContent currentPage)
        {
            return _contentHelper.GetGroupRoomPage().IsAncestorOrSelf(currentPage);
        }

        public ActivityFeedTabModel GetMainFeedTab(IPublishedContent currentContent, Guid groupId)
        {
            var groupRoom = _contentHelper.GetGroupRoomPage();
            var type = GetGroupFeedTabType(groupRoom);
            var result = new ActivityFeedTabModel
            {
                Content = groupRoom,
                Type = type,
                IsActive = groupRoom.Id == currentContent.Id,
                Links = _groupFeedLinkService.GetCreateLinks(type, groupId)
            };
            return result;
        }

        // ULTRA TODO : use tuples to return all tabs at once!
        public IEnumerable<ActivityFeedTabModel> GetActivityTabs(IPublishedContent currentContent, IIntranetUser user, Guid groupId)
        {
            yield return GetMainFeedTab(currentContent, groupId);



            foreach (var content in GetContent())
            {
                var tabType = GetGroupFeedTabType(content);
                var activityType = tabType.Id.ToEnum<IntranetActivityTypeEnum>();

                if (activityType == null)
                    continue;

                var tab = new ActivityFeedTabModel
                {
                    Content = content,
                    Type = tabType,
                    IsActive = content.IsAncestorOrSelf(currentContent),
                    Links = _groupFeedLinkService.GetCreateLinks(tabType, groupId)
                };

                yield return tab;
            }
        }

        public IEnumerable<PageTabModel> GetPageTabs(IPublishedContent currentContent, IIntranetUser user, Guid groupId)
        {
            Func<IPublishedContent, bool> skipPage = GetPageSkipResolver(user, groupId);

            foreach (var content in GetContent())
            {
                if (skipPage(content))
                    continue;
                var tabType = GetGroupFeedTabType(content);
                var activityType = tabType.Id.ToEnum<IntranetActivityTypeEnum>();
                if (activityType == null)
                    yield return GetPageTab(currentContent, content, groupId);
            }
        }

        private Func<IPublishedContent, bool> GetPageSkipResolver(IIntranetUser user, Guid groupId)
        {
            var canEdit = _groupService.CanEdit(groupId, user);
            var editGroupPage = _contentHelper.GetEditPage();

            var deactivatedPage = _contentHelper.GetDeactivatedGroupPage();

            Func<IPublishedContent, bool> skipPage = (content) =>
                    (!canEdit && AreSamePages(editGroupPage, content)
                     || AreSamePages(deactivatedPage, content));
            return skipPage;
        }

        private PageTabModel GetPageTab(IPublishedContent currentContent, IPublishedContent content, Guid groupId)
        {
            return new PageTabModel()
            {
                Content = content,
                IsActive = content.IsAncestorOrSelf(currentContent),
                Title = content.Name,
                Link = content.Url.AddGroupId(groupId)
            };
        }

        private static bool AreSamePages(IPublishedContent first, IPublishedContent second)
        {
            return first.Id == second.Id;
        }

        // TODO : this method is called in a loop. EACH time we parse grid. That decrease performance a lot, young man!
        public IIntranetType GetActivityTypeFromPlugin(IPublishedContent content, string gridPluginAlias)
        {
            var value = _gridHelper.GetValue(content, gridPluginAlias);

            if (value == null || value.tabType == null)
            {
                return _feedTypeProvider.Get(default(CentralFeedTypeEnum).ToInt());
            }

            int tabType;
            if (int.TryParse(value.tabType.ToString(), out tabType))
            {
                return _centralFeedTypeProvider.Get(tabType);
            }
            return _feedTypeProvider.Get(default(CentralFeedTypeEnum).ToInt());
        }

        private IEnumerable<IPublishedContent> GetContent()
        {
            return _contentHelper.GetGroupRoomPage().Children;
        }

        public IIntranetType GetGroupFeedTabType(IPublishedContent content)
        {
            return GetActivityTypeFromPlugin(content, GroupConstants.GroupFeedPluginAlias);
        }

        public IIntranetType GetCreateActivityType(IPublishedContent content)
        {
            return GetActivityTypeFromPlugin(content, GroupConstants.GroupActivityCreatePluginAlias);
        }
    }
}