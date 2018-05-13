package com.alansa.ideabag2.viewmodels

import android.arch.lifecycle.ViewModel
import android.content.Context
import android.databinding.ObservableField
import com.alansa.ideabag2.Global
import com.alansa.ideabag2.models.Category
import com.alansa.ideabag2.models.Note
import com.alansa.ideabag2.uimodels.IdeaDetailModel

class IdeaDetailViewModel : ViewModel() {
    private val model = IdeaDetailModel()
    val idea = ObservableField<Category.Item>(model.idea)
    var note = ObservableField<Note?>()

    val isBookmarked: Boolean
    get() = model.isBookmarked()

    init{
        refreshNote()
    }

    fun saveNote(note : Note){
        this.note.set(note)
        model.saveNote(note)
    }

    fun deleteNote(){
        note.set(null)
        model.deleteNote()
    }

    fun removeBookmark() = model.removeBookmark()

    fun addBookmark() = model.addBookmark()

    fun refreshNote() {
        note.set(model.getNote())
    }

    fun swipedLeft() {
        val index = ++Global.ideaClickIndex
        if(index <= Global.categories[Global.categoryClickIndex].items.size - 1){
            idea.set(Global.categories[Global.categoryClickIndex].items[index])
        }else normalize(Global.categories[Global.categoryClickIndex].items.size - 1)
    }

    private fun normalize(index : Int) {
        Global.ideaClickIndex = index
    }

    fun swipedRight() {
        val index = --Global.ideaClickIndex
        if(index >= 0){
            idea.set(Global.categories[Global.categoryClickIndex].items[index])
        }else normalize(0)
    }
}