using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace ProgrammingIdeas.Adapters
{
	public class NewIdeaAdapter : RecyclerView.Adapter
	{
		private List<Idea> newIdeas;
		private int count;

		public NewIdeaAdapter(List<Idea> newIdeas)
		{
			this.newIdeas = newIdeas;
		}

		public override int ItemCount => newIdeas.Count;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var newIdea = newIdeas[position];
			var newIdeasHolder = holder as NewIdeasViewHolder;
			newIdeasHolder.NewIdeaTitle.Text = newIdea.Title;
			newIdeasHolder.NewIdeaCategory.Text = newIdea.Category;
			newIdeasHolder.NewIdeaContent.Text = newIdea.Description;
			count++;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.newideasrow, parent, false);
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