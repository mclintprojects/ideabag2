package com.alansa.ideabag2.viewmodels

import android.arch.lifecycle.ViewModel
import android.content.Context
import android.content.Intent
import android.databinding.ObservableField
import android.net.Uri
import java.text.SimpleDateFormat
import java.util.*

class SubmitIdeaViewModel : ViewModel(){
    val authorName = ObservableField<String>()
    val ideaTitle = ObservableField<String>()
    val ideaCategory = ObservableField<String>()
    val ideaDetails = ObservableField<String>()

    fun submitIdea(context : Context){
        var submitIntent = Intent(Intent.ACTION_SEND)
        submitIntent.data = Uri.parse("mailto:")
        submitIntent.putExtra(Intent.EXTRA_EMAIL, arrayOf("alansagh@gmail.com"));
        submitIntent.putExtra(Intent.EXTRA_SUBJECT, "IdeaBag 2 Submission ${SimpleDateFormat("dd/M/yyyy hh:mm:ss").format(Date())}");

        var body = "Author: ${authorName.get()}\r\nCategory: ${ideaCategory.get()}\r\nTitle: ${ideaTitle.get()}\r\n\r\nDescription\r\n${ideaDetails.get()}";
        submitIntent.putExtra(Intent.EXTRA_TEXT, body);
        submitIntent.setType("message/rfc822");
        context.startActivity(Intent.createChooser(submitIntent, "Submit idea to developer via e-mail"));
    }
}