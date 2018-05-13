package com.alansa.ideabag2.viewmodels

import android.arch.lifecycle.ViewModel
import android.databinding.ObservableField
import com.alansa.ideabag2.uimodels.IdeaListModel

class IdeaListViewModel : ViewModel() {
    private val model = IdeaListModel()
    val ideas = model.ideas
    val categoryId = model.categoryId
    val progress = ObservableField(0)

    init {
        var completedCount = model.getCompletedCount()
        if (completedCount > 0)
            progress.set(model.ideas.size / completedCount)
    }
}