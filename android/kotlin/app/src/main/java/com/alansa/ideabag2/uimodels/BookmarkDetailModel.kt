package com.alansa.ideabag2.uimodels

import com.alansa.ideabag2.Global
import com.alansa.ideabag2.models.Bookmark
import com.alansa.ideabag2.models.Category
import com.alansa.ideabag2.models.Note
import io.paperdb.Paper

class BookmarkDetailModel {
    val bookmarks = Paper.book().read<MutableList<Bookmark>>("bookmarks", mutableListOf())
    val bookmark: Bookmark
        get() = bookmarks[Global.bookmarkClickIndex]
    val idea = Paper.book().read<List<Category>>("ideas")[bookmark.categoryId].items[bookmark.ideaId]

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