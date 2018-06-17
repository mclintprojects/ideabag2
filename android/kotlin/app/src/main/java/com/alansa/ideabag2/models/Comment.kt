package com.alansa.ideabag2.models

import java.text.DateFormat
import java.util.*

data class Comment(val id: String, val author: String, val comment: String, val created: Long) {
    fun getDateString() : String{
        return DateFormat.getDateInstance(DateFormat.SHORT).format(Date(created))
    }

    fun toMap() = hashMapOf("author" to author, "comment" to comment, "created" to created)
}