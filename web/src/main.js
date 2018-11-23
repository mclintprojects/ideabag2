import Vue from 'vue';
import App from './App.vue';
import VueRouter from 'vue-router';
import { routes } from './routes';
import { store } from './store/store';
import Toasted from 'vue-toasted';
import vmodal from 'vue-js-modal';
import Popper from 'vue-popperjs';

Vue.use(VueRouter);
Vue.use(Toasted);
Vue.use(vmodal);
Vue.use(Popper);

const router = new VueRouter({ routes, mode: 'history' });

new Vue({
  el: '#app',
  store,
  render: h => h(App),
  router
});
