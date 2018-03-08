<template>
  <div id="ideasContainer">
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
	props: ['ideas'],
	methods: {
		notifyIdeaClicked(index) {
			this.$emit('ideaClicked', index);
		}
	}
};
</script>

<style scoped>
#ideasContainer {
	margin: 0px;
	padding: 0px;
}

#ideaList {
	list-style-type: none;
	margin: 0px;
	padding: 0px;
}

#ideaList li {
	padding: 8px 16px 8px 16px;
	height: 88px;
}

#ideaList li:hover {
	background-color: var(--highlight);
	cursor: pointer;
}

#ideaTitle {
	font-size: 18px;
	margin: 0px 0px 4px 0px;
	text-overflow: ellipsis;
	overflow: hidden;
	white-space: nowrap;
	padding: 0px;
}

.badge {
	background-color: var(--primary);
	padding: 8px;
	color: rgba(0, 0, 0, 0.54);
}
</style>


