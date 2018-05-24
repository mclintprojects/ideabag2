package com.alansa.ideabag2.dialogs

import android.support.v7.app.AlertDialog
import android.app.Dialog
import android.content.Context
import android.os.Bundle
import android.support.v4.app.DialogFragment
import android.view.LayoutInflater
import android.view.View
import android.widget.Toast
import com.alansa.ideabag2.R
import com.alansa.ideabag2.extensions.using
import com.alansa.ideabag2.utils.Validator
import com.google.firebase.auth.FirebaseAuth
import kotlinx.android.synthetic.main.dialog_register.view.*

class RegisterDialog(private val ctx : Context, private val onRegisterSuccess: () -> Unit) : DialogFragment() {
    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        var view = LayoutInflater.from(activity).inflate(R.layout.dialog_register, null)

        view.registerBtn.setOnClickListener {

            val validator = Validator()
            using(validator) {
                val email = view.emailTb.text.toString()
                val pwd = view.passwordTb.text.toString()
                validator.validateIsNotEmpty(view.emailTIL, email)
                validator.validateIsNotEmpty(view.passwordTIL, pwd)

                if (validator.passedValidation && view.passwordTb.text.toString() == view.retypePasswordTb.text.toString()) {
                    view.loadingCircle.visibility = View.VISIBLE
                    view.registerBtn.isEnabled = false

                    FirebaseAuth
                            .getInstance()
                            .createUserWithEmailAndPassword(email, pwd)
                            .addOnCompleteListener {
                                view.loadingCircle.visibility = View.GONE
                                view.registerBtn.isEnabled = true

                                Toast.makeText(ctx, if (it.isSuccessful) "Sign up successful." else it.exception?.message, Toast.LENGTH_LONG).show()

                                if (it.isSuccessful) {
                                    onRegisterSuccess()
                                    dismiss()
                                }
                            }
                } else {
                    Toast.makeText(ctx, R.string.required, Toast.LENGTH_LONG).show()
                }
            }
        }

        return AlertDialog.Builder(ctx)
                .setTitle(R.string.register)
                .setView(view)
                .create()
    }
}