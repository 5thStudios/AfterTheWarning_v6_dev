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
	/// <summary>Prayer Request</summary>
	[PublishedContentModel("prayerRequest")]
	public partial class PrayerRequest : PublishedContentModel, ISEO
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "prayerRequest";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public PrayerRequest(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<PrayerRequest, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Base Calculation Date: Used to calculate the current percentage.  Resets to current date when current count = 0 and a new prayer is offered.
		///</summary>
		[ImplementPropertyType("baseCalculationDate")]
		public DateTime BaseCalculationDate
		{
			get { return this.GetPropertyValue<DateTime>("baseCalculationDate"); }
		}

		///<summary>
		/// Current Percentage: Used to calculate the current percentage of prayers being offered.
		///</summary>
		[ImplementPropertyType("currentPercentage")]
		public int CurrentPercentage
		{
			get { return this.GetPropertyValue<int>("currentPercentage"); }
		}

		///<summary>
		/// Prayer
		///</summary>
		[ImplementPropertyType("prayer")]
		public string Prayer
		{
			get { return this.GetPropertyValue<string>("prayer"); }
		}

		///<summary>
		/// Prayer Answered: This prayer has been answered.
		///</summary>
		[ImplementPropertyType("prayerAnswered")]
		public bool PrayerAnswered
		{
			get { return this.GetPropertyValue<bool>("prayerAnswered"); }
		}

		///<summary>
		/// Prayer is Public: Shows or hides prayer from the public.
		///</summary>
		[ImplementPropertyType("prayerIsPublic")]
		public bool PrayerIsPublic
		{
			get { return this.GetPropertyValue<bool>("prayerIsPublic"); }
		}

		///<summary>
		/// Prayer Request
		///</summary>
		[ImplementPropertyType("prayerRequestLbl")]
		public object PrayerRequestLbl
		{
			get { return this.GetPropertyValue("prayerRequestLbl"); }
		}

		///<summary>
		/// Prayer Request Member: Member who is requesting the prayers.
		///</summary>
		[ImplementPropertyType("prayerRequestMember")]
		public IPublishedContent PrayerRequestMember
		{
			get { return this.GetPropertyValue<IPublishedContent>("prayerRequestMember"); }
		}

		///<summary>
		/// Prayer Title
		///</summary>
		[ImplementPropertyType("prayerTitle")]
		public string PrayerTitle
		{
			get { return this.GetPropertyValue<string>("prayerTitle"); }
		}

		///<summary>
		/// Request Date: Date the prayer was submitted
		///</summary>
		[ImplementPropertyType("requestDate")]
		public DateTime RequestDate
		{
			get { return this.GetPropertyValue<DateTime>("requestDate"); }
		}

		///<summary>
		/// Total Prayers Offered: A count of all the prayers being offered.
		///</summary>
		[ImplementPropertyType("totalPrayersOffered")]
		public int TotalPrayersOffered
		{
			get { return this.GetPropertyValue<int>("totalPrayersOffered"); }
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
