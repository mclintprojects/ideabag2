import instance from '../axios-auth';

const state = {
    userId: '',
    token: ''
};

const mutations = {
    LOGIN_USER(state, loginData) {
        
    }
}

const actions = {
    loginUser(context, loginData)
    {
        loginData.returnSecureToken = true;

        instance.post('signupNewUser?key=AIzaSyCzIvIOojv9gOcQrLqFvRSd9naA-gzm6ck', loginData)
        .then(res => {
            console.log(res);
        })
        .catch(error => {

        });
    }
};

export default {
    state,
    actions
}