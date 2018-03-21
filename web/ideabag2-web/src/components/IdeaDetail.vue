<template>
	<div class="appContainer">
		<img v-if="isLoading" id="loadingCircle" src="https://samherbert.net/svg-loaders/svg-loaders/oval.svg" />
		<div id="card" v-if="idea != null">
			<p id="ideaTitle">{{idea.title}}</p>
			<p id="ideaDescription">{{idea.description}}</p>
		</div>
	</div>
</template>

<script>
export default {
	data() {
		return {
			idea: null
		};
	},
	computed: {
		isLoading() {
			return this.$store.getters.categories.length == 0;
		},
	},
	activated() {
		this.$store.dispatch('setTitle', 'Idea details');

		var categoryIndex = this.$route.params.categoryId;
		var ideaIndex = this.$route.params.ideaId;

		this.idea = this.$store.getters.categories[categoryIndex].items[ideaIndex];
	}
};
</script>

<style scoped>
#card {
	border: 2px solid transparent;
	border-radius: 10px;
	background-color: var(--primary);
	padding: 16px;
	margin: 16px;
}

#ideaTitle {
	color: rgba(0, 0, 0, 0.8);
	font-size: var(--ideaTextSize);
	font-weight: bold;
}

#ideaDescription {
	color: rgba(0, 0, 0, 0.54);
	font-size: 16px;
	white-space: pre-wrap;
	word-wrap: break-word;
}
</style>


