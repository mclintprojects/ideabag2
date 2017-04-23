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
        private Context parentContext;

        private int[] icons = {Resource.Mipmap.numbers, Resource.Mipmap.text, Resource.Mipmap.network, Resource.Mipmap.enterprise,
                                       Resource.Mipmap.cpu, Resource.Mipmap.web, Resource.Mipmap.file, Resource.Mipmap.database,
                                       Resource.Mipmap.multimedia, Resource.Mipmap.games};

        public CategoryAdapter(List<Category> category, Context context)
        {
            categories = category;
            parentContext = context;
        }

        public override int ItemCount
        {
            get
            {
                return categories.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var category = categories[position];

            var viewHolder = holder as mViewHolder;
            viewHolder.categoryLabel.Text = category.CategoryLbl;
			viewHolder.ideasCount.Text = $"Ideas: {category.CategoryCount}";
            viewHolder.imageView.SetImageResource(icons[position]);
            viewHolder.Root.SetBackgroundColor(Android.Graphics.Color.Transparent);
            if (position == Global.CategoryScrollPosition)
                viewHolder.Root.SetBackgroundResource(Resource.Color.highlight);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.categoryrow, parent, false);
            return new mViewHolder(row, OnClick);
        }

        private void OnClick(int position)
        {
            ItemClick?.Invoke(position);
        }
    }

    public class mViewHolder : RecyclerView.ViewHolder
    {
        public ImageView imageView { get; set; }
        public TextView categoryLabel { get; set; }
        public TextView ideasCount { get; set; }
        public RelativeLayout Root { get; set; }

        public mViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            View row = itemView;
            row.Click += (sender, e) => listener(AdapterPosition);
            imageView = itemView.FindViewById<ImageView>(Resource.Id.categoryIcon);
            categoryLabel = itemView.FindViewById<TextView>(Resource.Id.categoryLbl);
            ideasCount = itemView.FindViewById<TextView>(Resource.Id.ideaCountLbl);
            Root = itemView.FindViewById<RelativeLayout>(Resource.Id.categoryRoot);
        }
    }
}