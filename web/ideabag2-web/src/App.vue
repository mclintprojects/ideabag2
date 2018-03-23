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
import CategoryList from './components/CategoryList';
import IdeaList from './components/IdeaList';
import IdeaDetail from './components/IdeaDetail';
import Navbar from './components/Navbar';
import axios from 'axios';
import eventbus from './eventbus';

let ideasURL =
	'https://docs.google.com/document/d/17V3r4fJ2udoG5woDBW3IVqjxZdfsbZC04G1A-It_DRU/export?format=txt';

export default {
	name: 'app',
	components: { 'navbar': Navbar },
	methods: {
		getData() {
			var ideasdb = localStorage.getItem('ideasdb');

			if (ideasdb) {
				this.$store.dispatch('setLoading', false);
				eventbus.showToast('Loaded offline cache.', 'info');
				return JSON.parse(ideasdb);
			}

			return [];
		},
		tryLogin() {
			var loginData = localStorage.getItem('loginData');

			if (loginData) {
				this.$store.dispatch('loginUserLocal', loginData);
			}
		},
		saveData(ideasdb) {
			localStorage.setItem('ideasdb', JSON.stringify(ideasdb));
		}
	},
	created() {
		this.$store.dispatch('setCategories', this.getData());

		axios.get(ideasURL).then(response => {
			this.$store.dispatch('setCategories', response.data);
			this.$store.dispatch('setLoading', false);
			this.saveData(response.data);
		}).catch(error => {
			eventbus.showToast('Couldn\'t load data. Please check your connection and reload.', 'error', 'long');
		});

		this.tryLogin();
	}
};
</script>

<style>
:root {
	--primary: #ffa000;
	--primaryDark: #c67100;
	--background: #37474f;
	--highlight: #2c393f;
	--primaryText: rgba(255, 255, 255, 0.8);
	--primaryTextLight: rgba(255, 255, 255, 0.54);
	--primaryTextSize: 18px;
	--ideaTextSize: 22px;
	--categoryIconSize: 36px;
	--categoryIconBgSize: 72px;
	--badgePadding: 8px;
	--ideaDescriptionTextSize: 16px;
	--badgeTextSize: 12px;
	--avatarSize: 48px;
	--commentPadding: 16px;
	--cardMargin: 16px 0px 0px 0px;
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
	padding: 0px;
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

@media screen and (max-width: 576px),
(max-width: 768px) {
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
		--avatarSize: 36px;
		--commentPadding: 8px;
		--cardMargin: 16px 16px 0px 16px;
	}

	.col-xs-2,
	.col-xs-8 {
		padding: 0;
		margin: 0;
	}

	#deleteBtn {
		width: 104px;
	}

	#commentBar {
		margin: 0px 16px 0px 16px;
	}
}
</style>
