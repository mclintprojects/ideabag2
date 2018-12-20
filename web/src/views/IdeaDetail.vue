<template>
  <div class="container-app">
    <font-awesome-icon
      class="loader"
      v-if="isPerformingAction || isLoading"
      icon="spinner"
      size="3x"
      spin
      fixed-width
    ></font-awesome-icon>
    <div class="card" v-if="idea != null">
      <div class="card-top-row">
        <p class="idea-detail__title">{{idea.title}}</p>
        <div>
          <button class="button button--outlined" @click="toggleBookmark()">
            <font-awesome-icon :icon="[bookmarkIconPrefix, 'bookmark']" size="lg" fixed-width></font-awesome-icon>
          </button>
          <button
            class="button button--outlined"
            @click="$modal.show('progress-modal-0')"
          >Update progress</button>
          <progress-modal @update-progress="setProgress" :progress="progress"></progress-modal>
        </div>
      </div>
      <p class="idea-detail__description">{{idea.description}}</p>
    </div>

    <div class="container-progress"></div>

    <div class="container-comment">
      <textarea class="comment__textarea" v-model="comment" placeholder="Post a comment"></textarea>
      <button
        class="button"
        @click="postComment"
        :disabled="!userLoggedIn || isPerformingAction"
      >Post</button>
    </div>

    <div class="comments">
      <p class="text--primary">Comments</p>
      <ul>
        <li class="comment" v-for="(comment, index) in comments" :key="index">
          <div class="top-row">
            <div>
              <img class="comment__avatar" :src="getAvatar()" alt="avatar">
              <p class="comment__author">{{comment.author}}</p>
              <p class="comment__date">{{getTimestamp(comment.created)}}</p>
            </div>
            <button
              class="button-delete-comment icon-button"
              @click="deleteComment(comment.id, index)"
              :disabled="isPerformingAction"
              v-if="comment.author == email"
            >
              <font-awesome-icon icon="trash" size="lg" fixed-width></font-awesome-icon>
            </button>
          </div>

          <p class="comment__label">{{comment.comment}}</p>
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
import eventbus from '../eventbus';
import axios from 'axios';
import UserDataDBInterface from '../mixins/UserDataDBInterface';
import ProgressModal from '../components/ProgressModal';

export default {
  mixins: [UserDataDBInterface],
  components: {
    'progress-modal': ProgressModal
  },
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
    bookmarkIconPrefix() {
      if (this.isBookmarked) {
        return 'fas'; // Solid icon
      } else {
        return 'far'; // Regular icon (only outline)
      }
    },
    dataId() {
      return `${this.idea.categoryId}C-${this.idea.id}I`;
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

    const categoryId = this.$route.params.categoryId;
    const ideaId = this.$route.params.ideaId;
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
        var comment = {
          userId: this.userId,
          author: this.email,
          comment: this.comment,
          created: new Date().getTime()
        };
        var url = `/${this.dataId}/comments.json?auth=${this.token}`;
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
      axios
        .get(`${this.idea.categoryId - 1}C-${this.idea.id}I/comments.json`)
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
        .catch(() => {
          eventbus.showToast('Getting comments failed. Please retry.', 'error');
          this.$store.dispatch('isPerformingAction', false);
        });
    },
    deleteComment(commentId, index) {
      this.$store.dispatch('isPerformingAction', true);
      var url = `${this.dataId}/comments/${commentId}.json?auth=${this.token}`;
      axios
        .delete(url)
        .then(() => {
          eventbus.showToast('Comment deleted successfully', 'success');
          this.comments.splice(index, 1);
          this.$store.dispatch('isPerformingAction', false);
        })
        .catch(() => {
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
    loadUserData() {
      this.userDataDB
        .transaction(['ideas'], 'readonly')
        .objectStore('ideas')
        .get(this.dataId).onsuccess = event => {
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
        this.removeFromBookmarks(this.dataId);
      } else {
        this.addToBookmarks(this.dataId);
      }
      this.isBookmarked = !this.isBookmarked;
    },
    setProgress(progress) {
      this.updateProgress(this.dataId, progress);
      this.progress = progress;
    }
  }
};
</script>

<style>
.card {
  background-color: white;
  padding: 1.6rem;
  margin: var(--cardMargin);
}

.card-top-row {
  display: flex;
  flex-flow: row wrap;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.idea-detail__title {
  color: rgba(0, 0, 0, 0.8);
  font-size: var(--ideaTextSize);
  font-weight: bold;
}

.idea-detail__description {
  color: rgba(0, 0, 0, 0.54);
  font-size: 1.6rem;
  line-height: 2.2rem;
  white-space: pre-wrap;
  word-wrap: break-word;
}

.container-progress {
  background-color: var(--undecided);
  height: 0.5rem;
}

.comments {
  margin-top: 2.4rem;
}

.comments > p {
  margin-bottom: 0.8rem;
}

.comments > ul {
  list-style-type: none;
  margin: 0;
  padding: 0rem;
}

.container-comment {
  background-color: white;
  padding: 1.6rem;
  display: flex;
  flex-direction: row;
  justify-content: center;
}

.comment {
  padding: var(--commentPadding);
  border-bottom: solid 0.3rem var(--primary);
  background-color: white;
  margin-bottom: 1.6rem;
}

.comment__avatar {
  width: var(--avatarSize);
  border-radius: 50%;
}

.comment__textarea {
  height: 8rem;
  margin: auto;
  width: 100%;
  border: solid 0rem transparent;
  background-color: white;
  resize: none;
  font-size: 1.6rem;
}

.comment__label {
  background-color: transparent;
  font-size: 1.6rem;
  font-family: "Roboto", "Arial";
  color: rgba(0, 0, 0, 0.54);
  white-space: pre-wrap;
  word-wrap: break-word;
  margin-top: 0.4rem;
}

.button-delete-comment {
  color: black;
  cursor: pointer;
  margin-top: 0.4rem;
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

.comment__author {
  margin: 0;
  padding: 0;
  margin-left: 1.6rem;
  color: rgba(0, 0, 0, 0.8);
  font-size: var(--authorLblSize);
  font-weight: bold;
}

.comment__date {
  margin: 0;
  padding: 0;
  margin-left: var(--dateLblMargin);
  color: rgba(0, 0, 0, 0.5);
  font-size: var(--dateLblSize);
}

@media screen and (max-width: 76.8rem) {
  .card-top-row {
    flex-flow: column nowrap;
    align-items: flex-start;
  }

  .container-progress {
    margin: 0 1.6rem;
  }

  .container-comment {
    margin: 0rem 1.6rem;
  }
}
</style>
