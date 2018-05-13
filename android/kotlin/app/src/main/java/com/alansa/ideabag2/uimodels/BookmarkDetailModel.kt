package com.alansa.ideabag2.uimodels

import com.alansa.ideabag2.models.Bookmark
import com.alansa.ideabag2.models.Category
import com.alansa.ideabag2.models.Note
import io.paperdb.Paper

class BookmarkDetailModel {
    val bookmarks = Paper.book().read<MutableList<Bookmark>>("bookmarks", mutableListOf())
    var bookmark = Bookmark(0, 0)
    val idea: Category.Item
        get() = Paper.book().read<List<Category>>("ideas", listOf())[bookmark.categoryId].items[bookmark.ideaId - 1]

    fun setBookmark(category: String, ideaId: Int) {
        var categories = Paper.book().read<List<Category>>("ideas", listOf())
        var category = categories.find { it.categoryLbl == category }
        var categoryId = categories.indexOf(category)

        bookmark = bookmarks.find { it.categoryId == categoryId && it.ideaId == ideaId }!!
    }

    fun getNote(): Note? {
        var notes = Paper.book().read<MutableList<Note>>("notes", mutableListOf())
        return notes.find { it.categoryId == bookmark.categoryId && it.ideaId == bookmark.ideaId }
    }

    fun saveNote(note: Note) {
        var notes = Paper.book().read<MutableList<Note>>("notes", mutableListOf())
        var existingNote = notes.find { it.categoryId == bookmark.categoryId && it.ideaId == bookmark.ideaId }

        if (existingNote == null)
            notes.add(note)
        else
            notes[notes.indexOf(existingNote)] = note

        Paper.book().write("notes", notes)
    }

    fun deleteNote() {
        var notes = Paper.book().read<MutableList<Note>>("notes", mutableListOf())
        var existingNote = notes.find { it.categoryId == bookmark.categoryId && it.ideaId == bookmark.ideaId }
        notes.remove(existingNote)
        Paper.book().write("notes", notes)
    }

    fun isBookmarked(): Boolean = bookmarks.any { it.categoryId == bookmark.categoryId && it.ideaId == bookmark.ideaId }

    fun removeBookmark() {
        var existingBookmark = bookmarks.find { it.categoryId == bookmark.categoryId && it.ideaId == bookmark.ideaId }
        bookmarks.remove(existingBookmark)
        Paper.book().write("bookmarks", bookmarks)
    }

    fun addBookmark() {
        bookmarks.add(Bookmark(bookmark.categoryId, bookmark.ideaId))
        Paper.book().write("bookmarks", bookmarks)
    }
}