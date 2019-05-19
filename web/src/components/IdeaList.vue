<template>
  <div class="container-full">
    <div class="container-app">
      <div class="progress-bar" v-show="filteredIdeas.length !== 0">
        <div class="progress-bar-progress" :style="{width: ideaProgress + '%'}"></div>
      </div>
      <ul class="ideas" v-if="filteredIdeas.length > 0">
        <li
          v-for="(idea, index) in filteredIdeas"
          :key="index"
          :class="{'highlight': index === selectedIndex, 'progress--undecided': idea.progress === 'undecided', 'progress--in-progress': idea.progress === 'in-progress', 'progress--done': idea.progress === 'done'}"
        >
          <div class="idea" @click="notifyIdeaClicked(idea, index)">
            <div>
              <p class="idea__title text--primary">
                {{idea.title}}
                <span v-if="isNewIdea(idea)">New</span>
              </p>
              <p class="idea__difficulty">{{idea.difficulty}}</p>
            </div>
            <div class="idea-buttons">
              <div v-show="largeScreen">
                <button class="button button--outlined" @click.stop="toggleBookmark(index)">
                  <font-awesome-icon
                    :icon="[idea.bookmarked ? 'fas' : 'far', 'bookmark']"
                    size="lg"
                    fixed-width
                  ></font-awesome-icon>
                </button>
                <button
                  class="button button--outlined"
                  @click.stop="$modal.show('progress-modal-' + getDataId(idea))"
                >Update progress</button>
              </div>
              <popper
                trigger="click"
                :options="{placement: 'left'}"
                v-show="!largeScreen"
                @created="openNewPopper"
              >
                <div class="popper idea-menu-actions">
                  <button
                    @click.stop="openPopper.doClose();toggleBookmark(index)"
                  >{{ idea.bookmarked ? "Remove bookmark" : "Bookmark" }}</button>
                  <button
                    @click.stop="openPopper.doClose();$modal.show('progress-modal-' + getDataId(idea))"
                  >Update progress</button>
                </div>
                <button class="icon-button" slot="reference" @click.stop>
                  <font-awesome-icon icon="ellipsis-v" size="lg" fixed-width></font-awesome-icon>
                </button>
              </popper>
            </div>
          </div>
          <progress-modal
            v-if="idea.progress"
            @update-progress="event => setProgress(idea, index, event)"
            :progress="idea.progress"
            :id="getDataId(idea)"
          ></progress-modal>
        </li>
      </ul>
      <div class="no-ideas-to-display" v-show="ideas.length !== 0 && filteredIdeas.length === 0">
        <font-awesome-icon icon="filter" size="6x"></font-awesome-icon>
        <h2>No Ideas that match the filter</h2>
        <p>There are no Ideas that match the current filter settings</p>
      </div>
    </div>
    <button class="button floating-action-button" @click="$modal.show('sort-modal')">
      <font-awesome-icon icon="filter" size="lg" fixed-width></font-awesome-icon>
    </button>
    <modal name="sort-modal" height="auto" :adaptive="true">
      <ul class="modal-list">
        <li
          class="modal-list-item"
          v-for="(difficulty, index) in ['All', 'Beginner', 'Intermediate', 'Expert']"
          :key="index"
          @click="$modal.hide('sort-modal');difficultyFilter = difficulty"
        >
          <input
            class="modal-list-item__field"
            v-model="difficultyFilter"
            :class="'difficulty' + difficulty"
            type="radio"
            name="difficulty"
            :value="difficulty"
          >
          <label class="modal-list-item__label" :for="'difficulty' + difficulty">{{ difficulty }}</label>
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
    popper: Popper,
    'progress-modal': ProgressModal
  },
  data() {
    return {
      openPopper: null,
      mediaQueryList: window.matchMedia('only screen and (min-width: 120rem)'),
      largeScreen: window.matchMedia('only screen and (min-width: 120rem)').matches,
      ideaProgress: 0,
      difficultyFilter: 'All'
    };
  },
  computed: {
    selectedIndex() {
      return this.$store.getters.selectedIdeaIndex;
    },
    filteredIdeas() {
      if (this.difficultyFilter === 'All') {
        return this.ideas;
      }
      return this.ideas.filter(
        idea => idea.difficulty === this.difficultyFilter
      );
    },
    newIdeas() {
      return this.$store.getters.newIdeas;
    }
  },
  props: {
    ideas: {
      type: Array,
      required: true
    }
  },
  methods: {
    getDataId(idea) {
      return `${idea.categoryId}C-${idea.id}I`;
    },
    notifyIdeaClicked(idea, index) {
      const categoryId =
        this.$store.getters.categories.findIndex(
          x => x.categoryLbl === idea.category
        ) + 1;
      this.$router.push({
        name: 'ideas',
        params: { categoryId: categoryId, ideaId: idea.id }
      });
      this.$store.dispatch('setSelectedIdeaIndex', index);
    },
    toggleBookmark(index) {
      const idea = this.filteredIdeas[index];
      const id = this.getDataId(idea);
      if (idea.bookmarked) {
        this.removeFromBookmarks(id);
      } else {
        this.addToBookmarks(id);
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
      this.updateProgress(this.getDataId(idea), progress);
      this.setIdeaProgress();
    },
    setIdeaProgress() {
      const completedIdeas = this.ideas.reduce((accumulator, idea) => {
        if (idea.progress === 'done') return accumulator + 1;
        else return accumulator;
      }, 0);
      this.ideaProgress = (completedIdeas / this.ideas.length) * 100;
    },
    handleResize(mediaQueryList) {
      this.largeScreen = mediaQueryList.matches;
    },
    isNewIdea(idea) {
      return (
        this.newIdeas.findIndex(
          i => i.categoryId === idea.categoryId && i.ideaId === idea.id
        ) !== -1
      );
    }
  },
  activated() {
    this.mediaQueryList.addListener(this.handleResize);
  },
  deactivated() {
    this.mediaQueryList.removeListener(this.handleResize);
  }
};
</script>

