package com.alansa.ideabag2.dialogs

import android.app.AlertDialog
import android.app.Dialog
import android.os.Bundle
import android.support.v4.app.DialogFragment
import android.view.LayoutInflater
import com.alansa.ideabag2.R
import com.alansa.ideabag2.models.CompletionStatus
import kotlinx.android.synthetic.main.dialog_idea_progress.view.*

class SetProgressDialog(private val onProgressChanged: (CompletionStatus) -> Unit) : DialogFragment() {
    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        var view = LayoutInflater.from(activity).inflate(R.layout.dialog_idea_progress, null)

        view.doneRBtn.setOnCheckedChangeListener { _, _ -> notifyProgressChange(CompletionStatus.DONE) }
        view.undecidedRBtn.setOnCheckedChangeListener { _, _ -> notifyProgressChange(CompletionStatus.UNDECIDED) }
        view.inProgressRBtn.setOnCheckedChangeListener { _, _ -> notifyProgressChange(CompletionStatus.IN_PROGRESS) }

        return AlertDialog.Builder(activity)
                .setTitle("Set idea progress")
                .setView(view)
                .create()
    }

    fun notifyProgressChange(status: CompletionStatus) {
        onProgressChanged(status)
        dismiss()
    }

}