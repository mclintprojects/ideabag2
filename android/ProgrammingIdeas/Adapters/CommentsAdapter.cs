using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ProgrammingIdeas.Models;
using System;
using System.Collections.Generic;

namespace ProgrammingIdeas.Adapters
{
    internal partial class CommentsAdapter : RecyclerView.Adapter
    {
        private List<IdeaComment> comments;
        public Action<int> OnDeleteComment;

        public CommentsAdapter(List<IdeaComment> comments)
        {
            this.comments = comments;
        }

        public override int ItemCount => comments.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var comment = comments[position];

            var row = holder as CommentsViewholder;
            row.Author.Text = comment.Author;
            row.Comment.Text = comment.Comment;

            row.DeleteBtn.Visibility = (comment.Author == Global.LoginData.Email) ? ViewStates.Visible : ViewStates.Invisible;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.commentrow, parent, false);
            return new CommentsViewholder(row, OnDeleteComment);
        }

        private class CommentsViewholder : RecyclerView.ViewHolder
        {
            public TextView Author { get; set; }
            public TextView Comment { get; set; }
            public ImageView DeleteBtn { get; set; }

            public CommentsViewholder(View view, Action<int> OnDeleteComment) : base(view)
            {
                Author = view.FindViewById<TextView>(Resource.Id.authorLbl);
                Comment = view.FindViewById<TextView>(Resource.Id.commentLbl);
                DeleteBtn = view.FindViewById<ImageView>(Resource.Id.deleteBtn);

                DeleteBtn.Click += delegate
                {
                    OnDeleteComment?.Invoke(AdapterPosition);
                };
            }
        }
    }
}