package com.alansa.ideabag2.uimodels

import com.alansa.ideabag2.Global
import com.alansa.ideabag2.models.Category
import com.alansa.ideabag2.models.CompletionStatus
import com.alansa.ideabag2.models.Status
import io.paperdb.Paper

class IdeaListModel {
    val ideas: List<Category.Item>
        get() = Global.categories[Global.categoryClickIndex].items

    val categoryId: Int
        get() = Global.categoryClickIndex

    fun getCompletedCount(): Int {
        return Paper.book().read<List<Status>>("status", listOf())
                .count { it.categoryId == categoryId && it.status == CompletionStatus.DONE }
    }

    fun setProgress(progress: CompletionStatus, ideaId : Int) {
        var statuses = Paper.book().read<MutableList<Status>>("status", mutableListOf())

        var existingStatus = statuses.find { it.categoryId == categoryId && it.ideaId == ideaId }
        if(existingStatus != null) existingStatus.status = progress else statuses.add(Status(categoryId, ideaId, progress))

        Paper.book().write("status", statuses)
    }
}