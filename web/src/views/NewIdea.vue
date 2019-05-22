<template>
  <div class="container-app">
    <div>
      <label for="creator">Your name</label>
      <input
        v-model="creator"
        type="text"
        id="creator">
    </div>
    <div>
      <label for="title">Enter your idea's title</label>
      <input
        v-model="title"
        type="text"
        id="title">
    </div>
    <div>
      <label for="category">Select your idea's category</label>
      <select
        v-model="category"
        name="category"
        id="category">
        <option disabled value="">Please select a category</option>
        <option v-for="category in $store.getters.categories" :key="category.id" :value="category['categoryLbl']">
          {{ category['categoryLbl'] }}
        </option>
      </select>
    </div>
    <div>
      <label for="description">Enter an in depth description of your idea</label>
      <textarea
        v-model="description"
        name="description"
        id="description"
        rows="20"></textarea>
    </div>
    <a class="button" :href="'mailto:' + emailTo + '?subject=' + subject + '&body=' + body">Submit Idea</a>
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
  .container-app {
    color: var(--primaryText);
    font-size: 1.6rem;
    display: flex;
    flex-direction: column;
  }
  .container-app > * {
    font-size: var(--primaryTextSize);
  }
</style>
