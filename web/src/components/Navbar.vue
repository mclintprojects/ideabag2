<template>
  <div class="navbar">
    <div class="toolbar">
      <div class="toolbar-left">
        <button class="icon-button" v-if="!isRootComponent" @click="navigateAway" aria-label="Go back">
          <font-awesome-icon icon="arrow-left" size="lg" fixed-width></font-awesome-icon>
        </button>
        <h4 class="toolbar-left__title text--primary">{{title}}</h4>
      </div>
    </div>
    <div class="toolbar-right">
      <dropdown :visible="menuVisible" :position="['right', 'center', 'right', 'top']" @clickout="menuVisible = false">
        <button class="icon-button" @click="menuVisible = true" aria-label="Open menu">
          <font-awesome-icon icon="ellipsis-v" size="lg" fixed-width></font-awesome-icon>
        </button>
        <nav slot="dropdown" class="dropdown-menu" @click="menuVisible = false">
          <a
            v-if="this.$store.getters.userLoggedIn"
            href="#"
          >Welcome, {{this.$store.getters.userEmail}}!</a>
          <router-link v-if="!this.$store.getters.userLoggedIn" to="/login">Log in</router-link>
          <router-link v-if="!this.$store.getters.userLoggedIn" to="/register">Join</router-link>
          <router-link to="/addidea">Submit idea</router-link>
          <button v-if="this.$store.getters.userLoggedIn" @click="logout">Log out</button>
        </nav>
      </dropdown>
    </div>
  </div>
</template>

<script>
import eventbus from '../eventbus';
import dropdown from 'vue-my-dropdown';

export default {
  components: {
    dropdown
  },
  data() {
    return {
      isRootComponent: false,
      menuVisible: false
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
    }
  },
  watch: {
    $route: function() {
      this.isRootComponent = this.isRootRoute();
    }
  },
  created() {
    this.isRootComponent = this.isRootRoute();
  }
};
</script>

<style>
.navbar {
  display: flex;
  flex-direction: row;
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
</style>
