package com.alansa.ideabag2.uimodels

import com.alansa.ideabag2.extensions.fromJson
import com.alansa.ideabag2.models.Comment
import com.github.kittinunf.fuel.httpGet
import com.google.firebase.auth.FirebaseAuth
import com.google.gson.Gson
import com.google.gson.JsonObject
import kotlinx.coroutines.experimental.CommonPool
import kotlinx.coroutines.experimental.async

class CommentsModel {
    private val token = FirebaseAuth.getInstance().currentUser?.getIdToken(true).toString()
    private val baseUrl = "https://ideabag2.firebaseio.com/ideabag2"

    suspend fun getComments(ideaId: Int, categoryId: Int) {
        val dataId = getDataId(ideaId, categoryId)

        val json = (async(CommonPool){"${baseUrl}/${dataId}/comments.jsoon".httpGet().responseString()}.await()).third.component1()
        if(json != null)
        {
            var dictionary = Gson().fromJson<JsonObject>(json)
            println(dictionary)
        }
    }

    private fun getDataId(ideaId: Int, categoryId: Int): String = "${categoryId}C-${ideaId}I"
}