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

        public event EventHandler<int> ItemClick;

        private Context parentContext;
        private int[] icons;
        private int scrollPos;

        public CategoryAdapter(List<Category> category, Context context, int[] icons, int scrollPos)
        {
            categories = category;
            parentContext = context;
            this.icons = icons;
            this.scrollPos = scrollPos;
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
			viewHolder.ideasCount.Text = $"Ideas: {category.Items.Count}";
            viewHolder.imageView.SetImageResource(icons[position]);
            viewHolder.Root.SetBackgroundColor(Android.Graphics.Color.Transparent);
            if (position == scrollPos)
                viewHolder.Root.SetBackgroundResource(Resource.Color.highlight);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.categoryrow, parent, false);
            return new mViewHolder(row, OnClick);
        }

        private void OnClick(int position)
        {
            if (ItemClick != null)
            {
                ItemClick?.Invoke(this, position);
            }
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