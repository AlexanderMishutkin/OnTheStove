﻿using XamarinApp.Library.Objects.Boxes.Elements;

namespace XamarinApp.Library.Objects.Boxes
{
    /// <summary>
    /// Объект, который представляет собой набор ингредиентов для какого-то конкретного блюда.
    /// </summary>
    public class IngredientBox
    {
        public Ingredient[] Ingredients { get; }
        public string Title { get; }

        public IngredientBox(string title, Ingredient[] ingredients)
        {
            Title = title;
            Ingredients = ingredients;
        }
    }
}