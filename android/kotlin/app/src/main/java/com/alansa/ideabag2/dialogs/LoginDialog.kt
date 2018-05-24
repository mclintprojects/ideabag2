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
import kotlinx.android.synthetic.main.dialog_login.view.*

class LoginDialog(private val ctx : Context, private val onLoginSuccess: () -> Unit) : DialogFragment() {
    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        var view = LayoutInflater.from(activity).inflate(R.layout.dialog_login, null)

        view.loginBtn.setOnClickListener {

            val validator = Validator()
            using(validator) {
                val email = view.emailTb.text.toString()
                val pwd = view.passwordTb.text.toString()
                validator.validateIsNotEmpty(view.emailTIL, email)
                validator.validateIsNotEmpty(view.passwordTIL, pwd)

                if (validator.passedValidation) {
                    view.loadingCircle.visibility = View.VISIBLE
                    view.loginBtn.isEnabled = false

                    FirebaseAuth
                            .getInstance()
                            .signInWithEmailAndPassword(email, pwd)
                            .addOnCompleteListener {
                                view.loadingCircle.visibility = View.GONE
                                view.loginBtn.isEnabled = true

                                Toast.makeText(ctx, if (it.isSuccessful) "Login successful." else it.exception?.message, Toast.LENGTH_LONG).show()

                                if (it.isSuccessful) {
                                    onLoginSuccess()
                                    dismiss()
                                }
                            }
                } else {
                    Toast.makeText(ctx, R.string.required, Toast.LENGTH_LONG).show()
                }
            }
        }

        return AlertDialog.Builder(ctx)
                .setTitle(R.string.login)
                .setView(view)
                .create()
    }
}