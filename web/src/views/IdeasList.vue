<template>
  <div class="container-full">
    <font-awesome-icon class="loader" v-if="isLoading" icon="spinner" size="3x" spin fixed-width></font-awesome-icon>
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
      const categoryIndex = this.$route.params.categoryId - 1;
      this.ideas = this.$store.getters.categories[categoryIndex].items;
      const title = this.$store.getters.categories[categoryIndex].categoryLbl;
      this.$store.dispatch('setTitle', title);
    }
  }
};
</script>
