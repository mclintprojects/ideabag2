package com.alansa.ideabag2.viewmodels

import android.arch.lifecycle.MutableLiveData
import android.arch.lifecycle.ViewModel
import android.databinding.ObservableField
import com.alansa.ideabag2.models.Category
import com.alansa.ideabag2.uimodels.BookmarksModel

class BookmarksViewModel : ViewModel() {
    val bookmarkedIdeas = MutableLiveData<List<Category.Item>>()
    private val model = BookmarksModel()
    val progress = ObservableField(model.getCompletedCount())
    val maxProgress = ObservableField(0)
    val showEmptyState = ObservableField(true)

    init {
        refresh()
    }

    fun refresh() {
        val ideas = model.getIdeas()
        bookmarkedIdeas.value = ideas
        maxProgress.set(ideas.size)
        showEmptyState.set(ideas.size == 0)
    }
}