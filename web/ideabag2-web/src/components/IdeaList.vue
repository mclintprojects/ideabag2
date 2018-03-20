<template>
	<div class="appContainer">
		<img v-if="isLoading" id="loadingCircle" src="https://samherbert.net/svg-loaders/svg-loaders/oval.svg" />
		<ul id="ideaList">
			<li v-for="(idea, index) in ideas" :key="index" @click="notifyIdeaClicked(index)">
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
			return this.$store.state.categories.length == 0;
		}
	},
	methods: {
		notifyIdeaClicked(index) {
			console.log('Id: ' + this.$route.params.categoryId + ' IdeaId: ' + index);
			this.$router.push({ name: 'ideas', params: { categoryId: this.$route.params.categoryId, ideaId: index } });
		},
		showIdeas() {
			var index = this.$route.params.categoryId;
			this.ideas = this.$store.state.categories[index].items;
		}
	},
	watch: {
		'$store.state.categories'() {
			if (this.$store.state.categories.length > 0)
				this.showIdeas();
		}
	},
	activated() {
		if (this.$store.state.categories)
			this.showIdeas();
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


