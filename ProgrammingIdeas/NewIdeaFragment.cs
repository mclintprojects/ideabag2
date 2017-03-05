using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using System.Collections.Generic;

namespace ProgrammingIdeas
{
    public class NewIdeaFragment : DialogFragment
    {
        private RecyclerView recycler;
        private NewIdeaAdapter adapter;
        private RecyclerView.LayoutManager manager;
        private List<CategoryItem> newIdeas;

        public NewIdeaFragment(List<CategoryItem> newIdeas)
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