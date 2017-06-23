using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using ProgrammingIdeas.Adapters;
using System.Collections.Generic;

namespace ProgrammingIdeas.Fragment
{
    public class NewIdeaFragment : DialogFragment
    {
        private RecyclerView recycler;
        private NewIdeaAdapter adapter;
        private RecyclerView.LayoutManager manager;
        private List<Idea> newIdeas;

        public NewIdeaFragment(List<Idea> newIdeas)
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
            recycler = view.FindViewById<RecyclerView>(Resource.Id.newIdeaRecyclerView);
            adapter = new NewIdeaAdapter(newIdeas);
            manager = new LinearLayoutManager(Activity.BaseContext, LinearLayoutManager.Horizontal, false);
            recycler.SetLayoutManager(manager);
            recycler.SetAdapter(adapter);
            recycler.SetItemAnimator(new DefaultItemAnimator());
            return view;
        }
    }
}