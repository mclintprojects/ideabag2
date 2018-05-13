package com.alansa.ideabag2.views

import android.arch.lifecycle.ViewModelProviders
import android.content.Intent
import android.databinding.DataBindingUtil
import android.os.Bundle
import android.support.design.widget.Snackbar
import android.support.v7.widget.LinearLayoutManager
import com.alansa.ideabag2.BaseActivity
import com.alansa.ideabag2.Global
import com.alansa.ideabag2.R
import com.alansa.ideabag2.adapters.IdeaListAdapter
import com.alansa.ideabag2.databinding.ActivityIdeaListBinding
import com.alansa.ideabag2.dialogs.SetProgressDialog
import com.alansa.ideabag2.extensions.empty
import com.alansa.ideabag2.viewmodels.IdeaListViewModel
import kotlinx.android.synthetic.main.activity_idea_list.*

class IdeaListActivity : BaseActivity() {
    private lateinit var binding: ActivityIdeaListBinding
    private lateinit var viewmodel: IdeaListViewModel
    private lateinit var adapter: IdeaListAdapter

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_idea_list)
        viewmodel = ViewModelProviders.of(this).get(IdeaListViewModel::class.java)
        binding.viewmodel = viewmodel

        setupToolbar()
        supportActionBar?.title = viewmodel.ideas[0].category

        setupList()
    }

    private fun setupList() {
        adapter = IdeaListAdapter(viewmodel.categoryId, viewmodel.ideas, { itemClicked(it) }, { itemLongClicked(it) })
        itemRecyclerView.layoutManager = LinearLayoutManager(this)
        itemRecyclerView.adapter = adapter
    }

    private fun itemLongClicked(position: Int) {
        var dialog = SetProgressDialog() { status ->
            viewmodel.setIdeaProgress(status, position + 1)
            adapter.notifyIdeaStatusChanged(position)
            Snackbar.make(itemRecyclerView, "Progress updated.", Snackbar.LENGTH_SHORT).show()
        }
        dialog.show(supportFragmentManager, String.empty)
    }

    private fun itemClicked(position: Int) {
        startActivity(Intent(this, IdeaDetailActivity::class.java))
        overridePendingTransition(R.anim.push_left_in, R.anim.push_left_out)
        Global.ideaClickIndex = position
    }

    override fun onResume() {
        super.onResume()
        adapter.refresh()
    }
}
