<template>
  <modal :name="'progress-modal-' + id" height="auto" :adaptive="true" :classes="['v--modal', 'progress-modal']" @opened='updateProgressRadiobuttons'>
    <h3>Set idea progress</h3>
    <ul class="progress-list">
      <li @click.stop="$emit('update-progress', 'done')">
        <input v-model="progress" id="progress-done" class="progress-radiobutton" type="radio" name="progress" value="done" />
        <label for="progress-done">Done</label>
      </li>
      <li @click.stop="$emit('update-progress', 'in-progress')">
        <input v-model="progress" id="in-progress" class="progress-radiobutton" type="radio" name="progress" value="in-progress" />
        <label for="in-progress">In Progress</label>
      </li>
      <li @click.stop="$emit('update-progress', 'undecided')">
        <input v-model="progress" id="progress-undecided" class="progress-radiobutton" type="radio" name="progress" value="undecided" checked/>
        <label for="progress-undecided">Undecided</label>
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
