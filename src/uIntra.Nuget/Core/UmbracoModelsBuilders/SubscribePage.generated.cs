//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder v3.0.7.99
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.ModelsBuilder;
using Umbraco.ModelsBuilder.Umbraco;

namespace Umbraco.Web.PublishedContentModels
{
	/// <summary>Subscribe Page</summary>
	[PublishedContentModel("subscribePage")]
	public partial class SubscribePage : BasePage, INavigationComposition
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "subscribePage";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public SubscribePage(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<SubscribePage, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Is hide from Left Navigation
		///</summary>
		[ImplementPropertyType("isHideFromLeftNavigation")]
		public bool IsHideFromLeftNavigation
		{
			get { return Umbraco.Web.PublishedContentModels.NavigationComposition.GetIsHideFromLeftNavigation(this); }
		}

		///<summary>
		/// Is hide from Sub Navigation
		///</summary>
		[ImplementPropertyType("isHideFromSubNavigation")]
		public bool IsHideFromSubNavigation
		{
			get { return Umbraco.Web.PublishedContentModels.NavigationComposition.GetIsHideFromSubNavigation(this); }
		}

		///<summary>
		/// Navigation Name
		///</summary>
		[ImplementPropertyType("navigationName")]
		public string NavigationName
		{
			get { return Umbraco.Web.PublishedContentModels.NavigationComposition.GetNavigationName(this); }
		}
	}
}