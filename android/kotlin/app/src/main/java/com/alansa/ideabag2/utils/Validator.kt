package com.alansa.ideabag2.utils

import android.support.design.widget.TextInputEditText
import android.support.design.widget.TextInputLayout
import android.text.TextUtils
import com.alansa.ideabag2.models.IDisposable

class Validator() : IDisposable {
    private var alreadyFailed = false
    private var _passed = true
    val passedValidation: Boolean
        get() = _passed


    fun validateIsNotEmpty(inputLayout: TextInputLayout, text: String) {
        hideError(inputLayout)
        if (!alreadyFailed) {
            if (text.isNullOrEmpty() || text.isNullOrBlank()) {
                alreadyFailed = true
                _passed = false

                showError(inputLayout, "This is required.")
            } else _passed = true
        }
    }

    fun hideError(inputLayout: TextInputLayout){
        inputLayout.error = null
        inputLayout.isErrorEnabled = false
    }

    fun showError(inputLayout: TextInputLayout, error : String){
        inputLayout.error = error
        inputLayout.isErrorEnabled = true
    }

    override fun dispose() {
        alreadyFailed = false
    }
}