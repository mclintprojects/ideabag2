<template>
  <ul id="ideaList">
		<li v-for="(idea, index) in ideas" :key="index" @click="notifyIdeaClicked(idea, index)" :class="{'highlight': index === selectedIndex, 'progress-undecided': idea.progress === 'undecided', 'progress-in-progress': idea.progress === 'in-progress', 'progress-done': idea.progress === 'done'}">
			<div class="ideaItem">
        <div>
  				<p id="ideaTitle" class="primaryLbl">{{idea.title}}</p>
  				<p id="ideaDifficulty" class="badge secondaryLbl">{{idea.difficulty}}</p>
        </div>
        <div>
          <popper trigger="click" :options="{placement: 'left'}" @created="openNewPopper">
            <div class="popper idea-actions">
              <button @click.stop="toggleBookmark(index)" v-show="idea.bookmarked">Remove bookmark</button>
              <button @click.stop="toggleBookmark(index)" v-show="!idea.bookmarked">Bookmark</button>
            </div>
            <img slot="reference" src="../../public/img/baseline-more_vert-24px.svg" alt="Idea actions" @click.stop>
          </popper>
        </div>
			</div>
		</li>
	</ul>
</template>

<script>
import Vue from 'vue';
import Popper from 'vue-popperjs';
import UserDataDBInterface from '../mixins/UserDataDBInterface';

export default {
  mixins: [UserDataDBInterface],
  components: {
    'popper': Popper
  },
  data() {
    return {
      openPopper: null
    }
  },
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
  watch: {
    userDataDB(db) {
      if (db !== null && this.ideas.length > 0) {
        this.loadIdeaData();
      }
    },
    ideas(ideas) {
      if (this.userDataDB !== null && ideas.length > 0) {
        this.loadIdeaData();
      }
    }
  },
  methods: {
    loadIdeaData() {
      for (let i = 0; i < this.ideas.length; i++) {
        this.userDataDB
          .transaction(['ideas'])
          .objectStore('ideas')
          .get(
            `${this.ideas[i].categoryId}C-${this.ideas[i].id}I`
          ).onsuccess = event => {
          if (this.ideas.length > i) {
            if (event.target.result !== undefined) {
              Vue.set(this.ideas[i], 'progress', event.target.result.progress);
              Vue.set(this.ideas[i], 'bookmarked', event.target.result.bookmarked ? true : false);
            } else {
              Vue.set(this.ideas[i], 'progress', 'undecided');
              Vue.set(this.ideas[i], 'bookmarked', false);
            }
          }
        };
      }
    },
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
    },
    toggleBookmark(index) {
      const idea = this.ideas[index];
      const id = `${idea.categoryId}C-${idea.id}I`;
      if (idea.bookmarked) {
        this.removeFromBookmarks(id);
        idea.bookmarked = false;
      } else {
        this.addToBookmarks(id);
        idea.bookmarked = true;
      }
      this.openPopper.doClose();
    },
    openNewPopper(context) {
      if (this.openPopper && this.openPopper !== context) {
        this.openPopper.doClose();
      }
      this.openPopper = context;
    }
  },
  activated() {
    if (this.userDataDB !== null && this.ideas.length > 0) {
      this.loadIdeaData();
    }
  },
  created() {
    if (this.userDataDB !== null && this.ideas.length > 0) {
      this.loadIdeaData();
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
  border-left: 8px solid transparent;
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
.ideaItem {
  display: flex;
  align-items: center;
  justify-content: space-between;
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
.badge {
  background-color: var(--primary);
  padding: var(--badgePadding);
  color: rgba(0, 0, 0, 0.54);
  font-size: var(--badgeTextSize);
}
.idea-actions {
  border: none;
  display: flex;
  flex-flow: column;
  padding: 0;
}
.idea-actions > button {
  background-color: transparent;
  border: none;
  padding: 1rem;
}
.idea-actions > button:hover {
  background-color: rgba(0, 0, 0, 0.2);
}
</style>
