package com.alansa.ideabag2.adapters

import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import com.alansa.ideabag2.databinding.RowCommentBinding
import com.alansa.ideabag2.models.Comment

class CommentsAdapter(private val comments: List<Comment>, private val loggedInUserEmail: String?, private val onDeleteCommentClick: (Int) -> Unit) : RecyclerView.Adapter<CommentViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): CommentViewHolder {
        val binding = RowCommentBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return CommentViewHolder(binding, onDeleteCommentClick)
    }

    override fun getItemCount(): Int = comments.size

    override fun onBindViewHolder(holder: CommentViewHolder, position: Int) {
        holder.bind(comments[position], loggedInUserEmail)
    }
}

class CommentViewHolder(private val binding: RowCommentBinding, private val onDeleteCommentClick: (Int) -> Unit) : RecyclerView.ViewHolder(binding.layoutRoot) {
    fun bind(comment: Comment, loggedInUserEmail: String?) {
        binding.comment = comment
        binding.deleteBtn.visibility = if (comment.author == loggedInUserEmail) View.VISIBLE else View.GONE
        binding.deleteBtn.setOnClickListener { onDeleteCommentClick(adapterPosition) }
    }
}