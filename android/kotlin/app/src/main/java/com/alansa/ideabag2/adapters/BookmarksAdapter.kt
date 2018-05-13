package com.alansa.ideabag2.adapters

import android.graphics.Color
import android.support.v4.content.ContextCompat
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import com.alansa.ideabag2.Global
import com.alansa.ideabag2.R
import com.alansa.ideabag2.databinding.RowIdeaListBinding
import com.alansa.ideabag2.models.Bookmark
import com.alansa.ideabag2.models.Category
import com.alansa.ideabag2.models.CompletionStatus
import com.alansa.ideabag2.models.Status
import io.paperdb.Paper

class BookmarksAdapter(val ideas: List<Category.Item>, private val itemClick: (Int) -> Unit, private val longClick: (Int) -> Unit) : RecyclerView.Adapter<BookmarkViewHolder>() {
    private var statuses = Paper.book().read<List<Status>>("status", listOf())
    private var bookmarks = Paper.book().read<MutableList<Bookmark>>("bookmarks", mutableListOf())

    override fun getItemCount() = ideas.size

    override fun onBindViewHolder(holder: BookmarkViewHolder, position: Int) {
        val idea = ideas[position];
        holder.bind(idea, itemClick, longClick, getCompletionStatus(idea), isBookmarked(idea))
    }

    private fun isBookmarked(idea: Category.Item): Boolean = bookmarks.any { it.ideaId == idea.id }

    private fun getCompletionStatus(idea: Category.Item): CompletionStatus {
        val existingStatus = statuses.firstOrNull {
            it.ideaId == idea.id
        };

        if (existingStatus == null)
            return CompletionStatus.UNDECIDED

        return existingStatus.status
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): BookmarkViewHolder {
        val binding = RowIdeaListBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return BookmarkViewHolder(binding)
    }

    fun refresh() {
        bookmarks = Paper.book().read<MutableList<Bookmark>>("bookmarks", mutableListOf())
        notifyDataSetChanged()
    }

    fun notifyIdeaStatusChanged(position: Int) {
        statuses = Paper.book().read<List<Status>>("status", listOf())
        notifyItemChanged(position)
    }
}

class BookmarkViewHolder(binding: RowIdeaListBinding) : IdeaListViewHolder(binding) {
    override fun bind(idea: Category.Item, itemClick: (Int) -> Unit, longClick: (Int) -> Unit, status: CompletionStatus, bookmarked: Boolean) {
        binding.idea = idea
        binding.layoutRoot.setOnClickListener { itemClick(adapterPosition) }
        binding.layoutRoot.setOnLongClickListener {
            longClick(adapterPosition)
            return@setOnLongClickListener true
        }
        binding.layoutRoot.setBackgroundColor(Color.TRANSPARENT)
        setProgressState(status)

        if (Global.bookmarkClickIndex == adapterPosition)
            binding.layoutRoot.setBackgroundColor(ContextCompat.getColor(binding.layoutRoot.context, R.color.highlight))

        binding.bookmarkIndicator.visibility = if (bookmarked) View.VISIBLE else View.GONE
    }
}