<template>
	<div class="appContainer">
		<img v-if="isPerformingAction || isLoading" id="loadingCircle" src="https://samherbert.net/svg-loaders/svg-loaders/oval.svg" />
		<div id="card" v-if="idea != null">
			<div id="card-top-row">
				<p id="ideaTitle">{{idea.title}}</p>
				<div>
					<button class="appBtnOutline" @click="toggleBookmark()" @mouseover="bookmarkButtonHovered = true" @mouseleave="bookmarkButtonHovered = false"><img :src="bookmarkIcon" /></button>
					<button class="appBtnOutline" @click="$modal.show('progress-modal')">Update progress</button>
					<modal name="progress-modal" height="auto" :adaptive="true" :classes="['v--modal', 'progress-modal']" @opened='updateProgressRadiobuttons'>
						<h3>Set idea progress</h3>
						<ul class="progress-list">
							<li @click="setProgress('done');">
								<input id="progress-done" class="progress-radiobutton" type="radio" name="progress" value="done" />
								<label for="progress-done">Done</label>
							</li>
							<li @click="setProgress('in-progress');">
								<input id="in-progress" class="progress-radiobutton" type="radio" name="progress" value="in-progress" />
								<label for="in-progress">In Progress</label>
							</li>
							<li @click="setProgress('undecided');">
								<input id="progress-undecided" class="progress-radiobutton" type="radio" name="progress" value="undecided" checked/>
								<label for="progress-undecided">Undecided</label>
							</li>
						</ul>
					</modal>
				</div>
			</div>
			<p id="ideaDescription">{{idea.description}}</p>
		</div>

		<div id="progress-bar"></div>

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
			progress: 'undecided',
			bookmarkButtonHovered: false,
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
				if (this.bookmarkButtonHovered) {
					('img/outline-bookmark_colored-24px.svg');
				} else {
					('img/outline-bookmark-24px.svg');
				}
			} else {
				if (this.bookmarkButtonHovered) {
					return 'img/outline-bookmark_border_colored-24px.svg';
				} else {
					return 'img/outline-bookmark_border-24px.svg';
				}
			}
		},
		dataId() {
			return `${this.idea.categoryId - 1}C-${this.idea.id}I`;
		}
	},
	watch: {
		progress(progress) {
			document.getElementById(
				'progress-bar'
			).style.backgroundColor = `var(--${progress})`;
		},
		userDataDB(db) {
			if (db !== null) {
				this.loadUserData();
			}
		}
	},
	activated() {
		axios.defaults.baseURL = 'https://ideabag2.firebaseio.com';
		this.$store.dispatch('setTitle', 'Idea details');

		const categoryIndex = this.$route.params.categoryId;
		const ideaIndex = this.$route.params.ideaId;

		this.idea = this.$store.getters.categories[categoryId - 1].items[
			ideaId - 1
		];

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
				const dataId = this.getDataId();

				const comment = {
					userId: this.userId,
					author: this.email,
					comment: this.comment,
					created: new Date().getTime()
				};

				const url = `/${dataId}/comments.json?auth=${this.token}`;

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
			const face = this.getRandomFace();
			return `https://api.adorable.io/avatars/face/${face.eye}/${face.nose}/${
				face.mouth
			}/ffa000`;
		},
		getRandomNumber(min, max) {
			return Math.floor(Math.random() * (max - min + 1)) + min;
		},
		getRandomFace() {
			const eye = this.eyes[this.getRandomNumber(0, this.eyes.length - 1)];
			const nose = this.noses[this.getRandomNumber(0, this.noses.length - 1)];
			const mouth = this.mouths[
				this.getRandomNumber(0, this.mouths.length - 1)
			];

			return { eye, nose, mouth };
		},
		getComments() {
			this.$store.dispatch('isPerformingAction', true);
			const dataId = this.getDataId();

			axios
				.get(`/${dataId}/comments.json`)
				.then(response => {
					if (response.data != null) {
						const keys = Object.keys(response.data);

						for (let i = 0; i < keys.length; i++) {
							const comment = response.data[keys[i]];
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
		deleteComment(commentId, index) {
			this.$store.dispatch('isPerformingAction', true);
			const dataId = this.dataId();
			const url = `${dataId}/comments/${commentId}.json?auth=${this.token}`;

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
			return new Date(milliseconds).toLocaleDateString();
		},
		addNewIdea(ideaId, bookmarked, progress) {
			const bookmarkedBinary = bookmarked ? 1 : 0;
			this.userDataDB
				.transaction(['ideas'], 'readwrite')
				.objectStore('ideas')
				.add({
					id: ideaId,
					bookmarked: bookmarkedBinary,
					progress: progress
				}).onsuccess = event => {
				this.isBookmarked = bookmarked;
				this.progress = progress;
			};
		},
		loadUserData() {
			this.userDataDB
				.transaction(['ideas'], 'readonly')
				.objectStore('ideas')
				.get(this.getDataId()).onsuccess = event => {
				if (event.target.result) {
					this.progress = event.target.result.progress;
					this.isBookmarked = event.target.result.bookmarked;
				} else {
					this.progress = 'undecided';
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
			const objectStore = db
				.transaction(['ideas'], 'readwrite')
				.objectStore('ideas');

			objectStore.get(id).onsuccess = event => {
				if (event.target.result === undefined) {
					this.addNewIdea(id, true, 'undecided');
				} else {
					event.target.result.bookmarked = 1;
					objectStore.put(event.target.result).onsuccess = event =>
						(this.isBookmarked = true);
				}
			};
		},
		removeFromBookmarks() {
			const id = this.getDataId();
			const db = this.userDataDB;
			const objectStore = db
				.transaction(['ideas'], 'readwrite')
				.objectStore('ideas');
			objectStore.get(id).onsuccess = event => {
				event.target.result.bookmarked = 0;
				objectStore.put(event.target.result).onsuccess = event =>
					(this.isBookmarked = false);
			};
		},
		setProgress(progress) {
			const id = this.getDataId();
			const db = this.userDataDB;
			const objectStore = db
				.transaction(['ideas'], 'readwrite')
				.objectStore('ideas');
			objectStore.get(id).onsuccess = event => {
				if (event.target.result === undefined) {
					this.addNewIdea(id, false, progress);
				} else {
					event.target.result.progress = progress;
					objectStore.put(event.target.result).onsuccess = event =>
						(this.progress = progress);
				}
			};

			this.$modal.hide('progress-modal');
		},
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
	align-items: center;
	margin-bottom: 1rem;
}

#ideaTitle {
	color: rgba(0, 0, 0, 0.8);
	font-size: var(--ideaTextSize);
	font-weight: bold;
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
.progress-list > li > input,
.progress-list > li > label {
	cursor: pointer;
}

#ideaDescription {
	color: rgba(0, 0, 0, 0.54);
	font-size: 16px;
	white-space: pre-wrap;
	word-wrap: break-word;
}

#progress-bar {
	background-color: var(--undecided);
	border: 1px solid rgba(0, 0, 0, 0.2);
	height: 10px;
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
	#progress-bar {
		margin: 0 16px;
	}
	#commentBar {
		margin: 0px 16px;
	}
}
</style>
