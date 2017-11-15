using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using ProgrammingIdeas.Adapters;
using System.Collections.Generic;

namespace ProgrammingIdeas.Fragment
{
    /// <summary>
    /// Shows the list of newly added ideas
    /// </summary>
    public class NewIdeasFragment : DialogFragment
    {
        private readonly List<Idea> newIdeas;

        public NewIdeasFragment(List<Idea> newIdeas)
        {
            this.newIdeas = newIdeas;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            var view = inflater.Inflate(Resource.Layout.newideasfragment, container, false);
            var recycler = view.FindViewById<RecyclerView>(Resource.Id.newIdeaRecyclerView);
            var adapter = new NewIdeaAdapter(newIdeas);
            recycler.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false));
            recycler.SetAdapter(adapter);
            recycler.SetItemAnimator(new DefaultItemAnimator());
            return view;
        }
    }
}