<template>
  <div class="appContainer">
    <div class="form-group">
      <label for="emailTb">Email address</label>
      <input
        v-model="formData.email"
        id="emailTb"
        type="email"
        class="form-control"
        placeholder="Enter email"
      >
    </div>
    <div class="form-group">
      <label for="passwordTb">Password</label>
      <input
        v-model="formData.password"
        type="password"
        class="form-control"
        id="passwordTb"
        placeholder="Password"
      >
    </div>
    <button
      @click="registerUser"
      class="appBtn"
      :disabled="this.$store.getters.isPerformingAction"
    >Sign up</button>

    <font-awesome-icon
      id="loadingCircle"
      v-if="this.$store.getters.isPerformingAction"
      icon="spinner"
      size="3x"
      spin
      fixed-with
    ></font-awesome-icon>
  </div>
</template>

<script>
import eventbus from '../eventbus';

export default {
  data() {
    return {
      formData: {
        email: '',
        password: ''
      },
      errorCodes: {
        INVALID_PASSWORD: 'Your password needs to be longer.',
        EMAIL_EXISTS: 'A user with that email address already exists.',
        'TOO_MANY_ATTEMPTS_TRY_LATER:':
          'Too many attempts. Please try again in a few minutes.'
      }
    };
  },
  methods: {
    registerUser() {
      if (this.formData.email.length > 0 && this.formData.password.length > 0) {
        this.$store.dispatch('registerUser', this.formData);
      } else {
        eventbus.showToast('One or more required fields is empty.', 'error');
      }
    }
  },
  activated() {
    eventbus.$on('registration-success', message => {
      eventbus.showToast(message, 'success');
      this.$router.go(-1);
    });

    eventbus.$on('registration-failure', message => {
      if (this.errorCodes.hasOwnProperty(message))
        eventbus.showToast(this.errorCodes[message], 'error');
      else eventbus.showToast(message, 'error');
    });
  }
};
</script>

<style scoped>
label {
  color: var(--primaryText);
  font-family: "Roboto", sans-serif;
}

.appContainer {
  padding: 16px;
  padding-top: 50px;
}

.form-control {
  border: solid 0px transparent;
  border-radius: 2px;
  height: 40px;
  font-weight: normal;
}

.form-control:focus {
  box-shadow: 0px 0px 0px transparent;
}

#loadingCircle {
  position: initial;
  margin-left: 20px;
}
</style>
