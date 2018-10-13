<template>
	<div class="appContainer">
		<img v-if="isLoading" id="loadingCircle" src="https://samherbert.net/svg-loaders/svg-loaders/oval.svg" />
		<idea-list :ideas="ideas" />
	</div>
</template>

<script>
import IdeaList from "../components/IdeaList";

export default {
	data() {
		return {
			ideas: []
		};
	},
	computed: {
		isLoading() {
			return this.$store.getters.categories.length == 0;
		}
	},
	components: {
		"idea-list": IdeaList
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
