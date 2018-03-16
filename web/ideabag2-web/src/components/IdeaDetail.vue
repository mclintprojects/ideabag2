<template>
	<div class="appContainer">
		<div id="card" v-if="idea != null">
			<p id="ideaTitle">{{idea.title}}</p>
			<p id="ideaDescription">{{idea.description}}</p>
		</div>
	</div>
</template>

<script>
import { eventBus } from '../eventBus'
let me = null;

export default {
	data() {
		return {
			idea: null
		}
	},
	created() {
		me = this;
	}
};

eventBus.$on('ideaClicked', (idea) => {
	me.idea = idea;
});
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


