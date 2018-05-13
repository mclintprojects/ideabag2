package com.alansa.ideabag2.viewmodels

import android.arch.lifecycle.ViewModel
import android.databinding.ObservableField
import com.alansa.ideabag2.Global
import com.alansa.ideabag2.models.Note
import com.alansa.ideabag2.uimodels.AddNoteModel

class AddNoteViewModel : ViewModel() {
    val title = ObservableField<String>()
    val details = ObservableField<String>()
    var isInEditMode = ObservableField<Boolean>(false)

    private val model = AddNoteModel()

    private fun saveNote(ideaId: Int, categoryId : Int) = model.saveNote(Note(categoryId, ideaId, title.get()!!, details.get()!!))

    private fun updateNote(ideaId: Int, categoryId : Int) = model.updateNote(Note(categoryId, ideaId, title.get()!!, details.get()!!))

    fun saveOrUpdateNote(ideaId: Int, categoryId : Int) = if (isInEditMode.get()!!) updateNote(ideaId, categoryId) else saveNote(ideaId, categoryId)

    fun isInEditMode(ideaId: Int): Boolean {
        val isEditing = model.isInEditMode(ideaId)
        isInEditMode.set(isEditing)
        return isEditing
    }

    fun showExistingNote(ideaId: Int) {
        val note = model.getNote(ideaId)
        title.set(note.noteTitle)
        details.set(note.noteDetails)
    }

    fun deleteNote(ideaId: Int) = model.deleteNote(ideaId)
}