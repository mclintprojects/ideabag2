package com.alansa.ideabag2.adapters

import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.ViewGroup
import com.alansa.ideabag2.databinding.RowNotesBinding
import com.alansa.ideabag2.models.Note

class NotesAdapter(private val notes: List<Note>, private val actionClick: (NoteAction, Int) -> Unit) : RecyclerView.Adapter<NotesViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): NotesViewHolder {
        var binding = RowNotesBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return NotesViewHolder(binding, actionClick)
    }

    override fun getItemCount(): Int = notes.size

    override fun onBindViewHolder(holder: NotesViewHolder, position: Int) {
        holder.bind(notes[position])
    }
}

class NotesViewHolder(private val binding: RowNotesBinding, private val actionClick: (NoteAction, Int) -> Unit) : RecyclerView.ViewHolder(binding.layoutRoot) {
    fun bind(note: Note) {
        binding.note = note

        binding.deleteNoteBtn.setOnClickListener { actionClick(NoteAction.DELETE, adapterPosition) }
        binding.editNoteBtn.setOnClickListener { actionClick(NoteAction.EDIT, adapterPosition) }
        binding.viewNoteBtn.setOnClickListener { actionClick(NoteAction.VIEW, adapterPosition) }
    }
}

enum class NoteAction {
    VIEW,
    EDIT,
    DELETE
}