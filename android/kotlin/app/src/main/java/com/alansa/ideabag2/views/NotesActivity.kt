package com.alansa.ideabag2.views

import android.app.AlertDialog
import android.arch.lifecycle.Observer
import android.arch.lifecycle.ViewModelProviders
import android.content.Intent
import android.databinding.DataBindingUtil
import android.os.Bundle
import android.support.v7.widget.DefaultItemAnimator
import android.support.v7.widget.LinearLayoutManager
import com.alansa.ideabag2.BaseActivity
import com.alansa.ideabag2.R
import com.alansa.ideabag2.adapters.NoteAction
import com.alansa.ideabag2.adapters.NotesAdapter
import com.alansa.ideabag2.databinding.ActivityNotesBinding
import com.alansa.ideabag2.models.Note
import com.alansa.ideabag2.viewmodels.NotesViewModel
import kotlinx.android.synthetic.main.activity_notes.*

class NotesActivity : BaseActivity() {
    lateinit var binding: ActivityNotesBinding
    lateinit var viewmodel: NotesViewModel
    lateinit var adapter: NotesAdapter
    var lastClickPos = -1
    val notes = mutableListOf<Note>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_notes)
        viewmodel = ViewModelProviders.of(this).get(NotesViewModel::class.java)
        binding.viewmodel = viewmodel

        setupToolbar()
        setupList()

        viewmodel.observeNotes(this)
        viewmodel.notes.observe(this, Observer {
            notes.clear()
            notes.addAll(it!!)
            adapter.notifyDataSetChanged()
        })
    }

    override fun onResume() {
        super.onResume()
        viewmodel.refreshNotes()
        adapter.notifyItemChanged(lastClickPos)
    }

    private fun setupList() {
        adapter = NotesAdapter(notes, { action, pos -> onNoteAction(action, pos) })
        notesRecyclerView.layoutManager = LinearLayoutManager(this)
        notesRecyclerView.adapter = adapter
        notesRecyclerView.itemAnimator = DefaultItemAnimator()
    }

    private fun onNoteAction(action: NoteAction, pos: Int) {
        lastClickPos = pos

        val note = notes[pos]
        when (action) {
            NoteAction.VIEW -> viewNote(note)
            NoteAction.EDIT -> editNote(note)
            NoteAction.DELETE -> deleteNote(note, pos)
        }
    }

    private fun deleteNote(note: Note, pos: Int) {
        viewmodel.deleteNote(note)
        adapter.notifyItemRemoved(pos)
    }

    private fun editNote(note: Note) {
        var intent = Intent(this, AddNoteActivity::class.java)
        intent.putExtra("ideaId", note.ideaId)
        intent.putExtra("categoryId", note.categoryId)
        startActivity(intent)
        overridePendingTransition(R.anim.push_left_in, R.anim.push_left_out)
    }

    private fun viewNote(note: Note) {
        AlertDialog.Builder(this)
                .setTitle(note.noteTitle)
                .setMessage(note.noteDetails)
                .setPositiveButton(R.string.dismiss, { _, _ -> })
                .create()
                .show()
    }
}