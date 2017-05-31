//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder v3.0.5.96
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.ModelsBuilder;
using Umbraco.ModelsBuilder.Umbraco;
using Umbraco.Web;

namespace Compent.uIntra.Core.UmbracoModelsBuilders
{
	/// <summary>Notification Page</summary>
	[PublishedContentModel("notificationPage")]
	public partial class NotificationPage : BasePage, IDefaultGridComposition, INavigationComposition
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "notificationPage";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public NotificationPage(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<NotificationPage, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// ItemCountForPopup
		///</summary>
		[ImplementPropertyType("itemCountForPopup")]
		public int ItemCountForPopup
		{
			get { return this.GetPropertyValue<int>("itemCountForPopup"); }
		}

		///<summary>
		/// Grid
		///</summary>
		[ImplementPropertyType("grid")]
		public Newtonsoft.Json.Linq.JToken Grid
		{
			get { return DefaultGridComposition.GetGrid(this); }
		}

		///<summary>
		/// Is hide from Left Navigation
		///</summary>
		[ImplementPropertyType("isHideFromLeftNavigation")]
		public bool IsHideFromLeftNavigation
		{
			get { return NavigationComposition.GetIsHideFromLeftNavigation(this); }
		}

		///<summary>
		/// Is hide from Sub Navigation
		///</summary>
		[ImplementPropertyType("isHideFromSubNavigation")]
		public bool IsHideFromSubNavigation
		{
			get { return NavigationComposition.GetIsHideFromSubNavigation(this); }
		}

		///<summary>
		/// Navigation Name
		///</summary>
		[ImplementPropertyType("navigationName")]
		public string NavigationName
		{
			get { return NavigationComposition.GetNavigationName(this); }
		}
	}
}