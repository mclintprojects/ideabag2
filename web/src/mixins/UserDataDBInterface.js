export default {
  computed: {
    userDataDB() {
      return this.$store.getters.userDataDB;
    }
  },
  methods: {
    getIdeaIndex(ideaId) {
      return ideaId.replace('C', '').replace('I', '').split('-')[1] - 1;
    },
    getCategoryIndex(ideaId) {
      return ideaId.replace('C', '').replace('I', '').split('-')[0] - 1;
    },
    addToBookmarks(ideaId) {
      this.$store.getters.categories[this.getCategoryIndex(ideaId)].items[this.getIdeaIndex(ideaId)].bookmarked = true;

			const objectStore = this.userDataDB
				.transaction(['ideas'], 'readwrite')
				.objectStore('ideas');
			objectStore.get(ideaId).onsuccess = event => {
				if (event.target.result === undefined) {
					this.addIdeaToIndexedDB({ideaId, bookmarked: true});
				} else {
					event.target.result.bookmarked = 1;
					objectStore.put(event.target.result)
				}
			};
		},
    removeFromBookmarks(ideaId) {
      this.$store.getters.categories[this.getCategoryIndex(ideaId)].items[this.getIdeaIndex(ideaId)].bookmarked = false;

			const objectStore = this.userDataDB
				.transaction(['ideas'], 'readwrite')
				.objectStore('ideas');
			objectStore.get(ideaId).onsuccess = event => {
				event.target.result.bookmarked = 0;
				objectStore.put(event.target.result)
			};
		},
    addIdeaToIndexedDB({ideaId, bookmarked=false, progress='undecided', note=''}) {
			const bookmarkedBinary = bookmarked ? 1 : 0;
			this.userDataDB
				.transaction(['ideas'], 'readwrite')
				.objectStore('ideas')
				.add({
					id: ideaId,
					bookmarked: bookmarkedBinary,
					progress,
          note
				})
    },
    updateProgress(ideaId, progress) {
      this.$store.getters.categories[this.getCategoryIndex(ideaId)].items[this.getIdeaIndex(ideaId)].progress = progress;

			const objectStore = this.userDataDB
				.transaction(['ideas'], 'readwrite')
				.objectStore('ideas');
			objectStore.get(ideaId).onsuccess = event => {
				if (event.target.result === undefined) {
					this.addIdeaToIndexedDB({ideaId, progress});
				} else {
					event.target.result.progress = progress;
					objectStore.put(event.target.result)
				}
			};
		},
    saveNote(ideaId, note) {
      this.$store.getters.categories[this.getCategoryIndex(ideaId)].items[this.getIdeaIndex(ideaId)].note = note;

      const objectStore = this.userDataDB
        .transaction(['ideas'], 'readwrite')
        .objectStore('ideas');
      objectStore.get(ideaId).onsuccess = event => {
        if (event.target.result === undefined) {
          this.addIdeaToIndexedDB({ideaId, note});
        } else {
          event.target.result.note = note;
          objectStore.put(event.target.result);
        }
      }
    }
  }
}
