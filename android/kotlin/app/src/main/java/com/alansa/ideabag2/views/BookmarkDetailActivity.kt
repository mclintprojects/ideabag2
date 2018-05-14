package com.alansa.ideabag2.views

import android.arch.lifecycle.ViewModelProviders
import android.content.Intent
import android.databinding.DataBindingUtil
import android.os.Bundle
import android.support.v4.content.ContextCompat
import android.view.Menu
import android.view.MenuItem
import com.alansa.ideabag2.BaseActivity
import com.alansa.ideabag2.Global
import com.alansa.ideabag2.R
import com.alansa.ideabag2.databinding.ActivityBookmarkDetailBinding
import com.alansa.ideabag2.viewmodels.BookmarkDetailViewModel
import kotlinx.android.synthetic.main.activity_idea_detail.*

class BookmarkDetailActivity : BaseActivity() {
    private lateinit var binding: ActivityBookmarkDetailBinding
    private lateinit var viewmodel: BookmarkDetailViewModel
    private lateinit var bookmarkItem: MenuItem

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_bookmark_detail)
        viewmodel = ViewModelProviders.of(this).get(BookmarkDetailViewModel::class.java)
        binding.viewmodel = viewmodel

        setupToolbar()

        viewmodel.setBookmark(intent.getStringExtra("category"), intent.getIntExtra("ideaId", 0))
        viewmodel.showIdea()
        supportActionBar?.title = viewmodel.idea.get()!!.category

        addNotefab.setOnClickListener { addOrUpdateNote() }
        editNoteBtn.setOnClickListener { addOrUpdateNote() }
        deleteNote.setOnClickListener { viewmodel.deleteNote() }
    }

    private fun addOrUpdateNote() {
        var intent = Intent(this, AddNoteActivity::class.java)
        intent.putExtra("ideaId", this.intent.getIntExtra("ideaId", 0))
        intent.putExtra("categoryId", viewmodel.categoryId)
        startActivity(intent)
        overridePendingTransition(R.anim.push_left_in, R.anim.push_left_out)
    }

    override fun onResume() {
        super.onResume()
        viewmodel.refreshNote()
    }

    override fun onCreateOptionsMenu(menu: Menu?): Boolean {
        menuInflater.inflate(R.menu.item_details_menu, menu)
        bookmarkItem = menu?.findItem(R.id.bookmarkItem)!!

        if (viewmodel.isBookmarked) bookmarkItem.icon = ContextCompat.getDrawable(this, R.drawable.ic_bookmark_white_24dp)

        return true
    }

    private fun setAppropriateBookmarkIcon() {
        if (!viewmodel.isBookmarked)
            bookmarkItem.icon = ContextCompat.getDrawable(this, R.drawable.ic_bookmark_border_white_24dp)
        else
            bookmarkItem.icon = ContextCompat.getDrawable(this, R.drawable.ic_bookmark_white_24dp)
    }

    override fun onOptionsItemSelected(item: MenuItem?): Boolean {
        when (item?.itemId) {
            R.id.bookmarkItem -> if (viewmodel.isBookmarked) {
                viewmodel.removeBookmark()
                setAppropriateBookmarkIcon()
            } else {
                viewmodel.addBookmark()
                setAppropriateBookmarkIcon()
            }

            R.id.shareItem -> shareIdea()

            R.id.viewComments -> viewComment()
        }
        return super.onOptionsItemSelected(item)
    }

    private fun viewComment() {
        val intent = Intent(this, ViewCommentsActivity::class.java)
        intent.putExtra("categoryId", viewmodel.categoryId)
        intent.putExtra("ideaId", viewmodel.ideaId)
        startActivity(intent)
        overridePendingTransition(R.anim.push_left_in, R.anim.push_left_out)
    }

    private fun shareIdea() {
        var intent = Intent(Intent.ACTION_SEND)
        intent.type = "text/plain"

        val idea = viewmodel.idea.get()!!
        val textToShare = "Can you code this challenge?\r\n\r\n" +
                "Title: ${idea.title}\r\nDifficulty: ${idea.difficulty}\r\n\r\n${idea.description}\r\n\r\n" +
                "Want more coding ideas? Get the app here: https://play.google.com/store/apps/details?id=com.alansa.ideabag2";

        intent.putExtra(Intent.EXTRA_TEXT, textToShare)
        startActivity(Intent.createChooser(intent, "Share idea via"))
    }
}