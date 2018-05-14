package com.alansa.ideabag2.services

import android.app.NotificationManager
import android.app.PendingIntent
import android.content.Context
import android.content.Intent
import android.media.RingtoneManager
import android.support.v4.app.NotificationCompat
import com.alansa.ideabag2.R
import com.alansa.ideabag2.views.CategoryActivity
import com.google.firebase.messaging.FirebaseMessagingService
import com.google.firebase.messaging.RemoteMessage


class FCMService : FirebaseMessagingService() {
    val challengeCode = 1957
    override fun onMessageReceived(remoteMessage: RemoteMessage?) {
        var notif = remoteMessage?.notification
        if (notif != null)
            showNotification(notif)
    }

    private fun showNotification(notif: RemoteMessage.Notification) {
        val i = Intent(this, CategoryActivity::class.java)
        val pendingIntent = PendingIntent.getActivity(this, challengeCode,
                i, PendingIntent.FLAG_UPDATE_CURRENT)
        val defaultSoundUri = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION)

        val notification = NotificationCompat.Builder(this)
                .setContentText(notif.body)
                .setContentTitle(notif.title)
                .setContentIntent(pendingIntent)
                .setSmallIcon(R.mipmap.notif_icon)
                .setSound(defaultSoundUri)
                .build()

        val manager = getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager
        manager.notify(challengeCode, notification)
    }
}
