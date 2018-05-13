package com.alansa.ideabag2.viewmodels

import android.arch.lifecycle.LifecycleOwner
import android.arch.lifecycle.MutableLiveData
import android.arch.lifecycle.Observer
import android.arch.lifecycle.ViewModel
import android.databinding.ObservableField
import com.alansa.ideabag2.models.Category
import com.alansa.ideabag2.uimodels.CategoryModel
import kotlinx.coroutines.experimental.android.UI
import kotlinx.coroutines.experimental.launch

/**
 * Created by Mbah Clinton on 5/10/2018.
 */
class CategoryViewModel() : ViewModel() {
    val isLoading = ObservableField<Boolean>()
    val categories = MutableLiveData<List<Category>>()
    var newIdeas = listOf<Category>()


    var owner: LifecycleOwner? = null
    private var model = CategoryModel()

    init {
        launch(UI) {
            isLoading.set(true)
            model.ideas.observe(owner!!, Observer {
                categories.value = it
                isLoading.set(false)
            })
        }
    }

    fun retryDownload() {
        launch(UI) {
            isLoading.set(true)
            model.downloadIdeas()
        }
    }

    fun invalidateCache() {
        model.invalidateCache()
    }
}