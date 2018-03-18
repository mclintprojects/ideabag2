<template>
	<div class="appContainer">
		<img v-if="isLoading" id="loadingCircle" src="https://samherbert.net/svg-loaders/svg-loaders/oval.svg" />
		<ul id="categoryList">
			<li v-for="(category, index) in categories" :key="index" @click="notifyCategoryClicked(index)">
				<div class="categoryItem">
					<div class="categoryIconBg">
						<img class="categoryIcon" :src="icons[index]" />
					</div>
					<div class="categoryContent">
						<p id="categoryTitle" class="primaryLbl">{{category.categoryLbl}}</p>
						<p class="secondaryLbl">Ideas: {{category.categoryCount}}</p>
					</div>
				</div>
			</li>
		</ul>
	</div>
</template>

<script>
import { eventBus } from '../eventBus'

let ideasURL =
	'https://docs.google.com/document/d/17V3r4fJ2udoG5woDBW3IVqjxZdfsbZC04G1A-It_DRU/export?format=txt';

export default {
	data() {
		return {
			icons: [
				'../src/assets/numbers.png',
				'../src/assets/text.png',
				'../src/assets/network.png',
				'../src/assets/enterprise.png',
				'../src/assets/cpu.png',
				'../src/assets/web.png',
				'../src/assets/file.png',
				'../src/assets/database.png',
				'../src/assets/multimedia.png',
				'../src/assets/games.png'
			],
			categories: [],
			isLoading: true
		};
	},
	methods: {
		notifyCategoryClicked(index) {
			var ideas = this.categories[index].items;
			eventBus.$emit('categoryClicked', ideas)
			this.$router.push('/ideas');
		}
	},
	created() {
		this.$http.get(ideasURL)
			.then(response => {
				this.isLoading = false;
				this.categories = response.body;
			});
	}
};
</script>

<style scoped>
#loadingCircle {
	width: 36px;
	position: relative;
	left: 50%;
}

.categoryIcon {
	width: var(--categoryIconSize);
}

.categoryIconBg {
	background: rgba(0, 0, 0, 0.8);
	display: flex;
	justify-content: center;
	align-items: center;
	width: var(--categoryIconBgSize);
	height: var(--categoryIconBgSize);
	border-radius: 180px;
}

.categoryContent {
	display: flex;
	flex-direction: column;
	justify-content: center;
	margin-left: 16px;
}

#categoryList {
	list-style-type: none;
	margin: 0px;
	padding: 0px;
}

#categoryList li:hover {
	background-color: var(--highlight);
	cursor: pointer;
}

#categoryTitle {
	font-size: var(--primaryTextSize);
	margin: 0px;
}

.categoryItem {
	display: flex;
	flex-direction: row;
	padding: 8px 16px 8px 16px;
}
</style>


