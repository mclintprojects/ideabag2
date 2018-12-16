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

let ideasURL =
	'https://docs.google.com/document/d/17V3r4fJ2udoG5woDBW3IVqjxZdfsbZC04G1A-It_DRU/export?format=txt';

export default {
	name: 'app',
	components: { Navbar },
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
		createDb(version = 1) {
			const request = indexedDB.open('userData', version);

			request.onupgradeneeded = event => {
				const db = event.target.result;
				const ideasStore = db.createObjectStore('ideas', { keyPath: 'id' });
				ideasStore.createIndex('bookmarked', 'bookmarked');
			};

			request.onsuccess = event => {
				const db = event.target.result;
				if (!db.objectStoreNames.contains('ideas'))
					this.createDb(db.version + 1);
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
	--progress-bar-background: #2b2b2b;
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
  height: 36px;
	color: white;
	position: absolute;
	left: calc(50% - 36px);
	top: calc(50% - 36px);
}

.appBtn {
	background-color: var(--primary);
	border: solid 0px transparent;
	border-radius: 2px;
	color: white;
	width: 100px;
	height: 40px;
	transition: all 1s;
}
.appBtn:hover {
	background-color: var(--primaryDark);
	color: white;
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
.floating-action-button {
  border-radius: 180px;
  bottom: 20px;
  display: flex;
  justify-content: center;
	align-items: center;
  height: 60px;
  position: fixed;
  right: 20px;
  width: 60px;
}
.icon-button {
	border: none;
	background-color: transparent;
	color: white;
	padding: 1rem;
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

.modal-list {
	padding: 0;
	margin: 0;
}
.modal-list > li {
	border-top: 1px solid black;
	cursor: pointer;
	font-size: 1.7rem;
	list-style-type: none;
	padding: 2rem 3rem;
	width: 100%;
}
.modal-list > li:hover {
	background-color: rgba(0, 0, 0, 0.2);
}
.modal-list > li > input,
.modal-list > li > label {
	cursor: pointer;
}
.modal-list > li > label {
	padding-left: 1rem;
}
.no-ideas-to-display {
	color: var(--primaryTextLight);
  display: flex;
  flex-flow: column nowrap;
  height: 100%;
  justify-content: center;
  align-items: center;
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
	height: 100%;
	width: 55%;
	margin: 0 auto;
	padding-top: 50px;
}
.full-space-container {
	height: 100%;
	width: 100%;
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
