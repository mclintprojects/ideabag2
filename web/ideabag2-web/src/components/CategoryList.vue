<template>
    <div id="categoryContainer">
        <ul id="categoryList">
            <li v-for="(category, index) in categories" :key="index" @click="notifyCategoryClicked(index)">
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
</template>

<script>
export default {
	data() {
		return {
			icons: [
				'../src/assets/numbers.png',
				'../src/assets/text.png',
				'../src/assets/network.png',
				'../src/assets/enterprise.png',
				'../src/assets/cpu.png',
				'../src/assets/web.png',
				'../src/assets/file.png',
				'../src/assets/database.png',
				'../src/assets/multimedia.png',
				'../src/assets/games.png'
			]
		};
	},
	props: ['categories'],
	methods: {
		notifyCategoryClicked(index) {
			this.$emit('categoryClicked', index);
		}
	}
};
</script>

<style scoped>
#categoryContainer {
	height: 20%;
	margin-left: 16px;
	padding: 0px;
}

.categoryIcon {
	width: 36px;
}

.categoryIconBg {
	background: rgba(0, 0, 0, 0.8);
	display: flex;
	justify-content: center;
	align-items: center;
	width: 72px;
	height: 72px;
	border-radius: 180px;
}

.categoryContent {
	display: flex;
	flex-direction: column;
	justify-content: center;
	margin-left: 16px;
}

#categoryList {
	list-style-type: none;
	margin: 0px;
	padding: 0px;
}

#categoryList li:hover {
	background-color: var(--highlight);
	cursor: pointer;
}

#categoryTitle {
	font-size: 18px;
	margin: 0px;
}

.categoryItem {
	display: flex;
	flex-direction: row;
	padding: 8px 16px 8px 16px;
}
</style>


