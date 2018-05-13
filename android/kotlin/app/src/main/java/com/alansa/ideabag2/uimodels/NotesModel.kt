package com.alansa.ideabag2.uimodels

import android.arch.lifecycle.MutableLiveData
import com.alansa.ideabag2.models.Note
import io.paperdb.Paper

class NotesModel {
    var notes = MutableLiveData<List<Note>>()
    var _notes = Paper.book().read<MutableList<Note>>("notes", mutableListOf())

    init{
        notes.value = _notes
    }

    fun deleteNote(note: Note) {
        _notes.remove(_notes.find { it.categoryId == note.categoryId && it.ideaId == note.ideaId })
        Paper.book().write("notes", _notes)

        notes.value = _notes
    }

    fun refreshNotes() {
        _notes = Paper.book().read("notes", mutableListOf())
        notes.value = _notes
    }
}