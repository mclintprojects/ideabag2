package com.alansa.ideabag2.viewmodels

import android.arch.lifecycle.ViewModel
import android.databinding.ObservableField
import com.alansa.ideabag2.models.CompletionStatus
import com.alansa.ideabag2.uimodels.IdeaListModel
import com.alansa.ideabag2.utils.SortOption

class IdeaListViewModel : ViewModel() {
    private val model = IdeaListModel()
    val ideas = model.ideas
    val categoryId = model.categoryId
    val progress = ObservableField(model.getCompletedCount())

    fun setIdeaProgress(progress: CompletionStatus, ideaId: Int) {
        model.setProgress(progress, ideaId)
        this.progress.set(model.getCompletedCount())
    }

    fun sortIdeas(option: SortOption) = model.sortIdeas(option)
}