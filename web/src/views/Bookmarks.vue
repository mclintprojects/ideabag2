<template>
  <div class="container-full">
    <font-awesome-icon class="loader" v-if="isLoading" icon="spinner" size="3x" spin fixed-width></font-awesome-icon>
    <div class="no-ideas-to-display" v-if="!isLoading && ideas.length === 0">
      <font-awesome-icon :icon="['fas', 'bookmark']" size="6x"></font-awesome-icon>
      <h2>No bookmarks to show</h2>
      <p>Ideas that you bookmark will show up here</p>
    </div>
    <idea-list v-if="!isLoading && ideas.length > 0" :ideas="ideas"/>
  </div>
</template>

<script>
import IdeaList from '../components/IdeaList';

export default {
  data() {
    return {
      ideasLoaded: false
    };
  },
  computed: {
    isLoading() {
      return this.$store.getters.categories.length === 0;
    },
    userDataDB() {
      return this.$store.getters.userDataDB;
    },
    ideas() {
      const ideas = [];
      for (const category of this.$store.getters.categories) {
        for (const item of category.items) {
          if (item.bookmarked) {
            ideas.push(item);
          }
        }
      }
      return ideas;
    }
  },
  components: { IdeaList },
  activated() {
    this.$store.dispatch('setTitle', 'Bookmarks');
  }
};
</script>
