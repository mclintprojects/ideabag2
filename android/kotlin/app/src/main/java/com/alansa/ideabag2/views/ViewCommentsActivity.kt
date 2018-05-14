package com.alansa.ideabag2.views

import android.arch.lifecycle.Observer
import android.arch.lifecycle.ViewModelProviders
import android.databinding.DataBindingUtil
import android.os.Bundle
import android.support.v7.app.AlertDialog
import android.support.v7.widget.DefaultItemAnimator
import android.support.v7.widget.LinearLayoutManager
import android.widget.Toast
import com.alansa.ideabag2.BaseActivity
import com.alansa.ideabag2.R
import com.alansa.ideabag2.adapters.CommentsAdapter
import com.alansa.ideabag2.databinding.ActivityViewCommentsBinding
import com.alansa.ideabag2.models.Comment
import com.alansa.ideabag2.viewmodels.CommentsViewModel
import com.google.firebase.auth.FirebaseAuth
import kotlinx.android.synthetic.main.activity_view_comments.*

class ViewCommentsActivity : BaseActivity() {
    private lateinit var binding: ActivityViewCommentsBinding
    private lateinit var viewmodel: CommentsViewModel
    private val comments = mutableListOf<Comment>()
    private var categoryId = 0
    private var ideaId = 0
    private lateinit var adapter: CommentsAdapter

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_view_comments)
        viewmodel = ViewModelProviders.of(this).get(CommentsViewModel::class.java)
        binding.viewmodel = viewmodel

        setupToolbar()

        categoryId = intent.getIntExtra("categoryId", 0)
        ideaId = intent.getIntExtra("ideaId", 0)

        viewmodel.getComments(ideaId, categoryId)

        viewmodel.comments.observe(this, Observer {
            comments.clear()
            comments.addAll(it!!)
            adapter.notifyDataSetChanged()
        })

        setupList()

        commentBtn.setOnClickListener {
            if (commentTb.text.length > 0) {
                viewmodel.postComment()
            } else
                Toast.makeText(this, R.string.required, Toast.LENGTH_SHORT).show()
        }
    }

    private fun setupList() {
        adapter = CommentsAdapter(comments, FirebaseAuth.getInstance().currentUser?.email, { onDeleteClicked(it) })
        binding.commentsRecyclerView.layoutManager = LinearLayoutManager(this)
        binding.commentsRecyclerView.adapter = adapter
        binding.commentsRecyclerView.itemAnimator = DefaultItemAnimator()
    }

    private fun onDeleteClicked(position: Int) {
        AlertDialog.Builder(this)
                .setTitle(R.string.delete_comment)
                .setMessage(R.string.delete_comment_confirm_message)
                .setPositiveButton(R.string.yes, { _, _ -> viewmodel.deleteComment(comments[position].id, position) })
                .setNegativeButton(R.string.no, { _, _ -> })
                .create().show()

    }
}
