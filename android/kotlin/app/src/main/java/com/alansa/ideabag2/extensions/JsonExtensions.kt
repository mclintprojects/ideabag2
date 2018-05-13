package com.alansa.ideabag2.extensions

import com.google.gson.Gson
import com.google.gson.reflect.TypeToken

/**
 * Created by Mbah Clinton on 5/10/2018.
 */
inline fun <reified T: Any> typeToken() = object : TypeToken<T>() {}.type

inline fun <reified T: Any> Gson.fromJson(json: String): T = fromJson(json, typeToken<T>())