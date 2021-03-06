<template>
  <div class="container-main">
    <navbar></navbar>
    <main>
      <transition name="slide" mode="out-in">
        <keep-alive>
          <router-view></router-view>
        </keep-alive>
      </transition>
    </main>
  </div>
</template>

<script>
import Navbar from './components/Navbar';
import axios from 'axios';
import eventbus from './eventbus';

const ideasURL =
  'https://docs.google.com/document/d/17V3r4fJ2udoG5woDBW3IVqjxZdfsbZC04G1A-It_DRU/export?format=txt';
const indexedDBVersion = 1;

export default {
  name: 'app',
  components: { Navbar },
  methods: {
    loadLocalIdeaData() {
      const ideasdb = localStorage.getItem('ideasdb');
      if (ideasdb) {
        const ideaData = this.initializeCategories(JSON.parse(ideasdb));

        this.$store.dispatch('setCategories', ideaData);
        this.$store.dispatch('setLoading', false);
        eventbus.showToast('Loaded offline cache.', 'info');
      }
    },
    loadUserData() {
      const request = indexedDB.open('userData', indexedDBVersion);

      request.onupgradeneeded = this.createIndexedDB;
      request.onsuccess = event => {
        const db = event.target.result;
        // if (!db.objectStoreNames.contains('ideas'))
        //   this.createDb(db.version + 1);
        this.$store.dispatch('setUserDataDB', db);

        const ideasStore = db
          .transaction(['ideas'], 'readonly')
          .objectStore('ideas');
        ideasStore.openCursor().onsuccess = event => {
          const cursor = event.target.result;
          if (cursor) {
            const [categoryID, itemID] = cursor.value.id.replace('C', '').replace('I', '').split('-');
            this.$store.getters.categories[categoryID - 1].items[itemID - 1].progress = cursor.value.progress;
            this.$store.getters.categories[categoryID - 1].items[itemID - 1].bookmarked = cursor.value.bookmarked === 1;
            this.$store.getters.categories[categoryID - 1].items[itemID - 1].note = cursor.value.note;
            cursor.continue();
          }
        }
      };
    },
    createIndexedDB(event) {
      const db = event.target.result;
      const ideasStore = db.createObjectStore('ideas', { keyPath: 'id' });
      ideasStore.createIndex('bookmarked', 'bookmarked');
    },
    async downloadUpdatedIdeaData() {
      axios.defaults.timeout = 12000;

      try {
        const {data} = await axios.get(ideasURL);

        let categories = this.$store.getters.categories;
        if (categories.length === 0) {
          categories = this.initializeCategories(data);
        } else {
          for (const category of data) {
            // if the category is new
            if (categories.length < category.id - 1) {
              categories.push(category);
              categories[category.id - 1].items = this.initializeItems();
            } else {
              for (const item of category.items) {
                // if the item is new
                if (category.items.length < item.id - 1) {
                  categories[category.id - 1].items.push(item);
                  categories[category.id - 1].items[item.id - 1] = this.initializeItem(item);
                } else {
                  categories[category.id - 1].items[item.id - 1].title = item.title;
                  categories[category.id - 1].items[item.id - 1].difficulty = item.difficulty;
                  categories[category.id - 1].items[item.id - 1].description = item.description;
                }
              }
            }
          }
        }
        this.$store.dispatch('setCategories', categories);
        this.$store.dispatch('setLoading', false);
        this.saveDataLocally(data);
      } catch(error) {
        eventbus.showToast("Couldn't load data. Please check your connection and reload.", 'error', 'long')
      }
    },
    initializeCategories(categories) {
      for (let categoryIndex = 0; categoryIndex < categories.length; categoryIndex++) {
        categories[categoryIndex].items = this.initializeItems(categories[categoryIndex].items);
      }
      return categories;
    },
    initializeItems(items) {
      for (let itemIndex = 0; itemIndex < items.length; itemIndex++) {
        items[itemIndex] = this.initializeItem(items[itemIndex]);
      }
      return items;
    },
    initializeItem(item) {
      return {
        ...item,
        progress: 'undecided',
        bookmarked: false,
        note: ''
      }
    },
    tryLocalLogin() {
      const loginData = localStorage.getItem('loginData');

      if (loginData) {
        this.$store.dispatch('loginUserLocal', JSON.parse(loginData));
      }
    },
    saveDataLocally(ideasdb) {
      localStorage.setItem('ideasdb', JSON.stringify(ideasdb));
    },
    setupInterceptors() {
      axios.interceptors.response.use(res => {
        if (res.status === 401) {
          eventbus.showToast(
            'Authorization token expired. Please login again.',
            'error',
            '5000'
          );
          this.$store.dispatch('logout');
        }

        return res;
      });
    }
  },
  created() {
    this.loadLocalIdeaData();
    this.loadUserData();
    this.downloadUpdatedIdeaData();
    this.$store.dispatch('getNewIdeas');
    this.tryLocalLogin();
    this.setupInterceptors();
  }
};
</script>

