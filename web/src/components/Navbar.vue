<template>
  <div id="navbar">
    <div id="toolbar">
      <div id="left-toolbar-items">
        <button class="icon-button" v-if="!isRootComponent" @click="navigateAway">
          <font-awesome-icon icon="arrow-left" size="lg" fixed-width></font-awesome-icon>
        </button>
        <h4 id="title">{{title}}</h4>
      </div>
      <button class="icon-button" v-show="!bigScreen" @click="collapse = !collapse">
        <font-awesome-icon icon="bars" size="2x" fixed-width></font-awesome-icon>
      </button>
    </div>

    <nav v-show="!collapse || bigScreen">
      <a v-if="this.$store.getters.userLoggedIn" href="#">Welcome, {{this.$store.getters.userEmail}}!</a>
      <router-link v-if="!this.$store.getters.userLoggedIn" to="/login">Login</router-link>
      <router-link v-if="!this.$store.getters.userLoggedIn" to="/register">Signup</router-link>
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
      if (route == '/') return true;
      else return false;
    },
    handleResize(e) {
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
#navbar {
  background-color: var(--primary);
  display: flex;
  flex-flow: column;
  justify-content: space-between;
  position: fixed;
  top: 0px;
  width: 100%;
  padding: 0 1rem;
}
#toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 50px;
}
#left-toolbar-items {
  display: flex;
}
#title {
  margin-left: 16px;
  color: rgba(255, 255, 255, 0.8);
}
nav {
  display: flex;
  flex-flow: column wrap;
  align-items: flex-start;
}
nav > a {
  color: var(--primaryText);
  padding: 1rem;
}
nav > a:hover, nav > a:focus {
  color: rgba(0, 0, 0, 0.54);
  text-decoration: none;
}

@media only screen and (min-width: 768px) {
  #navbar {
    flex-flow: row;
  }
  nav {
    flex-flow: row;
    align-items: center;
  }
  nav > a {
    padding: 0 1.5rem;
  }
}
</style>
