import Vue from 'vue';
import App from './App.vue';
import VueResource from 'vue-resource';
import VueRouter from 'vue-router';
import { routes } from './routes';
import { store } from './store/store';
import Toasted from 'vue-toasted';
Vue.use(VueResource);
Vue.use(VueRouter);
Vue.use(Toasted);

const router = new VueRouter({ routes, mode: 'history' });

new Vue({
	el: '#app',
	store,
	render: h => h(App),
	router
});
