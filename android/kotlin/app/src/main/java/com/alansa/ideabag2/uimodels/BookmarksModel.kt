package com.alansa.ideabag2.uimodels

import com.alansa.ideabag2.models.Bookmark
import com.alansa.ideabag2.models.Category
import com.alansa.ideabag2.models.CompletionStatus
import com.alansa.ideabag2.models.Status
import io.paperdb.Paper

class BookmarksModel {
    private var bookmarks = Paper.book().read<MutableList<Bookmark>>("bookmarks", mutableListOf())

     fun getIdeas(): MutableList<Category.Item> {
        var ideas = mutableListOf<Category.Item>()
        var allIdeas = Paper.book().read<MutableList<Category>>("ideas")

        allIdeas.forEachIndexed { index, category -> ideas.addAll(category.items.filter { bookmarkContains(it, index) }) }
        return ideas
    }

    private fun bookmarkContains(idea: Category.Item, categoryId: Int): Boolean = bookmarks.any { it.ideaId == idea.id && it.categoryId == categoryId }

    fun getCompletedCount() : Int{
        var count = 0
        val statuses = Paper.book().read<List<Status>>("status", listOf())
        bookmarks.forEach {bookmark ->
            if(statuses.any { it.categoryId == bookmark.categoryId && it.ideaId == bookmark.ideaId && it.status == CompletionStatus.DONE }) count++  }

        return count
    }
}