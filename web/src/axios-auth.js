import axios from 'axios';
import config from './config';

const instance = axios.create({
	baseURL: config.authBaseURL
});

export default instance;
