<template>
	<div class="appContainer">
		<img v-if="isLoading" id="loadingCircle" src="https://samherbert.net/svg-loaders/svg-loaders/oval.svg" />
		<ul id="ideaList">
			<li v-for="(idea, index) in ideas" :key="index" @click="notifyIdeaClicked(index)" :class="{highlight: index == selectedIndex}">
				<div class="ideaItem">
					<p id="ideaTitle" class="primaryLbl">{{idea.title}}</p>
					<p id="ideaDifficulty" class="badge secondaryLbl">{{idea.difficulty}}</p>
				</div>
			</li>
		</ul>
	</div>
</template>

<script>
export default {
	data() {
		return {
			ideas: []
		};
	},
	computed: {
		isLoading() {
			return this.$store.getters.categories.length == 0;
		},
		selectedIndex() {
			return this.$store.getters.selectedIdeaIndex;
		}
	},
	methods: {
		notifyIdeaClicked(index) {
			this.$router.push({ name: 'ideas', params: { categoryId: this.$route.params.categoryId, ideaId: index } });

			this.$store.dispatch('setSelectedIdeaIndex', index);
		}
	},
	activated() {
		if (this.$store.getters.categories) {
			var title = this.$store.getters.categories[this.$route.params.categoryId].categoryLbl;
			this.$store.dispatch('setTitle', title);

			var index = this.$route.params.categoryId;
			this.ideas = this.$store.getters.categories[index].items;
		}
	}
};
</script>

<style scoped>
#ideaList {
	list-style-type: none;
	margin: 0px;
	padding: 0px;
}

#ideaList li {
	padding: 8px 16px 8px 16px;
}

#ideaList li:hover {
	background-color: var(--highlight);
	cursor: pointer;
}

#ideaTitle {
	font-size: var(--primaryTextSize);
	margin: 0px 0px 4px 0px;
	text-overflow: ellipsis;
	overflow: hidden;
	white-space: nowrap;
	padding: 0px;
}

.badge {
	background-color: var(--primary);
	padding: var(--badgePadding);
	color: rgba(0, 0, 0, 0.54);
	font-size: var(--badgeTextSize);
}
</style>


