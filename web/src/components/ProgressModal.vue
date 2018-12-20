<template>
  <modal
    :name="'progress-modal-' + id"
    height="auto"
    :adaptive="true"
    :classes="['v--modal', 'progress-modal']"
    @opened="updateProgressRadiobuttons"
  >
    <h3>Set idea progress</h3>
    <ul class="modal-list">
      <li
        class="modal-list-item"
        @click.stop="chooseProgress(progressState)"
        v-for="(progressState, index) in ['done', 'in-progress', 'undecided']"
        :key="index"
      >
        <input
          class="modal-list-item__field"
          v-model="progress"
          :id="'progress-' + progressState"
          type="radio"
          name="progress"
          :value="progressState"
        >
        <label
          class="modal-list-item__label"
          :for="'progress-' + progressState"
        >{{ progressState | variableToInterfaceFriendly }}</label>
      </li>
    </ul>
  </modal>
</template>

<script>
export default {
  props: {
    id: {
      type: String,
      default: '0'
    },
    progress: {
      type: String,
      required: true
    }
  },
  methods: {
    updateProgressRadiobuttons() {
      const radiobuttons = document.getElementsByClassName(
        'progress-radiobutton'
      );
      for (let i = 0; i < radiobuttons.length; i++) {
        if (radiobuttons[i].value === this.progress) {
          radiobuttons[i].checked = true;
        } else {
          radiobuttons[i].checked = false;
        }
      }
    },
    chooseProgress(progress) {
      this.$emit('update-progress', progress);
      this.$modal.hide('progress-modal-' + this.id);
    }
  },
  deactivated() {
    this.$modal.hide('progress-modal-' + this.id);
  },
  filters: {
    variableToInterfaceFriendly(value) {
      value = value.replace('-', ' ');
      value = value.charAt(0).toUpperCase() + value.slice(1);
      return value;
    }
  }
};
</script>

<style>
.progress-modal > h3 {
  text-align: center;
  font-size: 1.6rem;
  color: rgba(0, 0, 0, 0.54);
  padding: 1.6rem 0;
}
</style>
