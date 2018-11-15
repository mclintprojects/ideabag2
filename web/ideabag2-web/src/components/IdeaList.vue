<template>
  <ul id="ideaList">
		<li v-for="(idea, index) in ideas" :key="index" @click="notifyIdeaClicked(idea, index)" :class="{'highlight': index === selectedIndex, 'progress-undecided': idea.progress === 'undecided', 'progress-in-progress': idea.progress === 'in-progress', 'progress-done': idea.progress === 'done'}">
			<div :class="'ideaItem'">
				<p id="ideaTitle" class="primaryLbl">{{idea.title}}</p>
				<p id="ideaDifficulty" class="badge secondaryLbl">{{idea.difficulty}}</p>
			</div>
		</li>
	</ul>
</template>

<script>
import Vue from "vue";

export default {
  computed: {
    selectedIndex() {
			return this.$store.getters.selectedIdeaIndex;
		},
    userDataDB() {
			return this.$store.getters.userDataDB;
		}
  },
  props: {
    ideas: {
      type: Array,
      required: true
    }
  },
  watch: {
    userDataDB(db) {
      if (db !== null && this.ideas.length > 0) {
        this.loadProgressData();
      }
    },
    ideas(ideas) {
      if (this.userDataDB !== null && ideas.length > 0) {
        this.loadProgressData();
      }
    }
  },
  methods: {
    loadProgressData() {
      for (let i = 0; i < this.ideas.length; i++) {
        this.userDataDB.transaction(["ideas"])
        .objectStore("ideas")
        .get(this.getDataId(this.ideas[i].id))
        .onsuccess = event => {
          if (this.ideas.length > i) {
            if (event.target.result !== undefined) {
              Vue.set(this.ideas[i], "progress", event.target.result.progress);
            } else {
              Vue.set(this.ideas[i], "progress", "undecided");
            }
          }
        }
      }
    },
    getDataId(ideaId) {
			var categoryId = this.$route.params.categoryId;
			return `${categoryId}C-${ideaId}I`;
		},
    notifyIdeaClicked(idea, index) {
      const categoryId = this.$store.getters.categories.findIndex(x => x.categoryLbl == idea.category);
			this.$router.push({ name: 'ideas', params: { categoryId: categoryId, ideaId: idea.id - 1 } });

			this.$store.dispatch('setSelectedIdeaIndex', index);
		}
  },
  activated() {
    if (this.userDataDB !== null && this.ideas.length > 0) {
      this.loadProgressData();
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
  border-left: 7px solid transparent;
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
.progress-undecided {
  border-left: 8px solid var(--undecided) !important;
}
.progress-in-progress {
  border-left: 8px solid var(--in-progress) !important;
}
.progress-done {
  border-left: 8px solid var(--done) !important;
}
</style>
