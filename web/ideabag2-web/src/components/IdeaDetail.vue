<template>
	<div class="appContainer">
		<img v-if="isLoading" id="loadingCircle" src="https://samherbert.net/svg-loaders/svg-loaders/oval.svg" />
		<div id="card">
			<p id="ideaTitle">{{idea.title}}</p>
			<p id="ideaDescription">{{idea.description}}</p>
		</div>

		<div id="commentBar">
			<textarea id="commentTb" v-model="comment" placeholder="Post a comment"></textarea>
			<button class="appBtn" @click="postComment">Post</button>
		</div>

		<div id="comments">
			<ul>
				<li class="comment" v-for="(comment, index) in comments" :key="index">
					<div class="row">
						<div class="col-xs-2">
							<img :src="getAvatar()" alt="avatar" />
						</div>
						<div class="col-xs-8">
							<p class="commentLbl">{{comment}}</p>
						</div>

						<div class="col-xs-2">
							<img id="deleteCommentBtn" src="/src/assets/ic_delete_black_24px.svg" />
						</div>
					</div>
				</li>
			</ul>
		</div>
	</div>
</template>

<script>
export default {
	data() {
		return {
			idea: null,
			comment: '',
			comments: [],
			eyes: ["eyes1", "eyes10", "eyes2", "eyes3", "eyes4", "eyes5", "eyes6", "eyes7", "eyes9"],
			noses: ["nose2", "nose3", "nose4", "nose5", "nose6", "nose7", "nose8", "nose9"],
			mouths: ["mouth1", "mouth10", "mouth11", "mouth3", "mouth5", "mouth6", "mouth7", "mouth9"]
		};
	},
	computed: {
		isLoading() {
			return this.$store.getters.categories.length == 0;
		},
	},
	activated() {
		this.$store.dispatch('setTitle', 'Idea details');

		var categoryIndex = this.$route.params.categoryId;
		var ideaIndex = this.$route.params.ideaId;

		this.idea = this.$store.getters.categories[categoryIndex].items[ideaIndex];
	},
	methods: {
		postComment() {
			this.comments.push(this.comment);
			this.comment = '';
		},
		getAvatar() {
			var face = this.getRandomFace();
			return `https://api.adorable.io/avatars/face/${face.eye}/${face.nose}/${face.mouth}/ffa000`;
		},
		getRandomNumber(min, max) {
			return Math.floor(Math.random() * (max - min + 1)) + min;
		},
		getRandomFace() {
			var eye = this.eyes[this.getRandomNumber(0, this.eyes.length - 1)];
			var nose = this.noses[this.getRandomNumber(0, this.noses.length - 1)];
			var mouth = this.mouths[this.getRandomNumber(0, this.mouths.length - 1)];

			return { eye, nose, mouth };
		}
	}
};
</script>

<style scoped>
#card {
	border: 2px solid transparent;
	border-radius: 10px 10px 0px 0px;
	background-color: var(--primary);
	padding: 16px;
	margin: var(--cardMargin);
}

#ideaTitle {
	color: rgba(0, 0, 0, 0.8);
	font-size: var(--ideaTextSize);
	font-weight: bold;
}

#ideaDescription {
	color: rgba(0, 0, 0, 0.54);
	font-size: 16px;
	white-space: pre-wrap;
	word-wrap: break-word;
}

#comments {
	margin-top: 40px;
}

#comments>ul {
	list-style-type: none;
	margin: 0;
	padding: 0px;
}

#commentBar {
	background-color: white;
	padding: 16px;
	display: flex;
	flex-direction: row;
	justify-content: center;
}

.comment {
	padding: var(--commentPadding);
	border-bottom: solid 5px var(--primary);
	background-color: white;
}

.comment img {
	width: var(--avatarSize);
	border-radius: 50%;
}

#commentTb {
	height: 80px;
	margin: auto;
	width: 100%;
	border: solid 0px transparent;
	background-color: white;
	resize: none;
	font-size: 16px;
}

.commentLbl {
	background-color: transparent;
	font-size: 14px;
	font-family: 'Source Code Pro', monospace;
	color: rgba(0, 0, 0, 0.54);
	white-space: pre-wrap;
	word-wrap: break-word;
	padding: 0;
	margin: 0;
}

#deleteCommentBtn {
	width: 30px;
	cursor: pointer;
	float: right;
}
</style>


