package com.alansa.ideabag2.models

import com.google.gson.annotations.SerializedName

data class Category(
  @SerializedName("categoryLbl") val categoryLbl: String,
  @SerializedName("categoryCount") val categoryCount: Int,
  @SerializedName("description") val description: String,
  @SerializedName("items") val items: List<Item>
) {
  data class Item(
    @SerializedName("category") val category: String,
    @SerializedName("title") val title: String,
    @SerializedName("difficulty") val difficulty: String,
    @SerializedName("id") val id: Int,
    @SerializedName("description") val description: String
  )
}