<style>
@import url('https://fonts.googleapis.com/css?family=Sarabun&display=swap');

:root {
  --primary: #ffa000;
  --primaryDark: #c67100;
  --primaryLight: #fcc35f;
  --background: #37474f;
  --highlight: #2c393f;
  --undecided: #bababa;
  --in-progress: #f9bf3b;
  --done: #2ecc71;
  --primaryText: rgba(255, 255, 255, 0.8);
  --primaryTextLight: rgba(255, 255, 255, 0.54);
  --progress-bar-background: #2b2b2b;
  --primaryTextSize: 1.8rem;
  --ideaTextSize: 2.2rem;
  --categoryIconSize: 3.6rem;
  --categoryIconBgSize: 7.2rem;
  --badgePadding: 0.4rem 0.8rem;
  --ideaDescriptionTextSize: 1.6rem;
  --ideaNoteTextSize: 1.6rem;
  --badgeTextSize: 1rem;
  --avatarSize: 3rem;
  --commentPadding: 1.6rem;
  --cardMargin: 1.6rem 0rem 0rem 0rem;
  --authorLblSize: 1.6rem;
  --dateLblSize: 1.3rem;
  --dateLblMargin: 1.6rem;
  --buttonOutlinedHover: #ffa20060;
}

* {
  box-sizing: border-box;
  font-family: "Sarabun", sans-serif;
  font-size: 60%;
  margin: 0;
  padding: 0;
}

html,
body {
  height: 100%;
}

body {
  background-color: var(--background);
}

p {
  font-size: 1.4rem;
}

button {
  cursor: pointer;
}

select > option {
  font-size: 1.6rem;
}

main {
  padding-top: 5rem;
  flex-grow: 2;
}

.loader {
  width: 3.6rem;
  height: 3.6rem;
  color: white;
  position: absolute;
  left: calc(50% - 3.6rem);
  top: calc(50% - 3.6rem);
  font-size: 1.8rem;
}

.button {
  background-color: var(--primary);
  border: solid 0 transparent;
  border-radius: 0.4rem;
  color: white;
  height: 4rem;
  min-width: 8rem;
  transition: all 1s;
  text-transform: uppercase;
  font-size: 1.4rem;
  padding: 0 1.6rem;
  font-weight: 600;
  text-decoration: none;
  display: flex;
  align-items: center;
  justify-content: center;
}

.button:hover {
  background-color: var(--primaryDark);
  color: white;
  cursor: pointer;
}

.button:disabled {
  background-color: gray;
  color: #9a9a9a;
}

.button:disabled:hover {
  cursor: not-allowed;
}

.button--outlined {
  background-color: transparent;
  border: solid 0.1rem var(--primary);
  color: var(--primary);
  margin: 0.5rem;
  height: 4rem;
}

.button--outlined:hover {
  background-color: var(--buttonOutlinedHover);
  color: var(--primary);
}

.floating-action-button {
  border-radius: 50%;
  bottom: 1.6rem;
  display: flex;
  justify-content: center;
  align-items: center;
  width: 5.6rem;
  height: 5.6rem;
  position: fixed;
  right: 1.6rem;
  font-size: 1.4rem;
  min-width: 0;
  padding: 0;
  margin: 0;
}

.icon-button {
  border: none;
  background-color: transparent;
  cursor: pointer;
  color: white;
  font-size: 1.2rem;
  padding: 1.6rem;
}

