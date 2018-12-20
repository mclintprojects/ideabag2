import Vue from 'vue';
import App from './App.vue';
import VueRouter from 'vue-router';
import { routes } from './routes';
import { store } from './store/store';
import { library as fontAwesomeLibrary } from '@fortawesome/fontawesome-svg-core';
import {
  faSpinner,
  faBars,
  faBookmark,
  faEllipsisV,
  faArrowLeft,
  faTrash,
  faFilter
} from '@fortawesome/free-solid-svg-icons';
import { faBookmark as farBookmark } from '@fortawesome/free-regular-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import Toasted from 'vue-toasted';
import VModal from 'vue-js-modal';
import Popper from 'vue-popperjs';

fontAwesomeLibrary.add(
  faSpinner,
  faBars,
  faBookmark,
  farBookmark,
  faEllipsisV,
  faArrowLeft,
  faTrash,
  faFilter
);
Vue.component('font-awesome-icon', FontAwesomeIcon);

Vue.use(VueRouter);
Vue.use(Toasted);
Vue.use(VModal);
Vue.use(Popper);

const router = new VueRouter({ routes, mode: 'history' });

new Vue({
  el: '#app',
  store,
  render: h => h(App),
  router
});
