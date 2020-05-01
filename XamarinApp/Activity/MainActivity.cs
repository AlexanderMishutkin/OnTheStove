﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ObjectsLibrary;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XamarinAppLibrary;

namespace XamarinApp
{
    [Activity(Label = "На плите!", Theme = "@style/AppTheme.NoActionBar", Icon = "@drawable/icon", MainLauncher = true, 
        ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private RecyclerView recyclerView;
        private RecipeAdapter recipeAdapter;
        private DrawerLayout drawer;
        private SwipeRefreshLayout swipeRefreshLayout;
        private LinearLayoutManager linearLayoutManager;
        private NavigationView navigationView;
        private Button buttonMenu;
        private RecipeListener recipeListener;
        private EditText editText;
        private Spinner spinner;
        private ArrayAdapter spinnerAdapter;
        private ActionBarDrawerToggle actionBarDrawerToggle;

        private List<RecipeShort> recipeShorts;
        private int page = 1;
        private string lastQuery;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_search);

            // Определяем компоненты: 
            recyclerView = FindViewById<RecyclerView>(Resource.Id.listRecipeShorts);
            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            buttonMenu = FindViewById<Button>(Resource.Id.menu_button);
            editText = FindViewById<EditText>(Resource.Id.TextFind);
            spinner = FindViewById<Spinner>(Resource.Id.spinner);

            linearLayoutManager = new LinearLayoutManager(this);
            recipeListener = new RecipeListener(linearLayoutManager);

            // Инициализируем элементы:
            swipeRefreshLayout.SetColorSchemeColors(Color.Orange, Color.DarkOrange);
            swipeRefreshLayout.Refresh += RefreshLayout;

            recyclerView.AddOnScrollListener(recipeListener);
            recipeListener.LoadMoreEvent += LoadMoreElements;

            editText.KeyPress += FindByRecipeName;

            actionBarDrawerToggle = new ActionBarDrawerToggle(this, drawer, 
                Resource.String.navigation_drawer_open,Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(actionBarDrawerToggle);
            actionBarDrawerToggle.SyncState();
            navigationView.SetNavigationItemSelectedListener(this);
            buttonMenu.SetBackgroundResource(Resources.GetIdentifier("round_menu_24", "drawable", PackageName));
            buttonMenu.Click += SetMenuButtonClick;

            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SelectedItemSpinner);
            spinnerAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.sort_array, Resource.Layout.spinner_text);
            spinnerAdapter.SetDropDownViewResource(Resource.Layout.spinner_text);
            spinner.Adapter = spinnerAdapter;
        }

        private void FindByRecipeName(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            page = 1;

            if (e.Event.Action != KeyEventActions.Down || e.KeyCode != Keycode.Enter)
                return;

            lastQuery = $"section=recipe&recipeName={editText.Text}&page={page}";
            UpdateListView(lastQuery);

            //Toast.MakeText(this, "Загрузка...", ToastLength.Short).Show();
            e.Handled = true;
        }

        private void LoadMoreElements(object sender, EventArgs e)
        {
            // Получаем строку для нового запроса:
            string query = lastQuery.Substring(0, lastQuery.IndexOf("page=") + 5) + (++page);
            // Обновляем коллекцию:
            UpdateListView(query, recipeShorts);
        }

        private void RefreshLayout(object sender, EventArgs e)
        {
            // Обновление странички.
            if (lastQuery == null)
                UpdateListView();
            else
                UpdateListView(lastQuery);
        }

        private void SetMenuButtonClick(object sender, EventArgs args)
        {
            // Кнопка "меню" на toolbar.
            drawer.OpenDrawer(GravityCompat.Start);
        }

        private async void UpdateListView(string query = "section=new", List<RecipeShort> recipeShorts = null)
        {
            // Запустить кружочек.
            swipeRefreshLayout.Post(() =>
            {
                swipeRefreshLayout.Refreshing = true;
                recyclerView.Clickable = false;
            });

            recyclerView.SetLayoutManager(linearLayoutManager);

            if (recipeShorts == null)
            {
                // Тут какой-то костыль, трогать я не буду, простите :(
                // Из книги "Техники индуса-программиста-под-андройд-на-ксамарине-с-нуля".
                this.recipeShorts = await UpdateCollectionRecipes(query);
                recipeAdapter = new RecipeAdapter(this.recipeShorts, this);
                recipeAdapter.ItemClick += OnRecipeClick;
                recyclerView.SetAdapter(recipeAdapter);
            }
            else
            {
                var newRecipes = await UpdateCollectionRecipes(query);
                recipeAdapter.AddItems(newRecipes);
                recipeShorts.AddRange(newRecipes);
            }

            // Остановить кружочек.
            swipeRefreshLayout.Post(() =>
            {
                swipeRefreshLayout.Refreshing = false;
                recyclerView.Clickable = true;
            });

        }

        public override void OnBackPressed()
        {
            if (drawer.IsDrawerOpen(GravityCompat.Start))
                drawer.CloseDrawer(GravityCompat.Start);
            else
                base.OnBackPressed();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        void OnRecipeClick(object sender, int position)
        {
            Intent intent = new Intent(this, typeof(RecipeActivity));
            intent.PutExtra("url", recipeShorts[position].Url);
            intent.PutExtra("recipeShort", Data.RecipeToByteArray(recipeShorts[position]));

            StartActivity(intent);
        }


        private void SelectedItemSpinner(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var item = spinner.GetItemAtPosition(e.Position);

            page = 1;
            string query = null;

            switch (item.ToString())
            {
                case "Популярные рецепты":
                    query = $"section=popular&page={page}";
                    UpdateListView(query);
                    break;
                case "Случайные рецепты":
                    query = $"section=random&page={page}";
                    UpdateListView(query);
                    break;
                case "Новые рецепты":
                    query = $"section=new&page={page}";
                    UpdateListView(query);
                    break;
            }

            lastQuery = query;
        }

        private async Task<List<RecipeShort>> UpdateCollectionRecipes(string query)
        {
            return await Task.Run(function: () => HttpGet.GetPages(query));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            // Закрывает отрисовщик и возвращает, закрыт ли он или нет.
            int id = menuItem.ItemId;

            if (id == Resource.Id.nav_favorite)
            {
               Intent intent = new Intent(this, typeof(SavedRecipesActivity));
               StartActivity(intent);
            }
            if (id == Resource.Id.nav_cart)
            {
                Intent intent = new Intent(this, typeof(SavedIngredientsActivity));
                StartActivity(intent);
            }

            return CLoseDrawer(drawer).IsDrawerOpen(GravityCompat.Start);
        }

        private static DrawerLayout CLoseDrawer(DrawerLayout drawerLayout)
        {
            drawerLayout.CloseDrawer(GravityCompat.Start);
            return drawerLayout;
        }
    }
}