.container-main {
  display: flex;
  min-height: 100%;
}

.text--primary {
  color: var(--primaryText);
}

.text--secondary {
  color: var(--primaryTextLight);
}

.modal-list {
  padding: 0;
  margin: 0;
}

.modal-list-item {
  border-top: 0.1rem solid rgb(192, 192, 192);
  cursor: pointer;
  list-style-type: none;
  padding: 3rem 3rem;
  width: 100%;
}

.modal-list-item:hover {
  background-color: rgba(218, 218, 218, 0.2) !important;
}

.modal-list-item__field,
.modal-list-item__label {
  cursor: pointer;
  color: rgba(0, 0, 0, 0.8);
  font-size: 1.4rem;
}

.modal-list-item__label {
  padding-left: 1rem;
}

.no-ideas-to-display {
  color: var(--primaryTextLight);
  display: flex;
  flex-flow: column nowrap;
  height: 100%;
  justify-content: center;
  align-items: center;
  font-size: 1.5rem;
}
.no-ideas-to-display > h2 {
  margin-top: 1rem;
  font-size: 3rem;
}

.container-app {
  height: 100%;
  min-height: 100%;
  width: 55%;
  margin: 0 auto;
}

.container-full {
  height: 100%;
  min-height: 100%;
  width: 100%;
}

.container-extra-padding {
  padding-top: 10rem;
}

.form-section {
  display: flex;
  flex-direction: column;
}

.form-section__input, .form-section__textarea, .form-section__select {
  border: 0.1rem solid whitesmoke;
  border-radius: 0.4rem;
  padding-left: 0.8rem;
  margin-bottom: 1.6rem;
  font-size: 1.4rem;
}

.form-section__input, .form-section__select {
  height: 3.5rem;
}

.form-section__input:focus {
  box-shadow: 0 0 0 transparent;
}

.form-section__textarea {
  resize: none;
  padding-top: 1rem;
}

.form-section__label {
  font-size: 1.6rem;
  margin-bottom: 0.8rem;
}

.form__submit-button {
  margin-top: 1.6rem;
}

.highlight {
  background-color: var(--highlight);
}

.slide-enter-active {
  animation: slide-in 200ms ease-out forwards;
}

.slide-leave-active {
  animation: slide-out 200ms ease-out forwards;
}

.dropdown-menu {
  display: flex;
  flex-direction: column;
  background-color: #fafafa;
  color: #212121;
  box-shadow: rgb(58, 58, 58) 0 0 6px 0;
}

.dropdown-menu > button, .dropdown-menu > a {
  width: 16rem;
  font-size: 1.6rem;
  color: rgba(0, 0, 0, 0.8);
  background-color: transparent;
  border: none;
  padding: 1rem;
  text-decoration: none;
  text-align: left;
}

.dropdown-menu > button:hover, .dropdown-menu > a:hover {
  background-color: rgba(0, 0, 0, 0.2);
}

@keyframes slide-in {
  from {
    transform: translateX(-10rem);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

@keyframes slide-out {
  from {
    transform: translateX(0);
    opacity: 1;
  }
  to {
    transform: translateX(-10rem);
    opacity: 0;
  }
}

@media screen and (max-width: 76.8rem) {
  .container-app {
    width: 100% !important;
  }

  :root {
    --primaryTextSize: 1.6rem;
    --ideaTextSize: 1.8rem;
    --ideaDescriptionTextSize: 1.3rem;
    --categoryIconSize: 2.8rem;
    --categoryIconBgSize: 5.6rem;
    --badgePadding: 0.4rem;
    --badgeTextSize: 1rem;
    --avatarSize: 2.4rem;
    --commentPadding: 0.8rem;
    --cardMargin: 1.6rem 1.6rem 0rem 1.6rem;
    --authorLblSize: 1.3rem;
    --dateLblSize: 1rem;
    --dateLblMargin: 1.6rem;
  }

  .button--delete {
    width: 10.4rem;
  }

  .form-section, .form__submit-button {
    margin: 0 1.6rem;
  }

  .v-modal {
    top: 20rem !important;
  }
}
</style>
