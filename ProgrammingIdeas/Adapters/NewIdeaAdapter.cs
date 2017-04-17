using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;

namespace ProgrammingIdeas.Adapters
{
    public class NewIdeaAdapter : RecyclerView.Adapter
    {
        private List<CategoryItem> newIdeas;
        private int count;

        public NewIdeaAdapter(List<CategoryItem> newIdeas)
        {
            this.newIdeas = newIdeas;
        }

        public override int ItemCount
        {
            get
            {
                return newIdeas.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
			var newIdea = newIdeas[position];
            var idHolder = holder as NewIdeasViewHolder;
            idHolder.NewIdeaTitle.Text = newIdea.Title;
            idHolder.NewIdeaCategory.Text = newIdea.Category;
            idHolder.NewIdeaContent.Text = newIdea.Description;
            count++;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.newideasrow, null);
            return new NewIdeasViewHolder(row);
        }
    }

    public class NewIdeasViewHolder : RecyclerView.ViewHolder
    {
        public TextView NewIdeaTitle { get; set; }
        public TextView NewIdeaCategory { get; set; }
        public TextView NewIdeaContent { get; set; }

        public NewIdeasViewHolder(View itemView) : base(itemView)
        {
            NewIdeaTitle = itemView.FindViewById<TextView>(Resource.Id.newIdeaTitle);
            NewIdeaCategory = itemView.FindViewById<TextView>(Resource.Id.newIdeaCategory);
            NewIdeaContent = itemView.FindViewById<TextView>(Resource.Id.newIdeaContent);
        }
    }
}