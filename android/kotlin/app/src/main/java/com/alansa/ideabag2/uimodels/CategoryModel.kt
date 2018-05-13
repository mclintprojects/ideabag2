package com.alansa.ideabag2.uimodels

import android.arch.lifecycle.MutableLiveData
import com.alansa.ideabag2.extensions.fromJson
import com.alansa.ideabag2.models.Category
import com.github.kittinunf.fuel.core.FuelError
import com.github.kittinunf.fuel.core.Request
import com.github.kittinunf.fuel.core.Response
import com.github.kittinunf.fuel.httpGet
import com.github.kittinunf.result.Result
import com.google.gson.Gson
import io.paperdb.Paper
import kotlinx.coroutines.experimental.CommonPool
import kotlinx.coroutines.experimental.android.UI
import kotlinx.coroutines.experimental.async
import kotlinx.coroutines.experimental.launch

/**
 * Created by Mbah Clinton on 5/10/2018.
 */
class CategoryModel {
    val ideasUrl = "https://docs.google.com/document/d/17V3r4fJ2udoG5woDBW3IVqjxZdfsbZC04G1A-It_DRU/export?format=txt"
    val ideas = MutableLiveData<List<Category>>()

    init {
        launch(UI) { downloadIdeas() }
    }

    suspend fun downloadIdeas() {
        if (Paper.book().contains("ideas"))
            this.ideas.value = Paper.book().read("ideas");
        else {
            var response = async(CommonPool) { return@async ideasUrl.httpGet().responseString() }.await()

            if (response.third.component1() == null)
                this.ideas.value = mutableListOf()
            else {
                var ideas = Gson().fromJson<List<Category>>(response.third.component1()!!)
                Paper.book().write("ideas", ideas)
                this.ideas.value = ideas
            }
        }
    }

    fun invalidateCache() {
        launch(UI) {
            var response = async(CommonPool) { return@async ideasUrl.httpGet().responseString() }.await()
            cacheResponse(response)
        }
    }

    private fun cacheResponse(response: Triple<Request, Response, Result<String, FuelError>>) {
        if (response.third.component1() != null){
            var ideas = Gson().fromJson<List<Category>>(response.third.component1()!!)
            Paper.book().write("ideas", ideas)
            this.ideas.value = ideas
        }
    }
}
