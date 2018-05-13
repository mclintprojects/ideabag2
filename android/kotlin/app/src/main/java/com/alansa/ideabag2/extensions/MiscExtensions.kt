package com.alansa.ideabag2.extensions

import android.animation.ObjectAnimator
import android.view.Menu
import android.view.MenuItem
import android.view.View
import android.view.animation.Interpolator
import com.alansa.ideabag2.models.IDisposable

fun Menu.tryRemoveItem(id: Int) {
    if (this.findItem(id) != null)
        this.removeItem(id)
}

fun Menu.addIfNotExist(groupId: Int = 0, itemId: Int, order: Int = 0, title: String = ""): MenuItem {
    var item = this.findItem(itemId)
    if (item == null)
        return this.add(groupId, itemId, order, title)

    return item;
}

fun View.animateView(property: String, duration: Long, interpolator: Interpolator, vararg positions: Float) {
    var animator = ObjectAnimator.ofFloat(this, property, *positions)
    animator.duration = duration
    animator.interpolator = interpolator
    animator.start()
}

inline fun Any.using(disposable: IDisposable, action: () -> Unit) {
    action()
    disposable.dispose()
}

val String.Companion.empty: String
    get() = ""