<template>
  <div class="container-full">
    <font-awesome-icon
      class="loader"
      v-if="$store.getters.isLoading"
      icon="spinner"
      size="3x"
      spin
      fixed-width
    ></font-awesome-icon>
    <div class="container-app">
      <ul class="categories">
        <li
          v-for="(category, index) in categories"
          :key="index"
          @click="notifyCategoryClicked(index)"
          :class="{highlight: index === selectedIndex}"
        >
          <div class="category">
            <div class="category__icon-container">
              <img class="category__icon" :src="icons[index]" alt="category_icon">
            </div>
            <div class="category-content">
              <p class="category__title text--primary">{{category.categoryLbl}}</p>
              <p class="text--secondary">Ideas: {{category.categoryCount}}</p>
            </div>
          </div>
        </li>
      </ul>
    </div>

    <router-link class="button floating-action-button" to="bookmarks" aria-label="bookmarks">
      <font-awesome-icon :icon="['fas', 'bookmark']" size="lg" fixed-width></font-awesome-icon>
    </router-link>
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

<style>
.category__icon {
  width: var(--categoryIconSize);
}

.category__icon-container {
  background: rgba(0, 0, 0, 0.8);
  display: flex;
  justify-content: center;
  align-items: center;
  width: var(--categoryIconBgSize);
  height: var(--categoryIconBgSize);
  border-radius: 18rem;
}

.category-content {
  display: flex;
  flex-direction: column;
  justify-content: center;
  margin-left: 1.6rem;
}

.categories {
  width: 100%;
  list-style-type: none;
}

.categories li:hover {
  background-color: var(--highlight);
  cursor: pointer;
}

.category__title {
  font-size: var(--primaryTextSize);
}

.category {
  display: flex;
  flex-direction: row;
  padding: 0.8rem 1.6rem;
}
</style>
