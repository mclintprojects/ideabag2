package com.alansa.ideabag2.viewmodels

import android.arch.lifecycle.ViewModel
import android.databinding.ObservableField
import com.alansa.ideabag2.models.Category
import com.alansa.ideabag2.models.Note
import com.alansa.ideabag2.uimodels.BookmarkDetailModel

class BookmarkDetailViewModel : ViewModel() {
    private val model = BookmarkDetailModel()
    val idea = ObservableField<Category.Item>()
    var note = ObservableField<Note?>()
    val categoryId: Int
        get() = model.bookmark.categoryId
    val ideaId: Int
        get() = model.bookmark.ideaId
    val isBookmarked: Boolean
        get() = model.isBookmarked()

    fun showIdea() = idea.set(model.idea)

    fun saveNote(note: Note) {
        this.note.set(note)
        model.saveNote(note)
    }

    fun deleteNote() {
        note.set(null)
        model.deleteNote()
    }

    fun removeBookmark() = model.removeBookmark()

    fun addBookmark() = model.addBookmark()

    fun setNote() {
        note.set(model.getNote())
    }

    fun refreshNote() {
        note.set(model.getNote())
    }

    fun setBookmark(category: String, ideaId: Int) = model.setBookmark(category, ideaId)

}