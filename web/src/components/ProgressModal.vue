<template>
  <modal :name="'progress-modal-' + id" height="auto" :adaptive="true" :classes="['v--modal', 'progress-modal']" @opened='updateProgressRadiobuttons'>
    <h3>Set idea progress</h3>
    <ul class="progress-list">
      <li @click.stop="chooseProgress(progressState)" v-for="(progressState, index) in ['done', 'in-progress', 'undecided']" :key="index">
        <input v-model="progress" :id="'progress-' + progressState" class="progress-radiobutton" type="radio" name="progress" :value="progressState" />
        <label :for="'progress-' + progressState"> {{ progressState | variableToInterfaceFriendly }}</label>
      </li>
    </ul>
  </modal>
</template>

<script>
export default {
  props: {
    id: {
      type: Number,
      default: 1
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
  filters: {
    variableToInterfaceFriendly(value) {
      value = value.replace('-', ' ')
      value = value.charAt(0).toUpperCase() + value.slice(1);
      return value;
    }
  }
};
</script>

<style>
.progress-modal > h3 {
	text-align: center;
}
</style>

<style scoped>
.progress-list {
	padding: 0;
	margin: 0;
}
.progress-list > li {
	border-top: 1px solid black;
	cursor: pointer;
	font-size: 1.7rem;
	list-style-type: none;
	padding: 2rem 3rem;
	width: 100%;
}
.progress-list > li:hover {
	background-color: rgba(0, 0, 0, 0.2);
}
.progress-list > li > input,
.progress-list > li > label {
	cursor: pointer;
}
</style>
