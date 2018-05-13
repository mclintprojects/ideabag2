package com.alansa.ideabag2.views

import android.arch.lifecycle.ViewModelProviders
import android.databinding.DataBindingUtil
import android.os.Bundle
import android.view.Menu
import android.view.MenuItem
import android.widget.Toast
import com.alansa.ideabag2.BaseActivity
import com.alansa.ideabag2.R
import com.alansa.ideabag2.databinding.ActivityAddNoteBinding
import com.alansa.ideabag2.viewmodels.AddNoteViewModel
import kotlinx.android.synthetic.main.activity_add_note.*

class AddNoteActivity : BaseActivity() {
    private lateinit var binding: ActivityAddNoteBinding
    private lateinit var viewmodel: AddNoteViewModel
    private var ideaId = 0
    private var categoryId = 0

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_add_note)
        viewmodel = ViewModelProviders.of(this).get(AddNoteViewModel::class.java)
        binding.viewmodel = viewmodel

        setupToolbar()

        ideaId = intent.getIntExtra("ideaId", 0)
        categoryId = intent.getIntExtra("categoryId", 0)
        if (viewmodel.isInEditMode(ideaId, categoryId)) {
            supportActionBar?.title = "Update note"
            viewmodel.showExistingNote(ideaId, categoryId)
        } else supportActionBar?.title = "Create a note"

        saveBtn.setOnClickListener {
            if (noteTitle.text.isNullOrEmpty() || noteDetail.text.isNullOrEmpty())
                Toast.makeText(this, "One or required fields are empty", Toast.LENGTH_LONG).show()
            else
                saveOrUpdateNote()
        }
    }

    private fun saveOrUpdateNote() {
        viewmodel.saveOrUpdateNote(ideaId, categoryId)
        navigateAway()
    }

    override fun onCreateOptionsMenu(menu: Menu?): Boolean {
        menuInflater.inflate(R.menu.add_note_menu, menu)
        return super.onCreateOptionsMenu(menu)
    }

    override fun onOptionsItemSelected(item: MenuItem?): Boolean {
        when (item?.itemId) {
            R.id.deleteNote -> {
                viewmodel.deleteNote(ideaId, categoryId)
                navigateAway()
            }
        }
        return super.onOptionsItemSelected(item)
    }
}
