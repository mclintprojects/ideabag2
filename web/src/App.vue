<template>
  <div class="container-main">
    <navbar></navbar>
    <transition name="slide" mode="out-in">
      <keep-alive>
        <router-view></router-view>
      </keep-alive>
    </transition>
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
  --badgePadding: 0.8rem;
  --ideaDescriptionTextSize: 1.6rem;
  --badgeTextSize: 1.2rem;
  --avatarSize: 3rem;
  --commentPadding: 1.6rem;
  --cardMargin: 1.6rem 0rem 0rem 0rem;
  --authorLblSize: 1.6rem;
  --dateLblSize: 1.3rem;
  --dateLblMargin: 3.2rem;
}

* {
  font-family: "Roboto", sans-serif;
  font-size: 60%;
  margin: 0;
  padding: 0;
}

html,
body {
  height: 100%;
  margin: 0;
  padding: 0px;
}

body {
  background-color: var(--background);
}

p {
  font-size: 1.4rem;
}

#loader {
  width: 3.6rem;
  height: 3.6rem;
  color: white;
  position: absolute;
  left: calc(50% - 3.6rem);
  top: calc(50% - 3.6rem);
}

.button {
  background-color: var(--primary);
  border: solid 0rem transparent;
  border-radius: 0.2rem;
  color: white;
  height: 4rem;
  min-width: 10rem;
  transition: all 1s;
  text-transform: uppercase;
  font-size: 1.4rem;
}

.button:hover {
  background-color: var(--primaryDark);
  color: white;
  cursor: pointer;
}

.button:disabled {
  background-color: gray;
  color: black;
}

.button:disabled:hover {
  cursor: not-allowed;
}

.button--outlined {
  background-color: transparent;
  border: solid 0.2rem white;
  border-radius: 0.2rem;
  color: white;
  margin: 0.5rem;
  height: 4rem;
}

.button--outlined:hover {
  background-color: white;
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
  flex-flow: column;
  justify-content: center;
  height: 100%;
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

.modal-list > li {
  border-top: 0.1rem solid black;
  cursor: pointer;
  font-size: 1.7rem;
  list-style-type: none;
  padding: 2rem 3rem;
  width: 100%;
}

.modal-list > li:hover {
  background-color: rgba(0, 0, 0, 0.2);
}

.modal-list > li > input,
.modal-list > li > label {
  cursor: pointer;
}

.modal-list > li > label {
  padding-left: 1rem;
}

.no-ideas-to-display {
  color: var(--primaryTextLight);
  display: flex;
  flex-flow: column nowrap;
  height: 100%;
  justify-content: center;
  align-items: center;
}

.component-holder {
  margin-top: 7rem;
}

.container-app {
  height: 100%;
  width: 55%;
  margin: 0 auto;
  padding-top: 5rem;
}

.container-full {
  height: 100%;
  width: 100%;
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
  box-shadow: 0px 0px 0px transparent;
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

@media screen and (max-width: 57.6rem), (max-width: 76.8rem) {
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
}
</style>
