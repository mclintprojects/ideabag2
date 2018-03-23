import axios from 'axios';
import config from './config';

const instance = axios.create({
	baseURL: sonfig.authBaseURL
});

export default instance;
