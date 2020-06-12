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
	/// <summary>Illumination Statistics</summary>
	[PublishedContentModel("illuminationStatistics")]
	public partial class IlluminationStatistics : PublishedContentModel, INavigation, ISEO
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "illuminationStatistics";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public IlluminationStatistics(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<IlluminationStatistics, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Hide Children from Navigation: Displays page in menu, but hides the children.
		///</summary>
		[ImplementPropertyType("hideChildrenFromNavigation")]
		public bool HideChildrenFromNavigation
		{
			get { return Umbraco.Web.PublishedContentModels.Navigation.GetHideChildrenFromNavigation(this); }
		}

		///<summary>
		/// Logged-In Item Only: Item that only is visible in navigation when the user is logged in.
		///</summary>
		[ImplementPropertyType("loggedInItemOnly")]
		public bool LoggedInItemOnly
		{
			get { return Umbraco.Web.PublishedContentModels.Navigation.GetLoggedInItemOnly(this); }
		}

		///<summary>
		/// Logged-Out Item Only: Item that is only visible in navigation when the user is logged out
		///</summary>
		[ImplementPropertyType("loggedOutItemOnly")]
		public bool LoggedOutItemOnly
		{
			get { return Umbraco.Web.PublishedContentModels.Navigation.GetLoggedOutItemOnly(this); }
		}

		///<summary>
		/// Show in Minor Navigation: Show link in minor navigation
		///</summary>
		[ImplementPropertyType("showInMinorNavigation")]
		public bool ShowInMinorNavigation
		{
			get { return Umbraco.Web.PublishedContentModels.Navigation.GetShowInMinorNavigation(this); }
		}

		///<summary>
		/// Hide from Main Navigation: Prevents page from appearing in the menu.
		///</summary>
		[ImplementPropertyType("umbracoNaviHide")]
		public bool UmbracoNaviHide
		{
			get { return Umbraco.Web.PublishedContentModels.Navigation.GetUmbracoNaviHide(this); }
		}

		///<summary>
		/// Meta Robots
		///</summary>
		[ImplementPropertyType("metaRobots")]
		public object MetaRobots
		{
			get { return Umbraco.Web.PublishedContentModels.SEO.GetMetaRobots(this); }
		}

		///<summary>
		/// Redirects
		///</summary>
		[ImplementPropertyType("redirects")]
		public object Redirects
		{
			get { return Umbraco.Web.PublishedContentModels.SEO.GetRedirects(this); }
		}

		///<summary>
		/// SEO Checker
		///</summary>
		[ImplementPropertyType("sEOChecker")]
		public SEOChecker.MVC.MetaData SEochecker
		{
			get { return Umbraco.Web.PublishedContentModels.SEO.GetSEochecker(this); }
		}

		///<summary>
		/// XMLSitemap
		///</summary>
		[ImplementPropertyType("xMLSitemap")]
		public object XMlsitemap
		{
			get { return Umbraco.Web.PublishedContentModels.SEO.GetXMlsitemap(this); }
		}
	}
}
