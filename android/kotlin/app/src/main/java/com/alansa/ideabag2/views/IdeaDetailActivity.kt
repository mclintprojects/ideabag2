package com.alansa.ideabag2.views

import android.arch.lifecycle.ViewModelProviders
import android.content.Intent
import android.databinding.DataBindingUtil
import android.os.Bundle
import android.support.v4.content.ContextCompat
import android.view.Menu
import android.view.MenuItem
import android.view.animation.AnticipateOvershootInterpolator
import com.alansa.ideabag2.BaseActivity
import com.alansa.ideabag2.R
import com.alansa.ideabag2.databinding.ActivityIdeaDetailBinding
import com.alansa.ideabag2.extensions.animateView
import com.alansa.ideabag2.utils.OnSwipeListener
import com.alansa.ideabag2.utils.Swipe
import com.alansa.ideabag2.viewmodels.IdeaDetailViewModel
import kotlinx.android.synthetic.main.activity_idea_detail.*

class IdeaDetailActivity : BaseActivity() {
    private lateinit var binding: ActivityIdeaDetailBinding
    private lateinit var viewmodel: IdeaDetailViewModel
    private lateinit var bookmarkItem: MenuItem

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_idea_detail)
        viewmodel = ViewModelProviders.of(this).get(IdeaDetailViewModel::class.java)
        binding.viewmodel = viewmodel

        setupToolbar()
        supportActionBar?.title = viewmodel.idea.get()!!.category

        addNotefab.setOnClickListener { addOrUpdateNote() }
        editNoteBtn.setOnClickListener { addOrUpdateNote() }
        deleteNote.setOnClickListener { viewmodel.deleteNote() }

        var swipeListener = OnSwipeListener(this, { onSwipe(it) })
        detailsView.setOnTouchListener(swipeListener)
        itemDescription.setOnTouchListener(swipeListener)
    }

    private fun addOrUpdateNote() {
        var intent = Intent(this, AddNoteActivity::class.java)
        intent.putExtra("ideaId", viewmodel.idea.get()!!.id)
        startActivity(intent)
        overridePendingTransition(R.anim.push_left_in, R.anim.push_left_out)
    }

    private fun onSwipe(swipe: Swipe) {
        when (swipe) {
            Swipe.RIGHT -> {
                viewmodel.swipedRight()
                detailsView.animateView("translationX", 400, AnticipateOvershootInterpolator(), -400f, 0f)
                setAppropriateBookmarkIcon()
            }
            Swipe.LEFT -> {
                viewmodel.swipedLeft()
                detailsView.animateView("translationX", 400, AnticipateOvershootInterpolator(), 400f, 0f)
                setAppropriateBookmarkIcon()
            }
        }
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
        if(!viewmodel.isBookmarked)
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