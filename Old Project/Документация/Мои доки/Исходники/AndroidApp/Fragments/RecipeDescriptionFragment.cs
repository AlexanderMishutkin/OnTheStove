﻿using System;
using Android.OS;
using XamarinApp;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using ObjectsLibrary;
using Square.Picasso;
using AndroidLibrary;

namespace AndroidApp.Fragments
{
    public class RecipeDescriptionFragment : Android.Support.V4.App.Fragment
    {
        private RecipeFull _recipeFull;
        private ImageView _image;
        private TextView _description;
        private TextView _cpfcRecipe;
        private TextView _authorNameRecipe;
        private TextView _additionalInfoRecipe;
        private TextView _urlRecipe;
        private WebView _videoView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var arguments = Arguments;

            if (arguments != null)
            {
                _recipeFull = DataContext.ByteArrayToObject<RecipeFull>(arguments.GetByteArray("recipeFull"));
            }
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_recipe_description, container, false);

            _image = view.FindViewById<ImageView>(Resource.Id.imageMainRecipe);
            _description = view.FindViewById<TextView>(Resource.Id.titleMainDescription);
            _cpfcRecipe = view.FindViewById<TextView>(Resource.Id.CPFCRecipe);
            _authorNameRecipe = view.FindViewById<TextView>(Resource.Id.authorNameRecipe);
            _additionalInfoRecipe = view.FindViewById<TextView>(Resource.Id.additionalInfoRecipe);
            _urlRecipe = view.FindViewById<TextView>(Resource.Id.urlRecipe);
            _videoView = view.FindViewById<WebView>(Resource.Id.videoRecipe);

            if (string.IsNullOrEmpty(_recipeFull.Additional?.VideoUrl))
            {
                // Если видео нет:
                _videoView.LayoutParameters.Height = 0;
                _videoView.RefreshDrawableState();
                _videoView.RequestLayout();
            }

            return view;
        }
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            if (!string.IsNullOrEmpty(_recipeFull.TitleImage?.ImageUrl))
                Picasso.With(View.Context)
                    .Load(_recipeFull.TitleImage.ImageUrl)
                    .Into(_image);

            _description.Text += _recipeFull.Description;

            if (_recipeFull.Additional?.CPFC != null)
            {
                if (_recipeFull.Additional.CPFC.Calories != 0)
                    _cpfcRecipe.Text +=
                        $"Калории: {_recipeFull.Additional.CPFC.Calories} Ккал.{System.Environment.NewLine}";
                if (_recipeFull.Additional.CPFC.Proteins != 0)
                    _cpfcRecipe.Text += $"Белки: {_recipeFull.Additional.CPFC.Proteins} г.{System.Environment.NewLine}";
                if (_recipeFull.Additional.CPFC.Fats != 0)
                    _cpfcRecipe.Text += $"Жиры: {_recipeFull.Additional.CPFC.Fats} г.{System.Environment.NewLine}";
                if (_recipeFull.Additional.CPFC.Carbohydrates != 0)
                    _cpfcRecipe.Text +=
                        $"Углеводы: {_recipeFull.Additional.CPFC.Carbohydrates} г.{System.Environment.NewLine}";
            }

            if (_recipeFull.Additional != null && !string.IsNullOrEmpty(_recipeFull.Additional.AuthorName))
                _authorNameRecipe.Text = $"Рецепт от: {_recipeFull.Additional.AuthorName}";

            if (_recipeFull.Additional?.CountPortions != 0)
                _additionalInfoRecipe.Text +=
                    $"Количество порций: {_recipeFull.Additional.CountPortions}.{System.Environment.NewLine}";
            if (_recipeFull.Additional?.PrepMinutes != 0)
                _additionalInfoRecipe.Text += $"Количество минут на готовку: {_recipeFull.Additional.PrepMinutes} мин.";

            _urlRecipe.Text = $"Ссылка на рецепт: {_recipeFull.Url}";

            if (!string.IsNullOrEmpty(_recipeFull.Additional?.VideoUrl))
            {
                String videoSource = _recipeFull.Additional.VideoUrl;
                WebSettings webSettings = _videoView.Settings;
                webSettings.JavaScriptEnabled = true;
                _videoView.LoadUrl(videoSource);
            }


            base.OnActivityCreated(savedInstanceState);
        }
    }
}