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

export default {
	name: 'app',
	data() {
		return {
			categories: [],
			selectedCategoryIdeas: null,
			selectedIdea: null,
			categorySelectionIndex: 0,
			ideaSelectionIndex: 0,
			isLoading: true,
			isRootComponent: false
		};
	},
	components: { 'navbar': Navbar },
	methods: {
		selectCategory(index) {
			this.selectedCategoryIdeas = this.categories[index].items;
			this.selectedIdea = null;
		},
		selectIdea(index) {
			this.selectedIdea = this.selectedCategoryIdeas[index];
		}
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

.navbar {
	background-color: var(--primary);
	border-radius: 0px;
}

#toolbar {
	display: flex;
	align-items: center;
	box-shadow: 0px 2px 6px rgba(0, 0, 0, 0.1);
}

#toolbar h4 {
	margin-left: 16px;
	color: rgba(255, 255, 255, 0.8);
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
