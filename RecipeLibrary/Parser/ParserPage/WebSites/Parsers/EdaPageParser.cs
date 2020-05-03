﻿using AngleSharp.Html.Dom;
using ObjectsLibrary.Parser.ParserPage.Core;
using System.Collections.Generic;
using System.Linq;

namespace ObjectsLibrary.Parser.ParserPage.WebSites
{
    class EdaPageParser : IParserPage<RecipeShort[]>
    {
        public RecipeShort[] Parse(IHtmlDocument document, IParserPageSettings settings)
        {
            var recipes = document.QuerySelectorAll("div.tile-list__horizontal-tile.horizontal-tile.js-portions-count-parent.js-bookmark__obj");

            List <RecipeShort> recipeShorts = new List<RecipeShort>(recipes.Length);

            double index = settings.IndexPopularity + settings.IndexStep;

            foreach (var recipe in recipes)
            {
                index -= settings.IndexStep;
                var divInfo = recipe.QuerySelector("div.lazy-load-container");
                string imageUrl = divInfo.Attributes[3].Value;
                string title = divInfo.Attributes[1].Value;
                string url = settings.Url + recipe.QuerySelector("div.horizontal-tile__item-link.js-click-link").Attributes[1].Value;
                recipeShorts.Add(new RecipeShort(title, new ObjectsLibrary.Components.Image(imageUrl), url, index));
            }
            return recipeShorts.ToArray();
        }
    }
}

