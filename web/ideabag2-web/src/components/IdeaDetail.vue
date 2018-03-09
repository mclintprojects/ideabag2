<template>
  <div id="detailContainer">
      <div id="card" v-if="idea != null">
          <p id="ideaTitle">{{idea.title}}</p>
          <p id="ideaDescription">{{idea.description}}</p>
      </div>
  </div>
</template>

<script>
export default {
	props: ['idea']
};
</script>

<style scoped>
#detailContainer {
	padding: 0px 16px 16px 16px;
}

#card {
	border: 2px solid transparent;
	border-radius: 10px;
	background-color: var(--primary);
	padding: 16px;
}

#ideaTitle {
	color: rgba(0, 0, 0, 0.8);
	font-size: 22px;
	font-weight: bold;
}

#ideaDescription {
	color: rgba(0, 0, 0, 0.54);
	font-size: 16px;
	white-space: pre;
}
</style>


