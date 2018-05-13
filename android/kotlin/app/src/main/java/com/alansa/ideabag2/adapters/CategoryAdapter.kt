package com.alansa.ideabag2.adapters

import android.graphics.Color
import android.support.v4.content.ContextCompat
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.ViewGroup
import android.widget.ImageView
import com.alansa.ideabag2.Global
import com.alansa.ideabag2.R
import com.alansa.ideabag2.databinding.RowCategoryBinding
import com.alansa.ideabag2.models.Category

class CategoryAdapter(private val categories: List<Category>, var itemClick: (Int) -> Unit) : RecyclerView.Adapter<CategoryViewHolder>() {
    private val icons = listOf(R.mipmap.numbers, R.mipmap.text, R.mipmap.network, R.mipmap.enterprise,
            R.mipmap.cpu, R.mipmap.web, R.mipmap.file, R.mipmap.database, R.mipmap.multimedia, R.mipmap.games)

    override fun getItemCount(): Int {
        return categories.size
    }

    override fun onBindViewHolder(holder: CategoryViewHolder, position: Int) {
        holder.bind(categories[position], itemClick, icons[position]);
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): CategoryViewHolder {
        var binding = RowCategoryBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return CategoryViewHolder(binding);
    }
}

class CategoryViewHolder(val binding: RowCategoryBinding) : RecyclerView.ViewHolder(binding.categoryRoot) {
    fun bind(category: Category, itemClick: (Int) -> Unit, categoryIcon: Int) {
        binding.category = category;
        binding.categoryRoot.setBackgroundColor(Color.TRANSPARENT)
        binding.categoryRoot.setOnClickListener { itemClick(adapterPosition) }
        binding.categoryRoot.findViewById<ImageView>(R.id.categoryIcon).setImageResource(categoryIcon)

        if (Global.categoryClickIndex == adapterPosition)
            binding.categoryRoot.setBackgroundColor(ContextCompat.getColor(binding.categoryRoot.context, R.color.highlight))
    }
}