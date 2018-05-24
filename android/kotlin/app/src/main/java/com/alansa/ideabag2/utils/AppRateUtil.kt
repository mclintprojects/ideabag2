package com.alansa.ideabag2.utils

import android.content.Context
import android.content.Intent
import android.net.Uri
import android.support.v7.app.AlertDialog
import android.widget.Toast
import com.alansa.ideabag2.App
import com.alansa.ideabag2.R

class AppRateUtil {
    companion object {
        private val prefs = App.CurrentActivity.getPreferences(Context.MODE_PRIVATE)
        private val editor = prefs.edit()

        fun init() {
            val threshold = prefs.getInt("rateAppThreshold", 5)
            val currentThreshold = prefs.getInt("currentRateAppThreshold", 0)

            if (currentThreshold >= threshold) {
                showRateAppDialog()
                editor.putInt("currentRateAppThreshold", 0)
            } else
                editor.putInt("currentRateAppThreshold", currentThreshold + 1)

            editor.commit()
        }

        private fun showRateAppDialog() {
            AlertDialog.Builder(App.CurrentActivity)
                    .setTitle(App.CurrentActivity.getString(R.string.rate_ideabag))
                    .setMessage("If you enjoy using Programming Ideas 2, please help us by rating it. It only takes a few seconds!")
                    .setPositiveButton("Rate", { s, e -> attempted() })
                    .setNegativeButton("Never", { s, e -> dismissedNever() })
                    .setNeutralButton("Later", { s, e -> dismissedLater() })
                    .create().show();
        }

        private fun dismissedLater() {
            val threshold = prefs.getInt("rateAppThreshold", 5)
            editor.putInt("rateAppThreshold", threshold * 2)
            editor.commit()
        }

        private fun dismissedNever() {
            val threshold = prefs.getInt("rateAppThreshold", 5)
            editor.putInt("rateAppThreshold", threshold * 8)
            editor.commit()
        }

        private fun attempted() {
            try
            {
                App.CurrentActivity.startActivity(Intent(Intent.ACTION_VIEW, Uri.parse("market://details?id=${App.CurrentActivity.packageName}")))
            }
            catch(ex: Exception)
            {
                Toast.makeText(App.CurrentActivity, "You do not have the Google Play Store installed.", Toast.LENGTH_LONG).show()
            }

            val threshold = prefs.getInt("rateAppThreshold", 5)
            editor.putInt("rateAppThreshold", threshold * 4)
            editor.commit()
        }
    }
}