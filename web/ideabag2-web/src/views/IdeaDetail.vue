<template>
	<div class="appContainer">
		<img v-if="isPerformingAction || isLoading" id="loadingCircle" src="https://samherbert.net/svg-loaders/svg-loaders/oval.svg" />
		<div id="card" v-if="idea != null">
			<div id="card-top-row">
				<p id="ideaTitle">{{idea.title}}</p>
				<div>
					<img id="bookmarkBtn" @click="toggleBookmark()" :src="bookmarkIcon" />
					<button class="appBtnOutline" @click="$modal.show('progress-modal')">Update progress</button>
					<modal name="progress-modal" height="auto" :adaptive="true" :classes="['v--modal', 'progress-modal']">
						<h3>Set idea progress</h3>
						<ul class="progress-list">
							<li @click="setProgress('done');$modal.hide('progress-modal')">
								<img :src="ICON_DONE" alt="">
								<span>Done</span>
							</li>
							<li @click="setProgress('in-progress');$modal.hide('progress-modal')">
								<img :src="ICON_IN_PROGRESS" alt="">
								<span>In Progress</span>
							</li>
							<li @click="setProgress('undecided');$modal.hide('progress-modal')">
								<img :src="ICON_UNDECIDED" alt="" />
								<span>Undecided</span>
							</li>
						</ul>
					</modal>
				</div>
			</div>
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
						<img @click="deleteComment(comment.id, index)" :disabled="isPerformingAction" v-if="comment.author == email"
						 id="deleteCommentBtn" src="https://res.cloudinary.com/mclint-cdn/image/upload/v1523221457/ic_delete_black_24px.svg" />
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
			isBookmarked: false,
			ICON_DONE: require("../../static/img/baseline-check_box-24px.svg"),
			ICON_IN_PROGRESS: require("../../static/img/baseline-check_box_outline_blank-24px.svg"),
			ICON_UNDECIDED: require("../../static/img/baseline-indeterminate_check_box-24px.svg"),
			eyes: [
				'eyes1',
				'eyes10',
				'eyes2',
				'eyes3',
				'eyes4',
				'eyes5',
				'eyes6',
				'eyes7',
				'eyes9'
			],
			noses: [
				'nose2',
				'nose3',
				'nose4',
				'nose5',
				'nose6',
				'nose7',
				'nose8',
				'nose9'
			],
			mouths: [
				'mouth1',
				'mouth10',
				'mouth11',
				'mouth3',
				'mouth5',
				'mouth6',
				'mouth7',
				'mouth9'
			]
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
		},
		userDataDB() {
			return this.$store.getters.userDataDB;
		},
		bookmarkIcon() {
			if (this.isBookmarked) {
				return require("../../static/img/outline-bookmark-24px.svg");
			} else {
				return require("../../static/img/outline-bookmark_border-24px.svg");
			}
		}
	},
	watch: {
		userDataDB(db) {
			if (db !== null) {
				this.loadUserData();
			}
		}
	},
	activated() {
		axios.defaults.baseURL = 'https://ideabag2.firebaseio.com';
		this.$store.dispatch('setTitle', 'Idea details');

		var categoryIndex = this.$route.params.categoryId;
		var ideaIndex = this.$route.params.ideaId;

		this.idea = this.$store.getters.categories[categoryIndex].items[ideaIndex];

		if (this.userDataDB !== null) {
			this.loadUserData();
		}

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
					userId: this.userId,
					author: this.email,
					comment: this.comment,
					created: new Date().getTime()
				};

				var url = `/${dataId}/comments.json?auth=${this.token}`;

				axios
					.post(url, comment)
					.then(response => {
						this.$store.dispatch('isPerformingAction', false);
						comment.id = response.data.name;

						this.comments.push(comment);
						this.comment = '';
					})
					.catch(error => {
						this.$store.dispatch('isPerformingAction', false);
						eventbus.showToast(error.response.data.error, 'error');
					});
			} else {
				eventbus.showToast('Log in to post a comment', 'error', 'long');
			}
		},
		getAvatar() {
			var face = this.getRandomFace();
			return `https://api.adorable.io/avatars/face/${face.eye}/${face.nose}/${
				face.mouth
			}/ffa000`;
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

			axios
				.get(`/${dataId}/comments.json`)
				.then(response => {
					if (response.data != null) {
						var keys = Object.keys(response.data);

						for (var i = 0; i < keys.length; i++) {
							var comment = response.data[keys[i]];
							comment.id = keys[i];

							this.comments.push(comment);
						}
					}

					this.$store.dispatch('isPerformingAction', false);
				})
				.catch(error => {
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

			axios
				.delete(url)
				.then(response => {
					eventbus.showToast('Comment deleted successfully', 'success');
					this.comments.splice(index, 1);
					this.$store.dispatch('isPerformingAction', false);
				})
				.catch(error => {
					eventbus.showToast(
						'Failed to delete comment. Please retry.',
						'error'
					);
					this.$store.dispatch('isPerformingAction', false);
				});
		},
		getTimestamp(milliseconds) {
			var date = new Date(milliseconds);

			return date.toLocaleDateString();
		},
		addNewIdea(ideaId, bookmarked) {

		},
		loadUserData() {
			this.userDataDB.transaction(["ideas"], "readonly")
			.objectStore("ideas")
			.get(this.getDataId())
			.onsuccess = event => {
				if (event.target.result) {
					this.isBookmarked = event.target.result.bookmarked;
				} else {
					this.isBookmarked = false;
				}
			};
		},
		toggleBookmark() {
			if (this.isBookmarked) {
				this.removeFromBookmarks();
			} else {
				this.addToBookmarks();
			}
		},
		addToBookmarks() {
			const id = this.getDataId();
			const db = this.userDataDB;
			const objectStore = db.transaction(["ideas"], "readwrite").objectStore("ideas")

			objectStore.get(id).onsuccess = event => {
				if (event.target.result === undefined) {
					objectStore.add({"id": id, "bookmarked": 1, "progress": "undecided"})
					.onsuccess = event => this.isBookmarked = true;
				} else {
					event.target.result.bookmarked = 1;
					objectStore.put(event.target.result)
					.onsuccess = event => this.isBookmarked = true;
				}
			}
		},
		removeFromBookmarks() {
			const id = this.getDataId();
			const db = this.userDataDB;
			const objectStore = db.transaction(["ideas"], "readwrite").objectStore("ideas");
			objectStore.get(id)
			.onsuccess = event => {
				event.target.result.bookmarked = 0;
				objectStore.put(event.target.result)
				.onsuccess = event =>	this.isBookmarked = false;
			}
		},
		setProgress(progress) {
			const id = this.getDataId();
			const db = this.userDataDB;
			const objectStore = db.transaction(["ideas"], "readwrite").objectStore("ideas");
			objectStore.get(id)
			.onsuccess = event => {
				if (event.target.result === undefined) {
					objectStore.add({"id": id, "bookmarked": 0, "progress": progress});
				} else {
					event.target.result.progress = progress;
					objectStore.put(event.target.result);
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
#card {
	border: 2px solid transparent;
	border-radius: 10px 10px 0px 0px;
	background-color: var(--primary);
	padding: 16px;
	margin: var(--cardMargin);
}

#card-top-row {
	display: flex;
	flex-flow: row wrap;
	justify-content: space-between;
	margin-bottom: 1rem;
}

#ideaTitle {
	color: rgba(0, 0, 0, 0.8);
	font-size: var(--ideaTextSize);
	font-weight: bold;
}

#bookmarkBtn {
	cursor: pointer;
	margin: 0 10px 10px 0;
}
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

#ideaDescription {
	color: rgba(0, 0, 0, 0.54);
	font-size: 16px;
	white-space: pre-wrap;
	word-wrap: break-word;
}

#comments {
	margin-top: 40px;
}

#comments > ul {
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

.top-row > div {
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

@media screen and (max-width: 768px) {
	#card-top-row {
		flex-flow: column nowrap;
		align-items: flex-start;
	}
}
</style>
