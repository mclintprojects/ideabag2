import instance from '../axios-auth';
import eventbus from '../eventbus';

const state = {
	userId: '',
	userEmail: '',
	token: '',
	isLoggingIn: false,
	userLoggedIn: false
};

const getters = {
	isLoggingIn: state => {
		return state.isLoggingIn;
	},
	userEmail: state => {
		return state.userEmail;
	},
	userLoggedIn: state => {
		return state.userLoggedIn;
	}
};

const mutations = {
	LOGIN_USER(state, loginData) {
		state.userId = loginData.idToken;
		state.userEmail = loginData.email;
		state.userLoggedIn = true;

		localStorage.setItem('loginData', loginData);
	},
	IS_LOGGING_IN(state, isLoggingIn) {
		state.isLoggingIn = isLoggingIn;
	},
	LOGOUT(state) {
		state.userLoggedIn = false;
		state.userEmail = '';
		state.token = '';

		localStorage.removeItem('loginData');
	}
};

const actions = {
	signUpUser(context, data) {
		data.returnSecureToken = true;

		console.log(data);
		instance
			.post('/signupNewUser?key=AIzaSyCzIvIOojv9gOcQrLqFvRSd9naA-gzm6ck', data)
			.then(res => {
				console.log(res);
			})
			.catch(error => {
				console.log(error.response.error);
			});
	},
	loginUserLocal(context, loginData) {
		context.commit('LOGIN_USER', loginData);
	},
	loginUser(context, loginData) {
		context.dispatch('isLoggingIn', true);
		loginData.returnSecureToken = true;

		console.log(loginData);
		instance
			.post(
				'/verifyPassword?key=AIzaSyCzIvIOojv9gOcQrLqFvRSd9naA-gzm6ck',
				loginData
			)
			.then(res => {
				console.log(res);
				context.commit('LOGIN_USER', res.data);

				eventbus.$emit('login-success', 'Login successful');
				context.dispatch('isLoggingIn', false);
			})
			.catch(error => {
				eventbus.$emit('login-failure', error.response.data.error.message);
				context.dispatch('isLoggingIn', false);
			});
	},
	isLoggingIn(context, isLoggingIn) {
		context.commit('IS_LOGGING_IN', isLoggingIn);
	},
	logout(context) {
		context.commit('LOGOUT');
	}
};

export default {
	state,
	actions,
	mutations,
	getters
};
