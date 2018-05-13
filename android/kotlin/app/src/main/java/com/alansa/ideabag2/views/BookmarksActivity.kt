package com.alansa.ideabag2.views

import android.arch.lifecycle.ViewModelProviders
import android.databinding.DataBindingUtil
import android.os.Bundle
import com.alansa.ideabag2.BaseActivity
import com.alansa.ideabag2.R
import com.alansa.ideabag2.databinding.ActivityBookmarksBinding
import com.alansa.ideabag2.viewmodels.BookmarksViewModel

class BookmarksActivity : BaseActivity() {
    private lateinit var binding : ActivityBookmarksBinding
    private lateinit var viewmodel : BookmarksViewModel

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_bookmarks)
        viewmodel = ViewModelProviders.of(this).get(BookmarksViewModel::class.java)
        binding.viewmodel = viewmodel
    }
}