using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ProgrammingIdeas.Adapters;
using ProgrammingIdeas.Api;
using ProgrammingIdeas.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProgrammingIdeas.Activities
{
    [Activity(Label = "View comments", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class ViewCommentsActivity : BaseActivity
    {
        private RecyclerView commentsRecycler;
        private View emptyState;
        private List<IdeaComment> comments;
        private ImageView commentBtn;
        private EditText commentTb;
        private CommentsAdapter commentsAdapter;
        private ProgressBar loadingCircle;

        public override int LayoutResource => Resource.Layout.comments_bottom_sheet;

        public override bool HomeAsUpEnabled => true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetupUI();
            SetupEmptyState();
            SetupComments();
        }

        private void SetupUI()
        {
            loadingCircle = FindViewById<ProgressBar>(Resource.Id.loadingCircle);
            commentsRecycler = FindViewById<RecyclerView>(Resource.Id.commentsRecycler);
            emptyState = FindViewById(Resource.Id.empty);
            commentTb = FindViewById<EditText>(Resource.Id.commentTb);
            commentBtn = FindViewById<ImageView>(Resource.Id.commentBtn);

            commentBtn.Enabled = (Global.LoginData != null);

            commentBtn.RequestFocus();

            commentBtn.Click += delegate
            {
                if (commentTb.Text.Length > 0)
                {
                    var now = DateTime.Now;
                    var comment = new IdeaComment
                    {
                        Author = Global.LoginData.Email,
                        Comment = commentTb.Text,
                        Created = GetJavascriptMillis()
                    };

                    PostComment(comment);
                }
                else
                    Toast.MakeText(this, "Comment cannot be empty", ToastLength.Long).Show();
            };
        }

        private async void SetupComments()
        {
            loadingCircle.Visibility = ViewStates.Visible;
            var response = await IdeaBagApi.Instance.GetCommentsAsync(GetDataId());
            if (response.Payload != null)
            {
                this.comments = response.Payload;
                if (response.Payload.Count == 0)
                    ShowEmptyState();
                else
                    HideEmptyState();

                commentsAdapter = new CommentsAdapter(response.Payload);
                commentsRecycler.SetLayoutManager(new LinearLayoutManager(this));
                commentsRecycler.SetAdapter(commentsAdapter);
                commentsRecycler.SetItemAnimator(new DefaultItemAnimator());

                commentsAdapter.OnDeleteComment += (position) =>
                {
                    var comment = comments[position];
                    RequestDeleteComment(comment.Id, position);
                };
            }
            else
                Snackbar.Make(commentTb, response.ErrorMessage, Snackbar.LengthIndefinite)
                    .SetAction("Retry", (v) => SetupComments())
                    .Show();

            loadingCircle.Visibility = ViewStates.Gone;
        }

        private long GetJavascriptMillis()
        {
            DateTime oldTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan span = (DateTime.Now.ToUniversalTime() - oldTime);
            return (long)span.TotalMilliseconds;
        }

        private async Task PostComment(IdeaComment comment)
        {
            loadingCircle.Visibility = ViewStates.Visible;
            commentBtn.Enabled = false;

            var response = await IdeaBagApi.Instance.PostCommentAsync(GetDataId(), comment);

            if (response.Payload != null)
            {
                HideEmptyState();
                comment.Id = response.Payload;
                comments.Add(comment);
                commentsAdapter.NotifyItemInserted(comments.Count - 1);
                commentTb.Text = string.Empty;
            }
            else
                Snackbar.Make(commentBtn, "Couldn't add comment. Please retry.", Snackbar.LengthLong).Show();

            loadingCircle.Visibility = ViewStates.Gone;
            commentBtn.Enabled = true;
        }

        private void SetupEmptyState()
        {
            emptyState.FindViewById<TextView>(Resource.Id.infoText).Text = "No comments on this idea yet.";
            emptyState.FindViewById<ImageView>(Resource.Id.emptyIcon)
                .SetImageDrawable(AppCompatDrawableManager.Get().GetDrawable(this, Resource.Drawable.no_comments));
            ShowEmptyState();
        }

        private void ShowEmptyState()
        {
            commentsRecycler.Visibility = ViewStates.Gone;
            emptyState.Visibility = ViewStates.Visible;
        }

        private void HideEmptyState()
        {
            commentsRecycler.Visibility = ViewStates.Visible;
            emptyState.Visibility = ViewStates.Gone;
        }

        private void RequestDeleteComment(string id, int position)
        {
            new AlertDialog.Builder(this)
                .SetTitle("Delete comment")
                .SetMessage("Are you sure you want to delete this comment?")
                .SetPositiveButton("Yes", (s, e) => DeleteComment(id, position))
                .SetNegativeButton("No", (s, e) => { return; })
                .Show();
        }

        private async Task DeleteComment(string id, int position)
        {
            loadingCircle.Visibility = ViewStates.Visible;

            var deleted = await IdeaBagApi.Instance.DeleteCommentAsync(GetDataId(), id);
            if (deleted)
            {
                comments.RemoveAt(position);
                commentsAdapter.NotifyItemRemoved(position);

                Toast.MakeText(this, "Comment deleted", ToastLength.Long).Show();

                if (comments.Count == 0)
                    ShowEmptyState();
            }
            else
                Snackbar.Make(commentTb, "Failed to delete comment.", Snackbar.LengthLong).Show();

            loadingCircle.Visibility = ViewStates.Gone;
        }

        private string GetDataId() => $"{Global.CategoryScrollPosition}C-{Intent.GetIntExtra("ideaId", 0)}I";
    }
}