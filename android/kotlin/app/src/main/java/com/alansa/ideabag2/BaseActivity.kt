package com.alansa.ideabag2

import android.support.v7.app.AppCompatActivity
import android.support.v7.widget.Toolbar
import android.view.MenuItem

open class BaseActivity : AppCompatActivity() {
    lateinit var toolbar : Toolbar
    open var setHomeAsUpEnabled = true

    fun setupToolbar(){
        toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(toolbar)
        supportActionBar?.setDisplayHomeAsUpEnabled(setHomeAsUpEnabled)
    }

    override fun onOptionsItemSelected(item: MenuItem?): Boolean {
        when(item?.itemId)
        {
            android.R.id.home -> navigateAway()
        }

        return true;
    }

    open fun navigateAway(){
        finish()
        overridePendingTransition(R.anim.push_right_in, R.anim.push_right_out)
    }

    override fun onBackPressed() {
        navigateAway()
    }
}