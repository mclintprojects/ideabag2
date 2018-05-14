package com.alansa.ideabag2.uimodels

import android.arch.lifecycle.MutableLiveData
import com.alansa.ideabag2.extensions.empty
import com.alansa.ideabag2.models.Comment
import com.google.firebase.database.*

class CommentsModel {
    private val ref = FirebaseDatabase.getInstance().getReference()
    val comments = MutableLiveData<MutableList<Comment>>()
    val _comments = mutableListOf<Comment>()
    private var dataId = String.empty

    fun getComments(ideaId: Int, categoryId: Int, onComplete: (Int) -> Unit) {
        dataId = getDataId(ideaId, categoryId)

        ref.child("${dataId}/comments").addListenerForSingleValueEvent(object : ValueEventListener {
            override fun onDataChange(snapshot: DataSnapshot) {
                if (snapshot.value != null) {
                    for (pair in snapshot.value as (HashMap<String, HashMap<String, Any>>)) {
                        _comments.add(Comment(pair.key, pair.value["comment"] as String, pair.value["author"] as String, pair.value["created"] as Long))
                    }
                }

                comments.value = _comments
                onComplete(_comments.size)
            }

            override fun onCancelled(p0: DatabaseError?) {
            }
        })
    }

    private fun getDataId(ideaId: Int, categoryId: Int): String = "${categoryId}C-${ideaId}I"

    fun deleteComment(id: String, position: Int) {
        ref.child("${dataId}/comments/${id}").removeValue(DatabaseReference.CompletionListener { error, _ ->
            if(error?.message == null) {
                _comments.removeAt(position)
                comments.value = _comments
            }
        })
    }
}