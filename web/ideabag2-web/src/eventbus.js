import Vue from 'vue';

const eventbus = new Vue({
	methods: {
		showToast(message, type = 'default', toastLength = 'short') {
			var duration = toastLength == 'short' ? 3000 : 5000;

			this.$toasted.show(message, {
				duration,
				position: 'bottom-center',
				singleton: true,
				type,
				theme: 'bubble'
			});
			console.log('showed toast');
		}
	}
});

export default eventbus;
