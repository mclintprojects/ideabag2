package com.alansa.ideabag2.views

import android.arch.lifecycle.ViewModelProviders
import android.databinding.DataBindingUtil
import android.os.Bundle
import com.alansa.ideabag2.BaseActivity
import com.alansa.ideabag2.R
import com.alansa.ideabag2.databinding.ActivityViewCommentsBinding
import com.alansa.ideabag2.viewmodels.CommentsViewModel

class ViewCommentsActivity : BaseActivity() {
    lateinit var binding : ActivityViewCommentsBinding
    lateinit var viewmodel : CommentsViewModel
    var categoryId = 0
    var ideaId = 0

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_view_comments)
        viewmodel = ViewModelProviders.of(this).get(CommentsViewModel::class.java)
        binding.viewmodel = viewmodel

        setupToolbar()

        categoryId = intent.getIntExtra("categoryId", 0)
        ideaId = intent.getIntExtra("ideaId", 0)

        viewmodel.getComments(ideaId, categoryId)
    }
}
