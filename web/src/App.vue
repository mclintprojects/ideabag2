<template>
	<div class="main-container">
		<navbar></navbar>
		<transition name="slide" mode="out-in">
			<keep-alive>
				<router-view></router-view>
			</keep-alive>
		</transition>
	</div>
</template>

<script>
import Navbar from './components/Navbar';
import axios from 'axios';
import eventbus from './eventbus';
import { create } from 'domain';

let ideasURL =
	'https://docs.google.com/document/d/17V3r4fJ2udoG5woDBW3IVqjxZdfsbZC04G1A-It_DRU/export?format=txt';

export default {
	name: 'app',
	components: { Navbar },
	data() {
		return {
			dbVersion: 1
		};
	},
	methods: {
		getData() {
			const ideasdb = localStorage.getItem('ideasdb');

			if (ideasdb) {
				this.$store.dispatch('setLoading', false);
				eventbus.showToast('Loaded offline cache.', 'info');
				return JSON.parse(ideasdb);
			}

			return [];
		},
		tryLocalLogin() {
			const loginData = localStorage.getItem('loginData');

			if (loginData) {
				this.$store.dispatch('loginUserLocal', JSON.parse(loginData));
			}
		},
		saveData(ideasdb) {
			localStorage.setItem('ideasdb', JSON.stringify(ideasdb));
		},
		setupInterceptors() {
			axios.interceptors.response.use(res => {
				if (res.status == 401) {
					eventbus.showToast(
						'Authorization token expired. Please login again.',
						'error',
						'5000'
					);
					this.$store.dispatch('logout');
				}

				return res;
			});
		},
		downloadIdeas() {
			axios.defaults.timeout = 12000;

			axios
				.get(ideasURL)
				.then(response => {
					this.$store.dispatch('setCategories', response.data);
					this.$store.dispatch('setLoading', false);
					this.saveData(response.data);
				})
				.catch(() => {
					eventbus.showToast(
						"Couldn't load data. Please check your connection and reload.",
						'error',
						'long'
					);
				});
		},
		createDb(recreateDb = false) {
			if (recreateDb) this.dbVersion = this.dbVersion + 1;

			const request = indexedDB.open('userData', 1);

			request.onupgradeneeded = event => {
				const db = event.target.result;
				const ideasStore = db.createObjectStore('ideas', { keyPath: 'id' });
				ideasStore.createIndex('bookmarked', 'bookmarked');
			};

			request.onsuccess = event => {
				const db = event.target.result;
				if (!db.objectStoreNames.contains('ideas')) this.createDb(true);
				this.$store.dispatch('setUserDataDB', db);
			};
		}
	},
	created() {
		this.$store.dispatch('setCategories', this.getData());
		this.downloadIdeas();
		this.tryLocalLogin();
		this.setupInterceptors();
		this.createDb();
	}
};
</script>

<style>
:root {
	--primary: #ffa000;
	--primaryDark: #c67100;
	--background: #37474f;
	--highlight: #2c393f;
	--undecided: #bababa;
	--in-progress: #f9bf3b;
	--done: #2ecc71;
	--primaryText: rgba(255, 255, 255, 0.8);
	--primaryTextLight: rgba(255, 255, 255, 0.54);
	--primaryTextSize: 18px;
	--ideaTextSize: 22px;
	--categoryIconSize: 36px;
	--categoryIconBgSize: 72px;
	--badgePadding: 8px;
	--ideaDescriptionTextSize: 16px;
	--badgeTextSize: 12px;
	--avatarSize: 30px;
	--commentPadding: 16px;
	--cardMargin: 16px 0px 0px 0px;
	--authorLblSize: 16px;
	--dateLblSize: 13px;
	--dateLblMargin: 32px;
}

html,
body {
	height: 100%;
}
body {
	background-color: var(--background);
	font-family: 'Roboto', sans-serif;
	overflow-x: hidden;
}

#loadingCircle {
	width: 36px;
	position: absolute;
	left: 50%;
	top: 50%;
}

.appBtn {
	background-color: var(--primary);
	border: solid 0px transparent;
	border-radius: 2px;
	width: 100px;
	height: 40px;
	transition: all 1s;
}

.appBtn:hover {
	background-color: var(--primaryDark);
	cursor: pointer;
}

.appBtn:disabled {
	background-color: gray;
	color: black;
}

.appBtn:disabled:hover {
	cursor: not-allowed;
}

.appBtnOutline {
	background-color: transparent;
	border: solid 2px white;
	border-radius: 2px;
	color: white;
	margin: 0.5rem;
	height: 40px;
}

.appBtnOutline:hover {
	background-color: white;
	color: var(--primary);
}

#backBtn {
	margin-left: 16px;
}

#backBtn:hover {
	cursor: pointer;
}

.main-container {
	display: flex;
	justify-content: center;
	height: 100%;
}

.primaryLbl {
	color: var(--primaryText);
}

.secondaryLbl {
	color: var(--primaryTextLight);
}

.col-xs-3,
.col-xs-6,
.row {
	margin: 0px;
	padding: 0px;
}

#componentHolder {
	margin-top: 70px;
}

.appContainer {
	width: 55%;
	margin-top: 50px;
}

.highlight {
	background-color: var(--highlight);
}

.slide-enter-active {
	animation: slide-in 200ms ease-out forwards;
}

.slide-leave-active {
	animation: slide-out 200ms ease-out forwards;
}

@keyframes slide-in {
	from {
		transform: translateX(-100px);
		opacity: 0;
	}
	to {
		transform: translateX(0);
		opacity: 1;
	}
}

@keyframes slide-out {
	from {
		transform: translateX(0);
		opacity: 1;
	}
	to {
		transform: translateX(-100px);
		opacity: 0;
	}
}

@media screen and (max-width: 576px), (max-width: 768px) {
	.appContainer {
		width: 100%;
	}

	:root {
		--primaryTextSize: 16px;
		--ideaTextSize: 18px;
		--ideaDescriptionTextSize: 13px;
		--categoryIconSize: 28px;
		--categoryIconBgSize: 56px;
		--badgePadding: 4px;
		--badgeTextSize: 10px;
		--avatarSize: 24px;
		--commentPadding: 8px;
		--cardMargin: 16px 16px 0px 16px;
		--authorLblSize: 13px;
		--dateLblSize: 10px;
		--dateLblMargin: 16px;
	}

	.col-xs-2,
	.col-xs-8 {
		padding: 0;
		margin: 0;
	}

	#deleteBtn {
		width: 104px;
	}
}
</style>
