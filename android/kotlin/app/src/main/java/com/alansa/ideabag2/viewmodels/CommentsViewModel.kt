package com.alansa.ideabag2.viewmodels

import android.arch.lifecycle.ViewModel
import android.databinding.ObservableField
import com.alansa.ideabag2.Global
import com.alansa.ideabag2.extensions.empty
import com.alansa.ideabag2.uimodels.CommentsModel

class CommentsViewModel : ViewModel() {
    val model = CommentsModel()
    val showEmptyState = ObservableField(false)
    val isLoading = ObservableField(true)
    val isLoggedIn = ObservableField(Global.isLoggedIn)
    val comment = ObservableField(String.empty)
    val comments = model.comments

    fun getComments(ideaId: Int, categoryId: Int) {
        model.getComments(ideaId, categoryId) {
            if (it == 0) showEmptyState.set(true) else showEmptyState.set(false)

            isLoading.set(false)
        }
    }

    fun deleteComment(id: String, position: Int) {
        isLoading.set(true)
        model.deleteComment(id, position)
        isLoading.set(false)
    }
}