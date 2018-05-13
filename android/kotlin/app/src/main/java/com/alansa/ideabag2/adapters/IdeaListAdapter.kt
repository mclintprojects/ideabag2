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

class IdeaListAdapter(private val categoryId: Int, val ideas: List<Category.Item>, private val itemClick: (Int) -> Unit, private val longClick: (Int) -> Unit) : RecyclerView.Adapter<IdeaListViewHolder>() {
    private var statuses = Paper.book().read<List<Status>>("status", listOf()).filter { it.categoryId == categoryId }
    private var bookmarks = Paper.book().read<MutableList<Bookmark>>("bookmarks", mutableListOf()).filter { it.categoryId == categoryId }

    override fun getItemCount() = ideas.size

    override fun onBindViewHolder(holder: IdeaListViewHolder, position: Int) {
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

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): IdeaListViewHolder {
        val binding = RowIdeaListBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return IdeaListViewHolder(binding)
    }

    fun refresh() {
        bookmarks = Paper.book().read<MutableList<Bookmark>>("bookmarks", mutableListOf()).filter { it.categoryId == categoryId }
        notifyDataSetChanged()
    }

    fun notifyIdeaStatusChanged(position: Int) {
        statuses = Paper.book().read<List<Status>>("status", listOf()).filter { it.categoryId == categoryId }
        notifyItemChanged(position)
    }
}

open class IdeaListViewHolder(val binding: RowIdeaListBinding) : RecyclerView.ViewHolder(binding.layoutRoot) {
    open fun bind(idea: Category.Item, itemClick: (Int) -> Unit, longClick: (Int) -> Unit, status: CompletionStatus, bookmarked: Boolean) {
        binding.idea = idea
        binding.layoutRoot.setOnClickListener { itemClick(adapterPosition) }
        binding.layoutRoot.setOnLongClickListener {
            longClick(adapterPosition)
            return@setOnLongClickListener true
        }
        binding.layoutRoot.setBackgroundColor(Color.TRANSPARENT)
        setProgressState(status)

        if (Global.ideaClickIndex == adapterPosition)
            binding.layoutRoot.setBackgroundColor(ContextCompat.getColor(binding.layoutRoot.context, R.color.highlight))

        binding.bookmarkIndicator.visibility = if (bookmarked) View.VISIBLE else View.GONE
    }

    protected fun setProgressState(completionStatus: CompletionStatus) {
        val context = binding.layoutRoot.context
        when (completionStatus) {
            CompletionStatus.UNDECIDED -> binding.progressState.setBackgroundColor(ContextCompat.getColor(context, R.color.undecidedColor))
            CompletionStatus.IN_PROGRESS -> binding.progressState.setBackgroundColor(ContextCompat.getColor(context, R.color.inProgressColor))
            CompletionStatus.DONE -> binding.progressState.setBackgroundColor(ContextCompat.getColor(context, R.color.doneColor))
        }
    }
}