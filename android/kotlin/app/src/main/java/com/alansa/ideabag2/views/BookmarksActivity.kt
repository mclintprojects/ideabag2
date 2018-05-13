package com.alansa.ideabag2.views

import android.arch.lifecycle.Observer
import android.arch.lifecycle.ViewModelProviders
import android.content.Intent
import android.databinding.DataBindingUtil
import android.os.Bundle
import android.support.design.widget.Snackbar
import android.support.v7.widget.LinearLayoutManager
import com.alansa.ideabag2.BaseActivity
import com.alansa.ideabag2.Global
import com.alansa.ideabag2.R
import com.alansa.ideabag2.adapters.BookmarksAdapter
import com.alansa.ideabag2.databinding.ActivityBookmarksBinding
import com.alansa.ideabag2.dialogs.SetProgressDialog
import com.alansa.ideabag2.extensions.empty
import com.alansa.ideabag2.models.Category
import com.alansa.ideabag2.viewmodels.BookmarksViewModel
import kotlinx.android.synthetic.main.activity_bookmarks.*

class BookmarksActivity : BaseActivity() {
    private lateinit var binding: ActivityBookmarksBinding
    private lateinit var viewmodel: BookmarksViewModel
    private var bookmarkedIdeas = mutableListOf<Category.Item>()
    lateinit var adapter: BookmarksAdapter

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_bookmarks)
        viewmodel = ViewModelProviders.of(this).get(BookmarksViewModel::class.java)
        binding.viewmodel = viewmodel

        setupToolbar()
        setupList()

        viewmodel.bookmarkedIdeas.observe(this, Observer {
            bookmarkedIdeas.clear()
            bookmarkedIdeas.addAll(it!!)
        })
    }

    override fun onResume() {
        super.onResume()
        viewmodel.refresh()
        adapter.refresh()
    }

    private fun setupList() {
        adapter = BookmarksAdapter(bookmarkedIdeas, { onItemClick(it) }, { itemLongClicked(it) })
        bookmarkRecyclerView.layoutManager = LinearLayoutManager(this)
        bookmarkRecyclerView.adapter = adapter
    }

    private fun itemLongClicked(position: Int) {
        var dialog = SetProgressDialog() { status ->
            var idea = bookmarkedIdeas[position]
            viewmodel.setIdeaProgress(status, idea.category, idea.id)
            adapter.notifyIdeaStatusChanged(position)
            Snackbar.make(bookmarkRecyclerView, "Progress updated.", Snackbar.LENGTH_SHORT).show()
        }
        dialog.show(supportFragmentManager, String.empty)
    }

    private fun onItemClick(position: Int) {
        var idea = bookmarkedIdeas[position]

        var intent = Intent(this, BookmarkDetailActivity::class.java)
        intent.putExtra("ideaId", idea.id)
        intent.putExtra("category", idea.category)

        startActivity(intent)
        overridePendingTransition(R.anim.push_left_in, R.anim.push_left_out)
        Global.bookmarkClickIndex = position
    }
}