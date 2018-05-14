package com.alansa.ideabag2

import com.alansa.ideabag2.models.Category
import com.google.firebase.auth.FirebaseAuth

public class Global {
    companion object {
        val isLoggedIn: Boolean
            get() = FirebaseAuth.getInstance().currentUser != null

        var categories = mutableListOf<Category>()

        var categoryClickIndex = -1

        var ideaClickIndex = -1

        var bookmarkClickIndex = -1

        var authData: String? = ""
    }
}