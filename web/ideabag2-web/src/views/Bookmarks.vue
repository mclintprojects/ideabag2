<template>
  <div class="appContainer">
    <img v-if="isLoading" id="loadingCircle" src="https://samherbert.net/svg-loaders/svg-loaders/oval.svg" />
    <div id="no-bookmarks" v-if="!isLoading && ideas.length == 0">
      <img src="../../static/img/outline-bookmark-100px-grey.svg" alt="" />
      <h2>No bookmarks to show</h2>
      <p>Ideas that you bookmark will show up here</p>
    </div>
    <idea-list v-if="ideas.length > 0" :ideas="ideas" />
  </div>
</template>

<script>
import IdeaList from "../components/IdeaList";

export default {
  data() {
    return {
      ideas: [],
    };
  },
  computed: {
    isLoading() {
			return this.$store.getters.categories.length == 0;
		},
    userDataDB() {
			return this.$store.getters.userDataDB;
		},
  },
  watch: {
    userDataDB(db) {
      if (db !== undefined) {
        this.loadIdeas();
      }
    }
  },
  methods: {
    loadIdeas() {
      this.ideas = [];
      this.userDataDB.transaction(["bookmarks"]).objectStore("bookmarks").openCursor().onsuccess = (event) => {
        const cursor = event.target.result;
        if (cursor) {
          this.loadIdea(cursor.value.ideaId);
          cursor.continue();
        }
      }
    },
    loadIdea(id) {
      const categoryId = id[0];
      const ideaId = id[3] - 1;
      this.ideas.push(this.$store.getters.categories[categoryId].items[ideaId]);
    }
  },
  components: {
    "idea-list": IdeaList
  },
  activated() {
    this.$store.dispatch('setTitle', "Bookmarks");
    if (this.userDataDB !== undefined) {
      this.loadIdeas();
    }
  }
};
</script>

<style scoped>
#no-bookmarks {
  color: var(--primaryTextLight);
  display: flex;
  flex-flow: column nowrap;
  height: 100%;
  justify-content: center;
  text-align: center;
}
</style>
