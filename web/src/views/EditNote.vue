<template>
  <div class="container-app container-extra-padding">
    <div class="form-section">
      <label class="form-section__label text--secondary" for="note">Enter a note</label>
      <textarea
        v-model="note"
        class="form-section__textarea"
        name="note"
        id="note"
        rows="4"></textarea>
    </div>
    <button class="button form__submit-button" @click="submit()">Save</button>
  </div>
</template>

<script>
  import UserDataDBInterface from '../mixins/UserDataDBInterface';

  export default {
    mixins: [UserDataDBInterface],
    data() {
      return {
        note: ''
      }
    },
    methods: {
      dataId(categoryId, ideaId) {
        return `${categoryId}C-${ideaId}I`;
      },
      submit() {
        const ideaId = this.dataId(this.$route.params.categoryId, this.$route.params.ideaId);
        this.saveNote(ideaId, this.note);
        this.$router.go(-1);
      }
    },
    activated() {
      this.$store.dispatch('setTitle', 'Edit note');
    }
  };
</script>
