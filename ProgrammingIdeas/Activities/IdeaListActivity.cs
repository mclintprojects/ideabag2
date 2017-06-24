using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ProgrammingIdeas.Adapters;
using ProgrammingIdeas.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;
using Android.Support.Design.Widget;

namespace ProgrammingIdeas.Activities
{
	[Activity(Label = "ItemActivity", Theme = "@style/AppTheme")]
	public class IdeaListActivity : BaseActivity, PopupMenu.IOnMenuItemClickListener
	{
		private RecyclerView recyclerView;
		private RecyclerView.LayoutManager manager;
		private IdeaListAdapter adapter;
		private List<Category> allCategories = new List<Category>();
		private List<Idea> ideasList;
		private ProgressBar progressBar;
		private string ideasdb = Path.Combine(Global.APP_PATH, "ideasdb");

		public override int LayoutResource => Resource.Layout.idealistactivity;

		public override bool HomeAsUpEnabled => true;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			manager = new LinearLayoutManager(this);
			recyclerView = FindViewById<RecyclerView>(Resource.Id.itemRecyclerView);
			progressBar = FindViewById<ProgressBar>(Resource.Id.completedIdeasBar);
			allCategories = Global.Categories;
			progressBar.Max = allCategories[Global.CategoryScrollPosition].Items.Count;
			SetupUI();
		}

		protected override void OnResume()
		{
			base.OnResume();
			if (Global.RefreshBookmarks)
				adapter.RefreshBookmarks();
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.idea_list_menu, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.sortIdeas:
					var sortAnchor = FindViewById(Resource.Id.sortIdeas);
					ShowSortPopup(sortAnchor);
					return true;
			}
			return base.OnOptionsItemSelected(item);
		}

		private void ShowSortPopup(View sortAnchor)
		{
			var popup = new PopupMenu(this, sortAnchor);
			popup.MenuInflater.Inflate(Resource.Menu.idea_sort_menu, popup.Menu);
			popup.SetOnMenuItemClickListener(this);
			popup.Show();
		}

		private void ShowProgress()
		{
			var completedCount = allCategories[Global.CategoryScrollPosition].Items.FindAll(x => x.State == Status.Done).Count;
			progressBar.Progress = 0;
			progressBar.IncrementProgressBy(completedCount);
		}

		private void SetupUI()
		{
			var title = Global.Categories[Global.CategoryScrollPosition].CategoryLbl;
			ideasList = Global.Categories[Global.CategoryScrollPosition].Items;

			Title = title;
			recyclerView.SetLayoutManager(manager);
			adapter = new IdeaListAdapter(ideasList);
			adapter.ItemClick -= OnItemClick;
			adapter.ItemClick += OnItemClick;
			recyclerView.SetAdapter(adapter);
			manager.ScrollToPosition(Global.ItemScrollPosition);
			adapter.StateClicked -= Adapter_StateClicked;
			adapter.StateClicked += Adapter_StateClicked;

			ShowProgress();
		}

		void Adapter_StateClicked(string title, string state, int adapterPos)
		{
			if (ideasList != null && ideasList.Count != 0)
			{
				var changedItem = ideasList.ElementAt(adapterPos);
				if (changedItem != null)
				{
					changedItem.State = state;
					adapter.NotifyItemChanged(adapterPos);
					ShowProgress();
				}
			}
		}

		protected override void OnPause()
		{
			DBAssist.SerializeDBAsync(ideasdb, allCategories);
			base.OnPause();
		}

		private void OnItemClick(int position)
		{
			Global.ItemScrollPosition = position;
			StartActivity(new Intent(this, typeof(IdeaDetailsActivity)));
			OverridePendingTransition(Resource.Animation.push_left_in, Resource.Animation.push_left_out);
		}

		public bool OnMenuItemClick(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.sortByName:
					var sortedList = ideasList.OrderBy(x => x.Title).ToList();
					ideasList.Clear();
					ideasList.AddRange(sortedList);
					adapter.NotifyDataSetChanged();
					return true;

				case Resource.Id.sortByDifficulty:
					var sortedDiffList = ideasList.OrderBy(x => x.GetDifficultyId()).ToList();
					ideasList.Clear();
					ideasList.AddRange(sortedDiffList);
					adapter.NotifyDataSetChanged();
					return true;

				case Resource.Id.sortByStatus:
					var sortedStatusList = ideasList.OrderBy(x => x.GetStatusId()).ToList();
					ideasList.Clear();
					ideasList.AddRange(sortedStatusList);
					adapter.NotifyDataSetChanged();
					return true;

				default:
					return true;
			}
		}
	}
}