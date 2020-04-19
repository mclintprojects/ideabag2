import Vuex from 'vuex';
import Vue from 'vue';

Vue.use(Vuex);

export default (getters, actions = {}) => {
  return new Vuex.Store({ getters, actions });
};
