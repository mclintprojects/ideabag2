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

let ideasURL =
  'https://docs.google.com/document/d/17V3r4fJ2udoG5woDBW3IVqjxZdfsbZC04G1A-It_DRU/export?format=txt';

export default {
  name: 'app',
  components: { Navbar },
  methods: {
    getData() {
      const ideasdb = localStorage.getItem('ideasdb');

      if (ideasdb) {
        this.$store.dispatch('setLoading', false);
        eventbus.showToast('Loaded offline cache.', 'info');
        return JSON.parse(ideasdb);
      }

      return [];
    },
    tryLocalLogin() {
      const loginData = localStorage.getItem('loginData');

      if (loginData) {
        this.$store.dispatch('loginUserLocal', JSON.parse(loginData));
      }
    },
    saveData(ideasdb) {
      localStorage.setItem('ideasdb', JSON.stringify(ideasdb));
    },
    setupInterceptors() {
      axios.interceptors.response.use(res => {
        if (res.status == 401) {
          eventbus.showToast(
            'Authorization token expired. Please login again.',
            'error',
            '5000'
          );
          this.$store.dispatch('logout');
        }

        return res;
      });
    },
    downloadIdeas() {
      axios.defaults.timeout = 12000;

      axios
        .get(ideasURL)
        .then(response => {
          response.data.forEach(c => {
            c.items.forEach(i => {
              i.progress = 'undecided';
              i.bookmarked = false;
            });
          });

          this.$store.dispatch('setCategories', response.data);
          this.$store.dispatch('setLoading', false);
          this.saveData(response.data);
        })
        .catch(() => {
          eventbus.showToast(
            "Couldn't load data. Please check your connection and reload.",
            'error',
            'long'
          );
        });
    },
    createDb(version = 1) {
      const request = indexedDB.open('userData', version);

      request.onupgradeneeded = event => {
        const db = event.target.result;
        const ideasStore = db.createObjectStore('ideas', { keyPath: 'id' });
        ideasStore.createIndex('bookmarked', 'bookmarked');
      };

      request.onsuccess = event => {
        const db = event.target.result;
        if (!db.objectStoreNames.contains('ideas'))
          this.createDb(db.version + 1);
        this.$store.dispatch('setUserDataDB', db);
      };
    }
  },
  created() {
    this.$store.dispatch('setCategories', this.getData());
    this.downloadIdeas();
    this.$store.dispatch('getNewIdeas');
    this.tryLocalLogin();
    this.setupInterceptors();
    this.createDb();
  }
};
</script>

<style>
:root {
  --primary: #ffa000;
  --primaryDark: #c67100;
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
  border: solid 0rem transparent;
  border-radius: 0.4rem;
  color: white;
  height: 4rem;
  min-width: 8rem;
  transition: all 1s;
  text-transform: uppercase;
  font-size: 1.4rem;
  padding: 0 1.6rem;
  font-weight: 600;
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
  padding: 1rem;
}

.container-main {
  display: flex;
  flex-direction: column;
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

.form-section__input {
  height: 3.5rem;
  border: 0.1rem solid whitesmoke;
  border-radius: 0.4rem;
  padding-left: 0.8rem;
  margin-bottom: 1.6rem;
  font-size: 1.4rem;
}

.form-section__input:focus {
  box-shadow: 0rem 0rem 0rem transparent;
}

.form-section__label {
  font-size: 1.6rem;
  margin-bottom: 0.4rem;
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

  .v-modal {
    top: 20rem !important;
  }
}
</style>
