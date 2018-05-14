package com.alansa.ideabag2.viewmodels

import android.arch.lifecycle.ViewModel
import android.databinding.ObservableField
import com.alansa.ideabag2.uimodels.CommentsModel
import kotlinx.coroutines.experimental.android.UI
import kotlinx.coroutines.experimental.launch

class CommentsViewModel : ViewModel() {
    val showEmptyState = ObservableField(false)
    val isLoading = ObservableField(true)

    val model = CommentsModel()

    fun getComments(ideaId: Int, categoryId: Int) {
        launch(UI) {
            model.getComments(ideaId, categoryId)
            isLoading.set(false)
        }
    }
}