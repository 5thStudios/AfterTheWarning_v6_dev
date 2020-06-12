//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder v3.0.10.102
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
	// Mixin content Type 1176 with alias "navigation"
	/// <summary>Navigation</summary>
	public partial interface INavigation : IPublishedContent
	{
		/// <summary>Hide Children from Navigation</summary>
		bool HideChildrenFromNavigation { get; }

		/// <summary>Logged-In Item Only</summary>
		bool LoggedInItemOnly { get; }

		/// <summary>Logged-Out Item Only</summary>
		bool LoggedOutItemOnly { get; }

		/// <summary>Show in Minor Navigation</summary>
		bool ShowInMinorNavigation { get; }

		/// <summary>Hide from Main Navigation</summary>
		bool UmbracoNaviHide { get; }
	}

	/// <summary>Navigation</summary>
	[PublishedContentModel("navigation")]
	public partial class Navigation : PublishedContentModel, INavigation
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "navigation";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public Navigation(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<Navigation, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Hide Children from Navigation: Displays page in menu, but hides the children.
		///</summary>
		[ImplementPropertyType("hideChildrenFromNavigation")]
		public bool HideChildrenFromNavigation
		{
			get { return GetHideChildrenFromNavigation(this); }
		}

		/// <summary>Static getter for Hide Children from Navigation</summary>
		public static bool GetHideChildrenFromNavigation(INavigation that) { return that.GetPropertyValue<bool>("hideChildrenFromNavigation"); }

		///<summary>
		/// Logged-In Item Only: Item that only is visible in navigation when the user is logged in.
		///</summary>
		[ImplementPropertyType("loggedInItemOnly")]
		public bool LoggedInItemOnly
		{
			get { return GetLoggedInItemOnly(this); }
		}

		/// <summary>Static getter for Logged-In Item Only</summary>
		public static bool GetLoggedInItemOnly(INavigation that) { return that.GetPropertyValue<bool>("loggedInItemOnly"); }

		///<summary>
		/// Logged-Out Item Only: Item that is only visible in navigation when the user is logged out
		///</summary>
		[ImplementPropertyType("loggedOutItemOnly")]
		public bool LoggedOutItemOnly
		{
			get { return GetLoggedOutItemOnly(this); }
		}

		/// <summary>Static getter for Logged-Out Item Only</summary>
		public static bool GetLoggedOutItemOnly(INavigation that) { return that.GetPropertyValue<bool>("loggedOutItemOnly"); }

		///<summary>
		/// Show in Minor Navigation: Show link in minor navigation
		///</summary>
		[ImplementPropertyType("showInMinorNavigation")]
		public bool ShowInMinorNavigation
		{
			get { return GetShowInMinorNavigation(this); }
		}

		/// <summary>Static getter for Show in Minor Navigation</summary>
		public static bool GetShowInMinorNavigation(INavigation that) { return that.GetPropertyValue<bool>("showInMinorNavigation"); }

		///<summary>
		/// Hide from Main Navigation: Prevents page from appearing in the menu.
		///</summary>
		[ImplementPropertyType("umbracoNaviHide")]
		public bool UmbracoNaviHide
		{
			get { return GetUmbracoNaviHide(this); }
		}

		/// <summary>Static getter for Hide from Main Navigation</summary>
		public static bool GetUmbracoNaviHide(INavigation that) { return that.GetPropertyValue<bool>("umbracoNaviHide"); }
	}
}
