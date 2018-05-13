package com.alansa.ideabag2.uimodels

import com.alansa.ideabag2.Global
import com.alansa.ideabag2.models.Bookmark
import com.alansa.ideabag2.models.Note
import io.paperdb.Paper

class IdeaDetailModel {
    val idea = Global.categories[Global.categoryClickIndex].items[Global.ideaClickIndex]
    var bookmarks = Paper.book().read<MutableList<Bookmark>>("bookmarks", mutableListOf())

    fun getNote(): Note? {
        var notes = Paper.book().read<MutableList<Note>>("notes", mutableListOf())
        return notes.find { it.categoryId == Global.categoryClickIndex && it.ideaId == idea.id }
    }

    fun saveNote(note: Note) {
        var notes = Paper.book().read<MutableList<Note>>("notes", mutableListOf())
        var existingNote = notes.find { it.categoryId == Global.categoryClickIndex && it.ideaId == idea.id }

        if (existingNote == null)
            notes.add(note)
        else
            notes[notes.indexOf(existingNote)] = note

        Paper.book().write("notes", notes)
    }

    fun deleteNote() {
        var notes = Paper.book().read<MutableList<Note>>("notes", mutableListOf())
        var existingNote = notes.find { it.categoryId == Global.categoryClickIndex && it.ideaId == idea.id }
        notes.remove(existingNote)
        Paper.book().write("notes", notes)
    }

    fun isBookmarked(): Boolean {
        var existingBookmark = bookmarks.find { it.categoryId == Global.categoryClickIndex && it.ideaId == Global.ideaClickIndex + 1 }
        return existingBookmark != null;
    }

    fun removeBookmark() {
        var existingBookmark = bookmarks.find { it.categoryId == Global.categoryClickIndex && it.ideaId == idea.id }
        bookmarks.remove(existingBookmark)
        Paper.book().write("bookmarks", bookmarks)
    }

    fun addBookmark(){
        bookmarks.add(Bookmark(Global.categoryClickIndex, idea.id))
        Paper.book().write("bookmarks", bookmarks)
    }
}