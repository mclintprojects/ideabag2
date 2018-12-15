<template>
  <div class="full-width-container">
    <div class="appContainer">
      <div id="progress-bar">
        <div id="progress" :style="{width: ideaProgress + '%'}"></div>
      </div>
      <ul id="ideaList">
    		<li v-for="(idea, index) in ideas" :key="index" v-show="difficultyFilter === 'All' || idea.difficulty === difficultyFilter" :class="{'highlight': index === selectedIndex, 'progress-undecided': idea.progress === 'undecided', 'progress-in-progress': idea.progress === 'in-progress', 'progress-done': idea.progress === 'done'}">
    			<div class="ideaItem" @click="notifyIdeaClicked(idea, index)">
            <div>
      				<p id="ideaTitle" class="primaryLbl">{{idea.title}}</p>
      				<p id="ideaDifficulty" class="badge secondaryLbl">{{idea.difficulty}}</p>
            </div>
            <div class="idea-buttons">
              <div v-show="largeScreen">
                <button class="appBtnOutline" @click.stop="toggleBookmark(index)">
                  <font-awesome-icon :icon="[idea.bookmarked ? 'fas' : 'far', 'bookmark']" size="lg" fixed-width></font-awesome-icon>
                </button>
                <button class="appBtnOutline" @click.stop="$modal.show('progress-modal-' + getDataId(idea))">Update progress</button>
              </div>
              <popper trigger="click" :options="{placement: 'left'}" v-show="!largeScreen" @created="openNewPopper">
                <div class="popper idea-menu-actions">
                  <button @click.stop="openPopper.doClose();toggleBookmark(index)">{{ idea.bookmarked ? "Remove bookmark" : "Bookmark" }}</button>
                  <button @click.stop="openPopper.doClose();$modal.show('progress-modal-' + getDataId(idea))">Update progress</button>
                </div>
                <button class="icon-button" slot="reference" @click.stop>
                  <font-awesome-icon icon="ellipsis-v" size="lg" fixed-width></font-awesome-icon>
                </button>
              </popper>
            </div>
    			</div>
          <progress-modal v-if="idea.progress" @update-progress="event => setProgress(idea, index, event)" :progress="idea.progress" :id="getDataId(idea)"></progress-modal>
    		</li>
    	</ul>
    </div>
    <button class="appBtn floating-action-button" @click="$modal.show('sort-modal')">
      <font-awesome-icon icon="filter" size="lg" fixed-width></font-awesome-icon>
    </button>
    <modal name="sort-modal" height="auto" :adaptive="true">
      <ul class="modal-list">
        <li v-for="(difficulty, index) in ['All', 'Beginner', 'Intermediate', 'Expert']" :key="index" @click="$modal.hide('sort-modal');difficultyFilter = difficulty">
          <input v-model="difficultyFilter" :id="'difficulty' + difficulty" type="radio" name="difficulty" :value="difficulty">
          <label :for="'difficulty' + difficulty">{{ difficulty }}</label>
        </li>
      </ul>
    </modal>
  </div>
</template>

<script>
import Vue from 'vue';
import Popper from 'vue-popperjs';
import UserDataDBInterface from '../mixins/UserDataDBInterface';
import ProgressModal from '../components/ProgressModal';

export default {
  mixins: [UserDataDBInterface],
  components: {
    'popper': Popper,
    'progress-modal': ProgressModal
  },
  data() {
    return {
      openPopper: null,
      mediaQueryList: window.matchMedia('only screen and (min-width: 1200px)'),
      largeScreen: window.matchMedia('only screen and (min-width: 1200px)').matches,
      ideaProgress: 0,
      difficultyFilter: 'All'
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
    getDataId(idea) {
      return `${idea.categoryId}C-${idea.id}I`;
    },
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
            if (i == this.ideas.length - 1) {
              this.ideaProgress = this.getIdeaProgress();
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
      this.$emit('needs-update');
    },
    openNewPopper(context) {
      if (this.openPopper && this.openPopper !== context) {
        this.openPopper.doClose();
      }
      this.openPopper = context;
    },
    setProgress(idea, index, progress) {
      this.ideas[index].progress = progress;
      this.ideaProgress = this.getIdeaProgress();
      this.updateProgress(this.getDataId(idea), progress);
    },
    getIdeaProgress() {
      const completedIdeas = this.ideas.reduce((accumulator, idea) => {
        if (idea.progress === 'done') return accumulator + 1;
        else return accumulator;
      }, 0);
      return completedIdeas / this.ideas.length * 100;
    },
    handleResize(mediaQueryList) {
      this.largeScreen = mediaQueryList.matches;
    }
  },
  activated() {
    this.mediaQueryList.addListener(this.handleResize);
    if (this.userDataDB !== null && this.ideas.length > 0) {
      this.loadIdeaData();
    }
  },
  deactivated() {
    this.mediaQueryList.removeListener(this.handleResize);
  },
  created() {
    if (this.userDataDB !== null && this.ideas.length > 0) {
      this.loadIdeaData();
    }
  }
};
</script>

<style scoped>
#progress-bar {
  background-color: var(--progress-bar-background);
  width: 100%;
  height: 10px;
}
#progress {
  background-color: var(--primaryDark);
  height: 100%;
}
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
.idea-buttons {
  display: flex;
  justify-content: center;
  align-items: center;
}
.idea-menu-actions {
  border: none;
  display: flex;
  flex-flow: column;
  padding: 0;
}
.idea-menu-actions > button {
  background-color: transparent;
  border: none;
  padding: 1rem;
}
.idea-menu-actions > button:hover {
  background-color: rgba(0, 0, 0, 0.2);
}
</style>
