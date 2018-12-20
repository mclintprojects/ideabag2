<template>
  <div class="container-full">
    <font-awesome-icon class="loader" v-if="isLoading" icon="spinner" size="3x" spin fixed-width></font-awesome-icon>
    <idea-list v-if="!isLoading && ideasLoaded" :ideas="ideas"/>
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
      return this.$store.getters.isLoading;
    }
  },
  components: { IdeaList },
  activated() {
    const categoryIndex = this.$route.params.categoryId - 1;
    const title = this.$store.getters.categories[categoryIndex].categoryLbl;
    this.$store.dispatch('setTitle', title);

    this.ideas = this.$store.getters.categories[categoryIndex].items;
    this.ideasLoaded = true;
  },
  deactivated() {
    this.ideas = []
    this.ideasLoaded = false;
  }
};
</script>
