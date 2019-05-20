<template>
  <div class="container-full">
    <font-awesome-icon class="loader" v-if="isLoading" icon="spinner" size="3x" spin fixed-width></font-awesome-icon>
    <idea-list :ideas="ideas"/>
  </div>
</template>

<script>
import IdeaList from '../components/IdeaList';

export default {
  computed: {
    isLoading() {
      return this.$store.getters.isLoading;
    },
    ideas() {
      if (this.$route.params.categoryId) {
        return this.$store.getters.categories[this.$route.params.categoryId - 1].items;
      } else {
        return [];
      }
    }
  },
  components: { IdeaList },
  activated() {
    const categoryIndex = this.$route.params.categoryId - 1;
    const title = this.$store.getters.categories[categoryIndex].categoryLbl;
    this.$store.dispatch('setTitle', title);
  }
};
</script>
