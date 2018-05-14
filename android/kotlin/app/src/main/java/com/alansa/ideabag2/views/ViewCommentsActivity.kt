package com.alansa.ideabag2.views

import android.arch.lifecycle.Observer
import android.arch.lifecycle.ViewModelProviders
import android.databinding.DataBindingUtil
import android.os.Bundle
import android.support.v7.widget.DefaultItemAnimator
import android.support.v7.widget.LinearLayoutManager
import com.alansa.ideabag2.BaseActivity
import com.alansa.ideabag2.R
import com.alansa.ideabag2.adapters.CommentsAdapter
import com.alansa.ideabag2.databinding.ActivityViewCommentsBinding
import com.alansa.ideabag2.models.Comment
import com.alansa.ideabag2.viewmodels.CommentsViewModel
import com.google.firebase.auth.FirebaseAuth
import kotlinx.android.synthetic.main.activity_view_comments.*

class ViewCommentsActivity : BaseActivity() {
    private lateinit var binding : ActivityViewCommentsBinding
    private lateinit var viewmodel : CommentsViewModel
    private val comments = mutableListOf<Comment>()
    private var categoryId = 0
    private var ideaId = 0
    private lateinit var adapter : CommentsAdapter 

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
    }

    private fun setupList() {
        adapter = CommentsAdapter(comments, FirebaseAuth.getInstance().currentUser?.email, {onDeleteClicked(it)})
        commentsRecyclerView.layoutManager = LinearLayoutManager(this)
        commentsRecyclerView.adapter = adapter
        commentsRecyclerView.itemAnimator = DefaultItemAnimator()
    }

    private fun onDeleteClicked(position: Int) = viewmodel.deleteComment(comments[position].id, position)
}
