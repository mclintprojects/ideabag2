package com.alansa.ideabag2.uimodels

import com.alansa.ideabag2.models.Note
import io.paperdb.Paper

class AddNoteModel {


    fun saveNote(note: Note) {
        val notes = Paper.book().read<MutableList<Note>>("notes", mutableListOf())
        notes.add(note)
        Paper.book().write("notes", notes)
    }

    fun updateNote(note: Note) {
        val notes = Paper.book().read<MutableList<Note>>("notes", mutableListOf())
        val existingNote = notes.find { it.categoryId == note.categoryId && it.ideaId == note.ideaId }
        notes[notes.indexOf(existingNote)] = note
        Paper.book().write("notes", notes)
    }

    fun isInEditMode(ideaId: Int, categoryId: Int): Boolean = Paper.book().read<MutableList<Note>>("notes", mutableListOf()).any { it.categoryId == categoryId && it.ideaId == ideaId }

    fun getNote(ideaId: Int, categoryId: Int): Note {
        return Paper.book().read<MutableList<Note>>("notes", mutableListOf()).find { it.categoryId == categoryId && it.ideaId == ideaId }!!
    }

    fun deleteNote(ideaId: Int, categoryId: Int) {
        val notes = Paper.book().read<MutableList<Note>>("notes", mutableListOf())
        val existingNote = notes.find { it.categoryId == categoryId && it.ideaId == ideaId }
        notes.remove(existingNote)
        Paper.book().write("notes", notes)
    }
}