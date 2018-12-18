<template>
  <div class="full-space-container">
    <font-awesome-icon id="loadingCircle" v-if="isLoading" icon="spinner" size="3x" spin fixed-with></font-awesome-icon>
    <idea-list :ideas="ideas"/>
  </div>
</template>

<script>
import IdeaList from '../components/IdeaList';

export default {
  data() {
    return {
      ideas: []
    };
  },
  computed: {
    isLoading() {
      return this.$store.getters.categories.length == 0;
    }
  },
  components: { IdeaList },
  activated() {
    if (this.$store.getters.categories) {
      const categoryId = this.$route.params.categoryId;
      const title = this.$store.getters.categories[categoryId - 1].categoryLbl;
      this.$store.dispatch('setTitle', title);

      this.ideas = this.$store.getters.categories[categoryId - 1].items;
    }
  }
};
</script>
