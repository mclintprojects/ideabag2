package com.alansa.ideabag2.models

class Status(val categoryId: Int, val ideaId: Int, var status: CompletionStatus = CompletionStatus.UNDECIDED) {
}

enum class CompletionStatus {
    UNDECIDED,
    IN_PROGRESS,
    DONE
}