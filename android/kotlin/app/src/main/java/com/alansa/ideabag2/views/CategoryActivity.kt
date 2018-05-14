package com.alansa.ideabag2.views

import android.arch.lifecycle.Observer
import android.arch.lifecycle.ViewModelProviders
import android.content.Intent
import android.databinding.DataBindingUtil
import android.os.Bundle
import android.support.design.widget.Snackbar
import android.support.v4.content.ContextCompat
import android.support.v7.widget.LinearLayoutManager
import android.text.Html
import android.view.Menu
import android.view.MenuItem
import com.alansa.ideabag2.BaseActivity
import com.alansa.ideabag2.Global
import com.alansa.ideabag2.R
import com.alansa.ideabag2.adapters.CategoryAdapter
import com.alansa.ideabag2.databinding.ActivityCategoryBinding
import com.alansa.ideabag2.dialogs.LoginDialog
import com.alansa.ideabag2.dialogs.RegisterDialog
import com.alansa.ideabag2.extensions.addIfNotExist
import com.alansa.ideabag2.extensions.empty
import com.alansa.ideabag2.extensions.tryRemoveItem
import com.alansa.ideabag2.models.Category
import com.alansa.ideabag2.viewmodels.CategoryViewModel
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.messaging.FirebaseMessaging
import io.paperdb.Paper
import kotlinx.android.synthetic.main.activity_category.*

class CategoryActivity : BaseActivity() {
    private lateinit var binding: ActivityCategoryBinding
    private lateinit var viewmodel: CategoryViewModel
    private lateinit var adapter: CategoryAdapter
    private var categories = mutableListOf<Category>()
    private var menu: Menu? = null
    private val logoutId = 6;
    private val loginId = 5;
    private val registerId = 4;

    override var setHomeAsUpEnabled: Boolean = false

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_category)
        viewmodel = ViewModelProviders.of(this).get(CategoryViewModel::class.java)
        binding.viewmodel = viewmodel;
        binding.executePendingBindings()

        setupToolbar()
        Paper.init(this)

        viewmodel.owner = this
        viewmodel.categories.observe(this, Observer {
            if (this.categories.size == 0) {
                this.categories.addAll(it!!)
                setupList()
                viewmodel.invalidateCache()
            } else {
                categories.clear()
                categories.addAll(it!!)
                adapter.notifyDataSetChanged()
            }

            Global.categories.clear()
            Global.categories.addAll(it)
        })

        bookmarkFab.setOnClickListener {
            startActivity(Intent(this, BookmarksActivity::class.java))
            overridePendingTransition(R.anim.push_left_in, R.anim.push_left_out)
        }

        FirebaseMessaging.getInstance().subscribeToTopic("new-ideas")
        FirebaseMessaging.getInstance().subscribeToTopic("challenges")
    }

    override fun onCreateOptionsMenu(menu: Menu?): Boolean {
        menuInflater.inflate(R.menu.home_menu, menu)
        this.menu = menu!!
        manageOptionalMenuItems()
        return super.onCreateOptionsMenu(menu)
    }

    override fun onResume() {
        super.onResume()

        if (menu != null)
            manageOptionalMenuItems();

        Global.ideaClickIndex = -1
        Global.bookmarkClickIndex = -1
    }

    override fun onDestroy() {
        FirebaseMessaging.getInstance().unsubscribeFromTopic("new-ideas")
        FirebaseMessaging.getInstance().unsubscribeFromTopic("challenges")
        super.onDestroy()
    }

    private fun manageOptionalMenuItems() {
        if (Global.isLoggedIn) {
            menu?.tryRemoveItem(loginId)
            menu?.tryRemoveItem(registerId)
            menu?.addIfNotExist(0, logoutId, logoutId, "Logout")

            toolbar.subtitle = Html.fromHtml("<font color='#ffffff'>${FirebaseAuth.getInstance().currentUser?.email}</font>")
        } else {
            menu?.tryRemoveItem(logoutId)
            val registerItem = menu?.addIfNotExist(0, registerId, registerId, "Register")
            registerItem?.setShowAsAction(MenuItem.SHOW_AS_ACTION_ALWAYS)
            registerItem?.icon = ContextCompat.getDrawable(this, R.drawable.ic_person_add_white_24px)

            val loginItem = menu?.addIfNotExist(0, loginId, loginId, "Login")
            loginItem?.setShowAsAction(MenuItem.SHOW_AS_ACTION_ALWAYS)
            loginItem?.icon = ContextCompat.getDrawable(this, R.drawable.ic_user_login_button)

            toolbar.subtitle = String.empty
        }
    }

    override fun onOptionsItemSelected(item: MenuItem?): Boolean {
        when (item!!.itemId) {
            loginId -> showLoginDialog()
            registerId -> showRegisterDialog()
            logoutId -> logoutUser()
            R.id.submitIdea -> startActivity(Intent(this, SubmitIdeaActivity::class.java))
            R.id.viewNotes -> startActivity(Intent(this, NotesActivity::class.java))

            else -> return super.onOptionsItemSelected(item)
        }

        return true
    }

    private fun logoutUser() {
        FirebaseAuth.getInstance().signOut()
        manageOptionalMenuItems()
    }

    private fun showRegisterDialog() {
        RegisterDialog() { manageOptionalMenuItems() }
                .show(supportFragmentManager, String.empty)
    }

    private fun showLoginDialog() {
        LoginDialog() { manageOptionalMenuItems() }
                .show(supportFragmentManager, String.empty)
    }

    private fun setupList() {
        if (categories.size == 0) {
            Snackbar.make(recyclerView, R.string.failed_to_download_ideas, Snackbar.LENGTH_INDEFINITE)
                    .setAction(R.string.retry, { viewmodel.retryDownload() })
                    .show()
        } else {
            adapter = CategoryAdapter(categories, { onItemClick(it) })
            recyclerView.layoutManager = LinearLayoutManager(this)
            recyclerView.adapter = adapter
        }
    }

    private fun onItemClick(position: Int): Unit {
        startActivity(Intent(this, IdeaListActivity::class.java))
        overridePendingTransition(R.anim.push_left_in, R.anim.push_left_out)

        val previousPos = Global.categoryClickIndex
        Global.categoryClickIndex = position

        adapter.notifyItemChanged(previousPos)
        adapter.notifyItemChanged(position)

    }
}
