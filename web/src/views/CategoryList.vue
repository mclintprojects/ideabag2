<template>
	<div class="full-width-container">
		<font-awesome-icon id="loadingCircle" v-if="$store.getters.isLoading" icon="spinner" size="3x" spin fixed-with></font-awesome-icon>
		<div class="appContainer">
			<ul id="categoryList">
				<li v-for="(category, index) in categories" :key="index" @click="notifyCategoryClicked(index)" :class="{highlight: index == selectedIndex}">
					<div class="categoryItem">
						<div class="categoryIconBg">
							<img class="categoryIcon" :src="icons[index]" />
						</div>
						<div class="categoryContent">
							<p id="categoryTitle" class="primaryLbl">{{category.categoryLbl}}</p>
							<p class="secondaryLbl">Ideas: {{category.categoryCount}}</p>
						</div>
					</div>
				</li>
			</ul>
		</div>

		<router-link class="appBtn floating-action-button" to="bookmarks"><font-awesome-icon :icon="['fas', 'bookmark']" size="lg" fixed-width></font-awesome-icon></router-link>
	</div>
</template>

<script>
export default {
  data() {
    return {
      icons: [
        'https://res.cloudinary.com/mclint-cdn/image/upload/v1523221458/numbers.png',
        'https://res.cloudinary.com/mclint-cdn/image/upload/v1523221459/text.png',
        'https://res.cloudinary.com/mclint-cdn/image/upload/v1523221458/network.png',
        'https://res.cloudinary.com/mclint-cdn/image/upload/v1523221457/enterprise.png',
        'https://res.cloudinary.com/mclint-cdn/image/upload/v1523221457/cpu.png',
        'https://res.cloudinary.com/mclint-cdn/image/upload/v1523221459/web.png',
        'https://res.cloudinary.com/mclint-cdn/image/upload/v1523221457/file.png',
        'https://res.cloudinary.com/mclint-cdn/image/upload/v1523221457/database.png',
        'https://res.cloudinary.com/mclint-cdn/image/upload/v1523221458/multimedia.png',
        'https://res.cloudinary.com/mclint-cdn/image/upload/v1523221457/games.png'
      ],
      selectedIndex: 0
    };
  },
  computed: {
    categories() {
      return this.$store.getters.categories;
    }
  },
  methods: {
    notifyCategoryClicked(index) {
      this.selectedIndex = index;
      this.$router.push({
        name: 'categories',
        params: { categoryId: index + 1 }
      });
    }
  },
  activated() {
    this.$store.dispatch('setSelectedIdeaIndex', -1);
    this.$store.dispatch('setTitle', 'IdeaBag 2');
  }
};
</script>

<style scoped>
.categoryIcon {
  width: var(--categoryIconSize);
}

.categoryIconBg {
  background: rgba(0, 0, 0, 0.8);
  display: flex;
  justify-content: center;
  align-items: center;
  width: var(--categoryIconBgSize);
  height: var(--categoryIconBgSize);
  border-radius: 180px;
}

.categoryContent {
  display: flex;
  flex-direction: column;
  justify-content: center;
  margin-left: 16px;
}

#categoryList {
	width: 100%;
  list-style-type: none;
	margin: 0px;
  padding: 0px;
}

#categoryList li:hover {
  background-color: var(--highlight);
  cursor: pointer;
}

#categoryTitle {
  font-size: var(--primaryTextSize);
  margin: 0px;
}

.categoryItem {
  display: flex;
  flex-direction: row;
  padding: 8px 16px 8px 16px;
}
</style>
