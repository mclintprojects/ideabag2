<template>
  <div>
    <div id="toolbar" class="navbar navbar-fixed-top">
      <h4>IdeaBag 2</h4>
    </div>
		<router-view></router-view>
  </div>
</template>

<script>
import CategoryList from './components/CategoryList';
import IdeaList from './components/IdeaList';
import IdeaDetail from './components/IdeaDetail';
import { ideasMixin } from './mixins/ideasMixin';

let ideasURL =
	'https://docs.google.com/document/d/17V3r4fJ2udoG5woDBW3IVqjxZdfsbZC04G1A-It_DRU/export?format=txt';

export default {
	name: 'app',
	data() {
		return {
			selectedCategoryIdeas: null,
			selectedIdea: null,
			categorySelectionIndex: 0,
			ideaSelectionIndex: 0,
			isLoading: true
		};
	},
	mixins: [ideasMixin],
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
}

body {
	background-color: var(--background);
	font-family: 'Roboto', sans-serif;
	overflow-x: hidden;
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
</style>
