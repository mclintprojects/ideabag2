<template>
	<div class="main-container">
		<navbar></navbar>
		<keep-alive>
			<router-view></router-view>
		</keep-alive>
	</div>
</template>

<script>
import CategoryList from './components/CategoryList';
import IdeaList from './components/IdeaList';
import IdeaDetail from './components/IdeaDetail';
import Navbar from './components/Navbar';

let ideasURL =
	'https://docs.google.com/document/d/17V3r4fJ2udoG5woDBW3IVqjxZdfsbZC04G1A-It_DRU/export?format=txt';

export default {
	name: 'app',
	components: { 'navbar': Navbar },
	methods: {
		getData() {
			var ideasdb = localStorage.getItem('ideasdb');

			if (ideasdb) {
				this.$store.state.isLoading = false;
				this.$toasted.show('Loaded offline cache.', {
					duration: 3000,
					position: 'bottom-center'
				});
				return JSON.parse(ideasdb);
			}

			return [];
		},
		saveData(ideasdb) {
			localStorage.setItem('ideasdb', JSON.stringify(ideasdb));
		}
	},
	created() {
		this.$store.state.categories = this.getData();

		this.$http.get(ideasURL).then(response => {
			this.$store.state.isLoading = false;
			this.$store.state.categories = response.body;
			this.saveData(response.body);
		}, error => {
			this.$toasted.show('Couldn\'t load data. Please check your connection and reload.', {
				duration: 5000,
				position: 'bottom-center'
			});
		});
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

@media (max-width: 576px),
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
	}
}
</style>
