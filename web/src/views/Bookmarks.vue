<template>
  <div class="container-full">
    <font-awesome-icon class="loader" v-if="isLoading" icon="spinner" size="3x" spin fixed-width></font-awesome-icon>
    <div class="no-ideas-to-display" v-if="!isLoading && ideas.length == 0">
      <font-awesome-icon :icon="['fas', 'bookmark']" size="6x"></font-awesome-icon>
      <h2>No bookmarks to show</h2>
      <p>Ideas that you bookmark will show up here</p>
    </div>
    <idea-list @needs-update="loadIdeas" v-if="!isLoading && ideasLoaded" :ideas="ideas"/>
  </div>
</template>

<script>
import IdeaList from '../components/IdeaList';

export default {
  data() {
    return {
      ideas: [],
      ideasLoaded: false
    };
  },
  computed: {
    isLoading() {
      return this.$store.getters.categories.length == 0;
    },
    userDataDB() {
      return this.$store.getters.userDataDB;
    }
  },
  methods: {
    loadIdeas() {
      this.userDataDB
        .transaction(['ideas'])
        .objectStore('ideas')
        .index('bookmarked')
        .openKeyCursor(IDBKeyRange.only(1)).onsuccess = event => {
        const cursor = event.target.result;
        if (cursor) {
          this.loadIdea(cursor.primaryKey);
          cursor.continue();
        } else {
          this.ideasLoaded = true;
        }
      };
    },
    loadIdea(id) {
      const categoryId = id.split('C')[0];
      const ideaId = id.split('-')[1].split('I')[0];
      this.ideas.push(
        this.$store.getters.categories[categoryId - 1].items[ideaId - 1]
      );
    }
  },
  components: { IdeaList },
  activated() {
    this.$store.dispatch('setTitle', 'Bookmarks');
    this.loadIdeas();
  },
  deactivated() {
    this.ideas = [];
    this.ideasLoaded = false;
  }
};
</script>
