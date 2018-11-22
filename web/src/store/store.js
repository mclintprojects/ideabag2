import Vue from 'vue';
import Vuex from 'vuex';
import auth from './auth';
import app from './app';

Vue.use(Vuex);

export const store = new Vuex.Store({
  modules: {
    auth,
    app
  }
});
