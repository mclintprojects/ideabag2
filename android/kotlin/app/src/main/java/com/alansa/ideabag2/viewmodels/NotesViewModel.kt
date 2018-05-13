package com.alansa.ideabag2.viewmodels

import android.arch.lifecycle.LifecycleOwner
import android.arch.lifecycle.MutableLiveData
import android.arch.lifecycle.Observer
import android.arch.lifecycle.ViewModel
import android.content.Context
import android.databinding.ObservableField
import com.alansa.ideabag2.models.Note
import com.alansa.ideabag2.uimodels.NotesModel

class NotesViewModel : ViewModel() {
    val model = NotesModel()
    val notes = MutableLiveData<List<Note>>()
    val showEmptyState = ObservableField(true)

    fun observeNotes(ctx : LifecycleOwner){
        var notes = model.notes.observe(ctx, Observer {
            this.notes.value = it
            showEmptyState.set(it?.size == 0)
        })
    }

    fun deleteNote(note: Note) {
        var notes = model.deleteNote(note)
    }

    fun refreshNotes() = model.refreshNotes()

}