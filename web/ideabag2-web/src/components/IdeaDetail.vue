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
		}
	},
	computed: {
		isLoading() {
			return this.$store.state.categories.length == 0;
		}
	},
	watch: {
		'$store.state.categories'() {
			if (this.$store.state.categories.length > 0)
				this.showIdeas();
		}
	},
	methods: {
		showIdeas() {
			var categoryIndex = this.$route.params.categoryId;
			var ideaIndex = this.$route.params.ideaId;

			this.idea = this.$store.state.categories[categoryIndex].items[ideaIndex];
		}
	},
	activated() {
		if (this.$store.state.categories)
			this.showIdeas();
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


