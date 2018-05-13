package com.alansa.ideabag2

import com.alansa.ideabag2.models.Category

public class Global{
    companion object {
        val isLoggedIn : Boolean
            get() = authData != null

        var categories = mutableListOf<Category>()

        var categoryClickIndex = -1

        var ideaClickIndex = -1

        var bookmarkClickIndex = -1

        var authData : String? = ""
    }
}