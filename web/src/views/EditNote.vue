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
        note: '',
        categoryId: 1,
        ideaId: 1,
        dataId: '1C-1I'
      }
    },
    methods: {
      submit() {
        this.saveNote(this.dataId, this.note);
        this.$router.go(-1);
      }
    },
    activated() {
      this.$store.dispatch('setTitle', 'Edit note');

      const categoryId = this.$route.params.categoryId;
      const ideaId = this.$route.params.ideaId;
      this.dataId = `${categoryId}C-${ideaId}I`;
      this.note = this.$store.getters.categories[categoryId - 1].items[ideaId - 1].note;
    }
  };
</script>
