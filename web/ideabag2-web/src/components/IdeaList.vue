<template>
	<div class="appContainer">
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
import { eventBus } from '../eventBus'

let me = this;

export default {
	data() {
		return {
			ideas: []
		};
	},
	methods: {
		notifyIdeaClicked(index) {
			var idea = this.ideas[index];
			this.$router.push('/ideas/detail');
			eventBus.$emit('ideaClicked', idea);
		}
	},
	created() {
		eventBus.$on('categoryClicked', (ideas) => this.ideas = ideas);
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


