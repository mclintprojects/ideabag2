package com.alansa.ideabag2

import android.support.test.InstrumentationRegistry
import com.alansa.ideabag2.views.AddNoteActivity
import org.robolectric.RobolectricTestRunner
import org.robolectric.Robolectric
import junit.framework.Assert
import org.junit.Assert.assertEquals
import org.junit.Test
import org.junit.runner.RunWith

/**
 * Instrumented test, which will execute on an Android device.
 *
 * See [testing documentation](http://d.android.com/tools/testing).
 */
@RunWith(RobolectricTestRunner::class)
class PaperTests {
    val ctx = InstrumentationRegistry.getTargetContext()
    @Test
    fun useAppContext() {
        // Context of the app under test.
        assertEquals("com.alansa.ideabag2", ctx.packageName)
    }

}
