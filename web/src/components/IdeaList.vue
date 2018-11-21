<template>
  <ul id="ideaList">
		<li v-for="(idea, index) in ideas" :key="index" @click="notifyIdeaClicked(idea, index)" :class="{highlight: index == selectedIndex}">
			<div class="ideaItem">
				<p id="ideaTitle" class="primaryLbl">{{idea.title}}</p>
				<p id="ideaDifficulty" class="badge secondaryLbl">{{idea.difficulty}}</p>
			</div>
		</li>
	</ul>
</template>

<script>
export default {
	computed: {
		selectedIndex() {
			return this.$store.getters.selectedIdeaIndex;
		}
	},
	props: {
		ideas: {
			type: Array,
			required: true
		}
	},
	methods: {
		notifyIdeaClicked(idea, index) {
			const categoryId =
				this.$store.getters.categories.findIndex(
					x => x.categoryLbl == idea.category
				) + 1;
			this.$router.push({
				name: 'ideas',
				params: { categoryId: categoryId, ideaId: idea.id }
			});
			this.$store.dispatch('setSelectedIdeaIndex', index);
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