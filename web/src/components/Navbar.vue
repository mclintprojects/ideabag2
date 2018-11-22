<template>
    <div>
        <nav class="navbar navbar-default navbar-fixed-top">
            <div class="container-fluid">
                <div class="navbar-header">
                    <button class="navbar-toggle" @click="collapse = !collapse">
                        <span class="sr-only">Toggle navigation</span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                    <div id="toolbar" class="navbar-brand">
                        <img v-if="!isRootComponent" @click="navigateAway" id="backBtn" src="https://res.cloudinary.com/mclint-cdn/image/upload/v1523221457/ic_arrow_back_white_24px.svg" />
                        <h4>{{title}}</h4>
                    </div>
                </div>

                <div class="navbar-collapse" :class="{ 'collapse': collapse}">
                    <ul id="links" class="nav navbar-nav navbar-right">
                        <li v-if="this.$store.getters.userLoggedIn">
                            <a href="#">Welcome, {{this.$store.getters.userEmail}}!</a>
                        </li>
                        <li v-if="!this.$store.getters.userLoggedIn">
                            <router-link to="/login">Login</router-link>
                        </li>
                        <li v-if="!this.$store.getters.userLoggedIn">
                            <router-link to="/register">Signup</router-link>
                        </li>
                        <li v-if="this.$store.getters.userLoggedIn">
                            <a href="#" @click="logout">Log out</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </div>
</template>

<script>
import eventbus from '../eventbus';

export default {
  data() {
    return {
      isRootComponent: false,
      collapse: true
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
  background-color: var(--primary);
  border-radius: 0px;
}

.navbar-default {
  border-color: transparent;
}

#toolbar {
  display: flex;
  align-items: center;
}

#toolbar h4 {
  margin-left: 16px;
  color: rgba(255, 255, 255, 0.8);
}

#links > li > a {
  color: var(--primaryText);
}

#links > li > a:hover {
  color: rgba(0, 0, 0, 0.54);
}
</style>
