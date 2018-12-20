<template>
  <div class="container-app container-extra-padding">
    <div class="form-section">
      <label class="form-section__label text--secondary" for="emailTb">Email address</label>
      <input
        class="form-section__input"
        v-model="formData.email"
        id="emailTb"
        type="email"
        placeholder="Enter email"
      >
    </div>
    <div class="form-section">
      <label class="form-section__label text--secondary" for="passwordTb">Password</label>
      <input
        v-model="formData.password"
        type="password"
        class="form-section__input"
        id="passwordTb"
        placeholder="Password"
      >
    </div>
    <button
      @click="registerUser"
      class="button"
      :disabled="this.$store.getters.isPerformingAction"
    >Sign up</button>

    <font-awesome-icon
      class="loader"
      v-if="this.$store.getters.isPerformingAction"
      icon="spinner"
      size="3x"
      spin
      fixed-width
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
    this.$store.dispatch('setTitle', 'Join');
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
