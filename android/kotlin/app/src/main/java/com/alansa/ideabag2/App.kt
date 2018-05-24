package com.alansa.ideabag2

import android.app.Activity
import android.app.Application
import android.os.Bundle
import android.support.v7.app.AppCompatDelegate
import com.alansa.ideabag2.utils.AppRateUtil

class App : Application(), Application.ActivityLifecycleCallbacks {
    companion object {
        private lateinit var _activity : Activity
        public val CurrentActivity : Activity
            get() = _activity
    }

    override fun onCreate() {
        super.onCreate()
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true);
        registerActivityLifecycleCallbacks(this)
    }

    override fun onActivityPaused(p0: Activity?) {
    }

    override fun onActivityResumed(p0: Activity?) {
        _activity = p0!!
    }

    override fun onActivityStarted(p0: Activity?) {
    }

    override fun onActivityDestroyed(p0: Activity?) {
    }

    override fun onActivitySaveInstanceState(p0: Activity?, p1: Bundle?) {
    }

    override fun onActivityStopped(p0: Activity?) {
    }

    override fun onActivityCreated(p0: Activity?, p1: Bundle?) {
        _activity = p0!!
    }
}