<style>
.progress-bar {
  background-color: var(--progress-bar-background);
  width: 100%;
  height: 1rem;
}

.progress-bar-progress {
  background-color: var(--primaryDark);
  height: 100%;
}

.ideas {
  list-style-type: none;
  margin: 0;
  padding: 0;
}

.ideas > li {
  border-left: 0.8rem solid transparent;
  padding: 0.8rem 1.6rem 0.8rem 1.6rem;
}

.ideas > li:hover {
  background-color: var(--highlight);
  cursor: pointer;
}

.idea {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.idea__title {
  font-size: var(--primaryTextSize);
  margin-bottom: 0.4rem;
  text-overflow: ellipsis;
  overflow: hidden;
  white-space: nowrap;
}

.idea__title > span {
  font-size: 1.1rem;
  margin-left: 0.8rem;
  color: rgba(0, 0, 0, 0.54);
  background: white;
  padding: 0.4rem 0.8rem;
  border-radius: 0.4rem;
  text-transform: uppercase;
}

.idea__difficulty {
  display: inline-block;
  border-radius: 0.4rem;
  background-color: var(--primary);
  padding: var(--badgePadding);
  color: rgba(0, 0, 0, 0.54);
  font-size: var(--badgeTextSize);
  text-transform: uppercase;
}

.progress--undecided {
  border-left: 0.8rem solid var(--undecided) !important;
}

.progress--in-progress {
  border-left: 0.8rem solid var(--in-progress) !important;
}

.progress--done {
  border-left: 0.8rem solid var(--done) !important;
}

.idea-buttons {
  display: flex;
  justify-content: center;
  align-items: center;
}

.idea-menu-actions {
  border: none;
  display: flex !important;
  flex-direction: column;
  padding: 0;
}

.idea-menu-actions button {
  font-size: 1.4rem;
  text-align: left;
  color: rgba(0, 0, 0, 0.8);
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
