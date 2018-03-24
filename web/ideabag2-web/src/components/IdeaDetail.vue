<template>
	<div class="appContainer">
		<img v-if="isPerformingAction || isLoading" id="loadingCircle" src="https://samherbert.net/svg-loaders/svg-loaders/oval.svg" />
		<div id="card" v-if="idea != null">
			<p id="ideaTitle">{{idea.title}}</p>
			<p id="ideaDescription">{{idea.description}}</p>
		</div>

		<div id="commentBar">
			<textarea id="commentTb" v-model="comment" placeholder="Post a comment"></textarea>
			<button class="appBtn" @click="postComment" :disabled="!userLoggedIn || isPerformingAction">Post</button>
		</div>

		<div id="comments">
			<ul>
				<li class="comment" v-for="(comment, index) in comments" :key="index">
					<div class="top-row">
						<div>
							<img :src="getAvatar()" alt="avatar" />
							<p id="authorLbl">{{comment.author}}</p>
							<p id="dateLbl">{{getTimestamp(comment.created)}}</p>
						</div>
						<img @click="deleteComment(comment.id, index)" :disabled="isPerformingAction" v-if="comment.author == email" id="deleteCommentBtn" src="/src/assets/ic_delete_black_24px.svg" />
					</div>

					<p class="commentLbl">{{comment.comment}}</p>
				</li>
			</ul>
		</div>
	</div>
</template>

<script>
import eventbus from '../eventbus';
import axios from 'axios';

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
		isPerformingAction() {
			return this.$store.getters.isPerformingAction;
		},
		token() {
			return this.$store.getters.token;
		},
		email() {
			return this.$store.getters.userEmail;
		},
		userLoggedIn() {
			return this.$store.getters.userLoggedIn;
		}
	},
	activated() {
		axios.defaults.baseURL = 'https://ideabag2.firebaseio.com';
		this.$store.dispatch('setTitle', 'Idea details');

		var categoryIndex = this.$route.params.categoryId;
		var ideaIndex = this.$route.params.ideaId;

		this.idea = this.$store.getters.categories[categoryIndex].items[ideaIndex];

		this.getComments();
	},
	deactivated() {
		this.comments.length = 0;
	},
	methods: {
		postComment() {
			if (this.userLoggedIn) {
				this.$store.dispatch('isPerformingAction', true);
				var dataId = this.getDataId();

				var comment = {
					'userId': this.userId,
					'author': this.email,
					'comment': this.comment,
					'created': new Date().getTime()
				};

				var url = `/${dataId}/comments.json?auth=${this.token}`;
				console.log(url);

				axios.post(url, comment).then(response => {
					this.$store.dispatch('isPerformingAction', false);
					comment.id = response.data.name;

					this.comments.push(comment);
					this.comment = '';
				}).catch(error => {
					this.$store.dispatch('isPerformingAction', false);
					eventbus.showToast(error.response.data.error, 'error');
				});
			}
			else {
				eventbus.showToast('Log in to post a comment', 'error', 'long');
			}
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
		},
		getComments() {
			this.$store.dispatch('isPerformingAction', true);
			var dataId = this.getDataId();
			console.log('Getting comments ' + dataId);

			axios.get(`/${dataId}/comments.json`).then(response => {
				if (response.data != null) {
					var keys = Object.keys(response.data);

					for (var i = 0; i < keys.length; i++) {
						var comment = response.data[keys[i]];
						comment.id = keys[i];

						this.comments.push(comment);
					}
				}

				this.$store.dispatch('isPerformingAction', false);
			}).catch(error => {
				eventbus.showToast('Getting comments failed. Please retry.', 'error');
				this.$store.dispatch('isPerformingAction', false);
			});
		},
		getDataId() {
			var categoryId = this.$route.params.categoryId;
			var ideaId = this.idea.id;
			return `${categoryId}C-${ideaId}I`;
		},
		deleteComment(commentId, index) {
			this.$store.dispatch('isPerformingAction', true);
			var dataId = this.getDataId();
			var url = `${dataId}/comments/${commentId}.json?auth=${this.token}`;
			console.log('Deleting ' + url);

			axios.delete(url)
				.then(response => {
					eventbus.showToast('Comment deleted successfully', 'success');
					this.comments.splice(index, 1);
					this.$store.dispatch('isPerformingAction', false);
				}).catch(error => {
					console.log(error.response.data);
					eventbus.showToast('Failed to delete comment. Please retry.', 'error');
					this.$store.dispatch('isPerformingAction', false);
				});
		},
		getTimestamp(milliseconds) {
			var date = new Date(milliseconds);

			return date.toLocaleDateString();
		}
	}
}
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
	margin-bottom: 8px;
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
	font-size: 16px;
	font-family: 'Roboto', 'Arial';
	color: rgba(0, 0, 0, 0.54);
	white-space: pre-wrap;
	word-wrap: break-word;
	margin-top: 4px;
}

#deleteCommentBtn {
	cursor: pointer;
	margin-top: 4px;
}

.top-row {
	display: flex;
	flex-direction: row;
	justify-content: space-between;
	align-items: center;
}

.top-row>div {
	display: flex;
	flex-direction: row;
	align-items: center;
}

#authorLbl {
	margin: 0;
	padding: 0;
	margin-left: 16px;
	color: rgba(0, 0, 0, 0.8);
	font-size: var(--authorLblSize);
	font-weight: bold;
}

#dateLbl {
	margin: 0;
	padding: 0;
	margin-left: var(--dateLblMargin);
	color: rgba(0, 0, 0, 0.5);
	font-size: var(--dateLblSize);
}
</style>


