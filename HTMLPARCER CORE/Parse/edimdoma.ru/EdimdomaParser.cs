﻿using AngleSharp.Html.Dom;
using HTMLPARCER_CORE.Parse;
using System.Collections.Generic;
using System.Linq;

namespace HTMLPARCER_CORE
{
    public class EdimdomaParser : IParser<RecipeShort[]>
    {
        public static int count;

        public int GetCount() => count;

        public RecipeShort[] Parse(IHtmlDocument document)
        {
            var list = new List<RecipeShort>();

            var title = document.QuerySelectorAll("a").
                    Where(item => item.ClassName != null &&
                    item.ClassName.Contains("listRecipieTitle")).ToArray();

            var urlPicture = document.QuerySelectorAll("img").
                Where(item => item.ClassList != null && item.ParentElement.TagName == "SPAN" &&
                item.ParentElement.ClassName.Contains("a thumb hashString")).ToArray();

            count = title.Length;

            for (int j = 0; j < title.Length; j++)
                list.Add(new RecipeShort("edimdoma.ru", title[j].TextContent, urlPicture[j].Attributes[0].Value,
                     "https://povar.ru" + title[j].Attributes[0].Value));

            return list.ToArray();
        }
    }
}
