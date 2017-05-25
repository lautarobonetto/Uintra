﻿using System.Collections.Generic;
using System.Linq;
using uIntra.Core.Configuration;
using uIntra.Navigation.Configuration;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace uIntra.Navigation
{
    public class SubNavigationModelBuilder : NavigationModelBuilderBase<SubNavigationMenuModel>, ISubNavigationModelBuilder
    {
        public SubNavigationModelBuilder(
            UmbracoHelper umbracoHelper,
            IConfigurationProvider<NavigationConfiguration> navigationConfigurationProvider
            ) : base(umbracoHelper, navigationConfigurationProvider)
        {
        }

        public override SubNavigationMenuModel GetMenu()
        {
            if (IsHomePage(CurrentPage) || IsShowInHomeNavigation(CurrentPage))
            {
                return null;
            }

            var model = new SubNavigationMenuModel
            {
                Items = GetContentForSubNavigation(CurrentPage).Select(MapSubNavigationItem),
                Parent = (IsHomePage(CurrentPage.Parent) || IsContentUnavailable(CurrentPage.Parent)) ?
                    null :
                    MapSubNavigationItem(CurrentPage.Parent),
                Title = GetNavigationName(CurrentPage)
            };

            return model;
        }

        protected override bool IsHideFromNavigation(IPublishedContent publishedContent)
        {
            var result = publishedContent.GetPropertyValue<bool?>(NavigationConfiguration.IsHideFromSubNavigation.Alias);
            return result ?? NavigationConfiguration.IsHideFromSubNavigation.DefaultValue;
        }

        private IEnumerable<IPublishedContent> GetContentForSubNavigation(IPublishedContent publishedContent)
        {
            var result = (publishedContent.Children.Any() || IsHomePage(publishedContent.Parent)) ?
                publishedContent.Children :
                 publishedContent.Parent.Children;

            return GetAvailableContent(result);
        }

        private bool IsHomePage(IPublishedContent content)
        {
            return content.DocumentTypeAlias == NavigationConfiguration.HomePageAlias;
        }

        private MenuItemModel MapSubNavigationItem(IPublishedContent publishedContent)
        {
            var result = new MenuItemModel
            {
                Id = publishedContent.Id,
                Name = GetNavigationName(publishedContent),
                Url = publishedContent.Url,
                IsActive = publishedContent.IsAncestorOrSelf(CurrentPage)
            };

            return result;
        }
    }
}
