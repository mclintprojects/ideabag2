package com.alansa.ideabag2.viewmodels

import android.arch.lifecycle.MutableLiveData
import android.arch.lifecycle.ViewModel
import android.databinding.ObservableField
import com.alansa.ideabag2.models.Category
import com.alansa.ideabag2.uimodels.BookmarksModel

class BookmarksViewModel : ViewModel() {
    val bookmarkedIdeas = MutableLiveData<List<Category.Item>>()
    private val model = BookmarksModel()
    val progress = ObservableField(0)
    val maxProgress = ObservableField(0)
    val showEmptyState = ObservableField(true)

    init {
        val ideas = model.getIdeas()
        bookmarkedIdeas.value = ideas

        val count = model.getCompletedCount()
        if(count > 0)
            progress.set(ideas.size / count)

        maxProgress.set(ideas.size)
        showEmptyState.set(ideas.size == 0)
    }
}