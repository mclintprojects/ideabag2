import axios from 'axios';

const instance = axios.create({
    baseUrl: 'https://www.googleapis.com/identitytoolkit/v3/relyingparty'
});

export default instance;