export default {
  computed: {
    userDataDB() {
      return this.$store.getters.userDataDB;
    }
  },
  methods: {
    addToBookmarks(ideaId) {
      const [categoryId, itemId] = ideaId.replace('C', '').replace('I', '').split('-');
      this.$store.getters.categories[categoryId - 1].items[itemId - 1].bookmarked = true;

			const objectStore = this.userDataDB
				.transaction(['ideas'], 'readwrite')
				.objectStore('ideas');
			objectStore.get(ideaId).onsuccess = event => {
				if (event.target.result === undefined) {
					this.addIdeaToIndexedDB(ideaId, true, 'undecided');
				} else {
					event.target.result.bookmarked = 1;
					objectStore.put(event.target.result)
				}
			};
		},
    removeFromBookmarks(ideaId) {
      const [categoryId, itemId] = ideaId.replace('C', '').replace('I', '').split('-');
      this.$store.getters.categories[categoryId - 1].items[itemId - 1].bookmarked = false;

			const objectStore = this.userDataDB
				.transaction(['ideas'], 'readwrite')
				.objectStore('ideas');
			objectStore.get(ideaId).onsuccess = event => {
				event.target.result.bookmarked = 0;
				objectStore.put(event.target.result)
			};
		},
    addIdeaToIndexedDB(ideaId, bookmarked, progress) {
			const bookmarkedBinary = bookmarked ? 1 : 0;
			this.userDataDB
				.transaction(['ideas'], 'readwrite')
				.objectStore('ideas')
				.add({
					id: ideaId,
					bookmarked: bookmarkedBinary,
					progress: progress
				})
    },
    updateProgress(ideaId, progress) {
      const [categoryId, itemId] = ideaId.replace('C', '').replace('I', '').split('-');
      this.$store.getters.categories[categoryId - 1].items[itemId - 1].progress = progress;

			const objectStore = this.userDataDB
				.transaction(['ideas'], 'readwrite')
				.objectStore('ideas');
			objectStore.get(ideaId).onsuccess = event => {
				if (event.target.result === undefined) {
					this.addIdeaToIndexedDB(ideaId, false, progress);
				} else {
					event.target.result.progress = progress;
					objectStore.put(event.target.result)
				}
			};
		},
    saveNote(ideaId, note) {

    }
  }
}
