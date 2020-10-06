using formulate.app.Types;
using formulate.core.Models;
using Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedContentModels;
using ContentModels = Umbraco.Web.PublishedContentModels;


namespace Controllers
{
    public class ScriptureController : SurfaceController
    {
        public static ScriptureContent ObtainScriptureData(ContentModels.Scripture cmModel, string chapter)
        {
            //Instantiate variables
            Models.ScriptureContent PgContent = new ScriptureContent();

            //Obtain current chapter
            PgContent.chapterCount = cmModel.Chapters;
            int currentChapter = int.TryParse(chapter, out currentChapter) ? currentChapter : 1;
            PgContent.currentChapter = currentChapter;
            if ((PgContent.currentChapter < 1) | (PgContent.currentChapter > PgContent.chapterCount)) { PgContent.currentChapter = 1; }

            //Obtain chapter content to display on page.
            IPublishedContent ipChapter = cmModel.Children.Skip(PgContent.currentChapter - 1).FirstOrDefault();
            PgContent.verses = JsonConvert.DeserializeObject(ipChapter.GetPropertyValue<string>(Common.NodeProperties.verses));


            return PgContent;
        }
        public static TOCContent ObtainTocContent(UmbracoHelper Umbraco, ContentModels.TableOfContent cmTOC)
        {
            TOCContent tOCContent = new TOCContent();
            tOCContent.prefaceUrl = Umbraco.TypedContent((int)Common.siteNode.Preface).Url;

            //Loop thru all book and build ToC structure
            foreach (var ipBook in cmTOC.Children<Scripture>().Where(x => x.DocumentTypeAlias == Common.docType.Scripture))
            {
                if (ipBook.Testament != tOCContent.currentTestament)
                {
                    //Compare current testament with previous testament
                    tOCContent.currentTestament = ipBook.Testament;

                    //Add testament set
                    tOCContent.testament = new tocTestament();
                    tOCContent.testament.testament = tOCContent.currentTestament;
                    tOCContent.toc.lstTestaments.Add(tOCContent.testament);
                }

                if (ipBook.BookSet != tOCContent.currentBookset)
                {
                    //Compare current book set with previous book set
                    tOCContent.currentBookset = ipBook.BookSet;

                    //Add book set
                    tOCContent.bookSet = new tocBookSet();
                    tOCContent.bookSet.bookSet = tOCContent.currentBookset;
                    tOCContent.testament.lstBookSets.Add(tOCContent.bookSet);
                }

                //Add book to proper set
                tOCContent.book = new tocBook();
                tOCContent.book.name = ipBook.FullName;
                tOCContent.book.url = ipBook.Url();
                tOCContent.bookSet.lstBooks.Add(tOCContent.book);
            }

            return tOCContent;
        }
    }
}
