using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using ProgrammingIdeas.Animation;
using System;
using System.Collections.Generic;
using ProgrammingIdeas;
using ProgrammingIdeas.Adapters;

namespace Adapters
{
	public class BookmarkListAdapter : RecyclerView.Adapter
	{
		private List<Idea> itemsList;
		public Action<int> ItemClick;
		public Action<string, string, int> StateClicked;

		public BookmarkListAdapter(List<Idea> list)
		{
			itemsList = list;
		}

		public override int ItemCount => itemsList.Count;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var itemHolder = holder as ItemViewHolder;
			var item = itemsList[position];

			itemHolder.Difficulty.Text = item.Difficulty;
			itemHolder.Title.Text = item.Title;
			itemHolder.Id.Text = item.Id.ToString();
			itemHolder.State.SetBackgroundResource(Resource.Color.undecidedColor);
			itemHolder.Root.SetBackgroundColor(Color.Transparent);
			switch (item.State)
			{
				case Status.Undecided:
					itemHolder.State.SetBackgroundResource(Resource.Color.undecidedColor);
					break;

				case Status.InProgress:
					itemHolder.State.SetBackgroundResource(Resource.Color.inProgressColor);
					break;

				case Status.Done:
					itemHolder.State.SetBackgroundResource(Resource.Color.doneColor);
					break;
			}
			if (position == Global.ItemScrollPosition)
				itemHolder.Root.SetBackgroundResource(Resource.Color.highlight);
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.idealistrow, parent, false);
			return new ItemViewHolder(row, itemsList, ItemClick, StateClicked);
		}
	}
}