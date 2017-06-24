using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace ProgrammingIdeas.Adapters
{
	public class CategoryAdapter : RecyclerView.Adapter
	{
		private List<Category> categories;
		public Action<int> ItemClick;

		private int[] icons = {Resource.Mipmap.numbers, Resource.Mipmap.text, Resource.Mipmap.network, Resource.Mipmap.enterprise,
									   Resource.Mipmap.cpu, Resource.Mipmap.web, Resource.Mipmap.file, Resource.Mipmap.database,
									   Resource.Mipmap.multimedia, Resource.Mipmap.games};

		public CategoryAdapter(List<Category> category)
		{
			categories = category;
		}

		public override int ItemCount => categories.Count;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var category = categories[position];

			var viewHolder = holder as CategoryViewholder;
			viewHolder.CategoryLabel.Text = category.CategoryLbl;
			viewHolder.IdeasCount.Text = $"Ideas: {category.CategoryCount}";
			viewHolder.Icon.SetImageResource(icons[position]);
			viewHolder.Root.SetBackgroundColor(Android.Graphics.Color.Transparent);
			if (position == Global.CategoryScrollPosition)
				viewHolder.Root.SetBackgroundResource(Resource.Color.highlight);
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.categoryrow, parent, false);
			return new CategoryViewholder(row, ItemClick);
		}
	}

	public class CategoryViewholder : RecyclerView.ViewHolder
	{
		public ImageView Icon { get; set; }
		public TextView CategoryLabel { get; set; }
		public TextView IdeasCount { get; set; }
		public RelativeLayout Root { get; set; }

		public CategoryViewholder(View itemView, Action<int> listener) : base(itemView)
		{
			itemView.Click += (sender, e) => listener?.Invoke(AdapterPosition);
			Icon = itemView.FindViewById<ImageView>(Resource.Id.categoryIcon);
			CategoryLabel = itemView.FindViewById<TextView>(Resource.Id.categoryLbl);
			IdeasCount = itemView.FindViewById<TextView>(Resource.Id.ideaCountLbl);
			Root = itemView.FindViewById<RelativeLayout>(Resource.Id.categoryRoot);
		}
	}
}