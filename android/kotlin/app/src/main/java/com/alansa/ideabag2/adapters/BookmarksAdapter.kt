package com.alansa.ideabag2.adapters

import com.alansa.ideabag2.models.Category
import io.paperdb.Paper

class BookmarksAdapter : IdeaListAdapter {
    constructor(ideas: MutableList<Category.Item>, itemClick: (Int) -> Unit) : super(0, ideas, itemClick) {
        bookmarks = Paper.book().read("bookmarks", mutableListOf())
        statuses = Paper.book().read("status", listOf())
    }

    override fun refresh() {
        bookmarks = Paper.book().read("bookmarks", mutableListOf())
        notifyDataSetChanged()
    }
}