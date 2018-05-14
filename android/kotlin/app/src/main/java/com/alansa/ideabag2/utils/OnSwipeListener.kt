package com.alansa.ideabag2.utils

import android.content.Context
import android.view.GestureDetector
import android.view.MotionEvent
import android.view.View

enum class Swipe {
    LEFT,
    RIGHT
}

class OnSwipeListener(ctx: Context, val action: (Swipe) -> Unit) : View.OnTouchListener {
    private val detector = GestureDetector(ctx, GestureListener({ swipeReceived(it) }))
    override fun onTouch(p0: View?, p1: MotionEvent?): Boolean {
        return detector.onTouchEvent(p1)
    }

    fun swipeReceived(swipe: Swipe) {
        action(swipe)
    }

    class GestureListener(val action: (Swipe) -> Unit) : GestureDetector.SimpleOnGestureListener() {
        val SWIPE_THRESHOLD = 150
        val SWIPE_VELOCITY_THRESHOLD = 80

        override fun onDown(e: MotionEvent?): Boolean = true

        override fun onFling(e1: MotionEvent?, e2: MotionEvent?, velocityX: Float, velocityY: Float): Boolean {
            var result = false
            try {
                val diffY = e2!!.y - e1!!.y

                // Get horizontal swipe distance.
                val diffX = e2.x - e1.x

                // calculating swipe for x-axis.
                if (Math.abs(diffX) > Math.abs(diffY) && Math.abs(diffX) > SWIPE_THRESHOLD && Math.abs(velocityX) > SWIPE_VELOCITY_THRESHOLD) {
                    if (diffX > 0)
                        swipeRight()
                    else
                        swipeLeft()
                    result = true
                }
            } catch (e: Exception) {
            }

            return result
        }

        private fun swipeLeft() = action(Swipe.LEFT)


        private fun swipeRight() = action(Swipe.RIGHT)
    }
}