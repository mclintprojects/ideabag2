<template>
  <div class="container-app  container-extra-padding">
    <div class="form-section">
      <label class="form-section__label text--secondary" for="creator">Your name</label>
      <input
        class="form-section__input"
        v-model="creator"
        type="text"
        id="creator">
    </div>
    <div class="form-section">
      <label class="form-section__label text--secondary" for="title">Enter your idea's title</label>
      <input
        class="form-section__input"
        v-model="title"
        type="text"
        id="title">
    </div>
    <div class="form-section">
      <label class="form-section__label text--secondary" for="category">Select your idea's category</label>
      <select
        class="form-section__select"
        v-model="category"
        name="category"
        id="category">
        <option disabled value="">Please select a category</option>
        <option v-for="category in $store.getters.categories" :key="category.id" :value="category['categoryLbl']">
          {{ category['categoryLbl'] }}
        </option>
      </select>
    </div>
    <div class="form-section">
      <label class="form-section__label text--secondary" for="description">Enter an in depth description of your idea</label>
      <textarea
        class="form-section__textarea"
        v-model="description"
        name="description"
        id="description"
        rows="4"></textarea>
    </div>
    <a class="button" id="submit-button" :href="`mailto:${emailTo}?subject=${subject}&body=${body}`">Submit Idea</a>
  </div>
</template>

<script>
  const IDEA_SUBMISSION_EMAIL = 'alansagh@gmail.com';

  export default {
    data() {
      return {
        emailTo: IDEA_SUBMISSION_EMAIL,
        currentDate: new Date(),
        creator: '',
        title: '',
        category: 'Numbers',
        description: ''
      }
    },
    computed: {
      subject() {
        return 'IdeaBag 2 Submission ' + this.currentDate.toISOString();
      },
      body() {
        return `
          Author: ${this.creator}%0A
          Category: ${this.category}%0A
          Title: ${this.title}%0A
          %0A
          Description:%0A
          ${this.description}
        `;
      }
    },
    activated() {
      this.$store.dispatch('setTitle', 'Submit idea');
    }
  };
</script>

<style scoped>
  #submit-button {
    margin-top: 1.6rem;
  }
</style>
