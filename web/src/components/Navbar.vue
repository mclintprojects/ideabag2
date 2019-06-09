<template>
  <div class="navbar">
    <div class="toolbar">
      <div class="toolbar-left">
        <button class="icon-button" v-if="!isRootComponent" @click="navigateAway" aria-label="Go back">
          <font-awesome-icon icon="arrow-left" size="lg" fixed-width></font-awesome-icon>
        </button>
        <h4 class="toolbar-left__title text--primary">{{title}}</h4>
      </div>
      <button class="icon-button" v-show="!bigScreen" @click="collapse = !collapse" aria-label="Open navigation menu">
        <font-awesome-icon icon="bars" size="2x" fixed-width></font-awesome-icon>
      </button>
    </div>

    <nav v-show="!collapse || bigScreen">
      <a
        v-if="this.$store.getters.userLoggedIn"
        href="#"
      >Welcome, {{this.$store.getters.userEmail}}!</a>
      <router-link to="/addidea">Submit idea</router-link>
      <router-link v-if="!this.$store.getters.userLoggedIn" to="/login">Log in</router-link>
      <router-link v-if="!this.$store.getters.userLoggedIn" to="/register">Join</router-link>
      <a v-if="this.$store.getters.userLoggedIn" href="#" @click="logout">Log out</a>
    </nav>
  </div>
</template>

<script>
import eventbus from '../eventbus';

export default {
  data() {
    return {
      isRootComponent: false,
      collapse: true,
      bigScreen: false
    };
  },
  computed: {
    title() {
      return this.$store.getters.navbarTitle;
    }
  },
  methods: {
    navigateAway() {
      this.$router.go(-1);
    },
    logout() {
      this.$store.dispatch('logout');
      eventbus.showToast("You're logged out now.", 'success');
    },
    isRootRoute() {
      const route = this.$route.path;
      return route === '/';
    },
    handleResize() {
      this.bigScreen = window.innerWidth >= 992;
    }
  },
  watch: {
    $route: function() {
      this.isRootComponent = this.isRootRoute();
    }
  },
  created() {
    this.isRootComponent = this.isRootRoute();
    this.handleResize();
    window.addEventListener('resize', this.handleResize);
  },
  destroyed() {
    window.removeEventListener('resize', this.handleResize);
  }
};
</script>

<style>
.navbar {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  background-color: var(--primary);
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
}

.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 5rem;
}

.toolbar-left {
  display: flex;
  align-items: center;
}

.toolbar-left__title {
  font-size: 1.8rem;
  margin-left: 1.6rem;
}

nav {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}

nav > a {
  color: var(--primaryText);
  text-decoration: none;
  font-size: 1.6rem;
  margin-left: 1.6rem;
  margin-bottom: 1.6rem;
}

nav > a:hover,
nav > a:focus {
  color: rgba(0, 0, 0, 0.54);
  text-decoration: none;
}

@media only screen and (min-width: 76.8rem) {
  .navbar {
    flex-direction: row;
  }

  nav {
    flex-direction: row;
    align-items: center;
    margin-right: 1.6rem;
  }

  nav > a {
    margin: 0 0 0 2.4rem;
    padding: 0;
  }
}
</style>
