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
        <div class="idea-detail__buttons">
          <button class="button button--outlined" @click="toggleBookmark()" aria-label="Toggle bookmarking" :aria-pressed="idea.bookmarked">
            <font-awesome-icon :icon="[bookmarkIconPrefix, 'bookmark']" size="lg" fixed-width></font-awesome-icon>
          </button>
          <button
            class="button button--outlined"
            @click="$modal.show('progress-modal-0')"
          >Update progress</button>
          <progress-modal @update-progress="setProgress" :progress="idea.progress"></progress-modal>
        </div>
      </div>
      <p class="idea-detail__description">{{idea.description}}</p>
    </div>

    <div class="container-progress" :style="'background-color: var(--' + idea.progress + ')'"></div>

    <div class="container-comment">
      <textarea class="comment__textarea" v-model="comment" placeholder="Post a comment"></textarea>
      <button
        class="button"
        @click="postComment"
        :disabled="!userLoggedIn || isPerformingAction"
      >Post</button>
    </div>

    <div class="card container-note" v-if="idea.note">
      <font-awesome-icon class="note__icon" :icon="['fas', 'sticky-note']" fixed-width></font-awesome-icon>
      <p class="note__text">{{ idea.note }}</p>
      <router-link
        class="button note__button"
        :to="{ name: 'editnote', params: { categoryId: idea.categoryId, ideaId: idea.id } }"
      >
        <font-awesome-icon :icon="['fas', 'pen']" size="lg" fixed-width></font-awesome-icon>
      </router-link>
      <button class="button note__button" @click="saveNote(dataId, '')">
        <font-awesome-icon :icon="['fas', 'trash']"></font-awesome-icon>
      </button>
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
              v-if="comment.author === email"
              aria-label="delete comment"
            >
              <font-awesome-icon icon="trash" size="lg" fixed-width></font-awesome-icon>
            </button>
          </div>

          <hr>

          <p class="comment__label">{{comment.comment}}</p>
        </li>
      </ul>
    </div>

    <router-link
      class="button floating-action-button"
      :to="{ name: 'editnote', params: { categoryId: idea.categoryId, ideaId: idea.id } }"
      aria-label="Edit note"
    >
      <font-awesome-icon :icon="['fas', 'sticky-note']" size="lg" fixed-width></font-awesome-icon>
    </router-link>
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
      comment: '',
      comments: [],
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
    idea() {
      const categoryId = this.$route.params.categoryId;
      const ideaId = this.$route.params.ideaId;
      if (categoryId && ideaId) {
        return this.$store.getters.categories[categoryId - 1].items[ideaId - 1];
      } else {
        return {
          category: 'Numbers',
          categoryId: 1,
          title: '',
          difficulty: 'Beginner',
          id: 1,
          description: '',
          progress: 'undecided',
          bookmarked: false
        };
      }
    },
    isLoading() {
      return this.$store.getters.categories.length === 0;
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
      if (this.idea.bookmarked) {
        return 'fas'; // Solid icon
      } else {
        return 'far'; // Regular icon (only outline)
      }
    },
    dataId() {
      return `${this.idea.categoryId}C-${this.idea.id}I`;
    }
  },
  activated() {
    axios.defaults.baseURL = 'https://ideabag2.firebaseio.com';
    this.$store.dispatch('setTitle', 'Idea details');

    this.getComments();
  },
  deactivated() {
    this.comments.length = 0;
  },
  methods: {
    postComment() {
      if (this.userLoggedIn) {
        this.$store.dispatch('isPerformingAction', true);
        const comment = {
          userId: this.userId,
          author: this.email,
          comment: this.comment,
          created: new Date().getTime()
        };
        const url = `/${this.dataId}/comments.json?auth=${this.token}`;
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
      const mouth = this.mouths[this.getRandomNumber(0, this.mouths.length - 1)];
      return { eye, nose, mouth };
    },
    getComments() {
      this.$store.dispatch('isPerformingAction', true);
      axios
        .get(`${this.idea.categoryId - 1}C-${this.idea.id}I/comments.json`)
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
        .catch(() => {
          eventbus.showToast('Getting comments failed. Please retry.', 'error');
          this.$store.dispatch('isPerformingAction', false);
        });
    },
    deleteComment(commentId, index) {
      this.$store.dispatch('isPerformingAction', true);
      const url = `${this.dataId}/comments/${commentId}.json?auth=${this.token}`;
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
    toggleBookmark() {
      if (this.idea.bookmarked) {
        this.removeFromBookmarks(this.dataId);
      } else {
        this.addToBookmarks(this.dataId);
      }
      this.isBookmarked = !this.isBookmarked;
    },
    setProgress(progress) {
      this.updateProgress(this.dataId, progress);
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

.idea-detail__buttons {
  display: flex;
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

.container-note {
  background-color: var(--primary);
  display: grid;
  grid-template-columns: 5% auto 10% 10%;
  grid-column-gap: 1.5rem;
  justify-items: center;
  align-items: center;
}

.note__icon {
  font-size: 2.5rem;
  color: var(--primaryLight);
}

.note__text {
  justify-self: start;
  font-size: var(--ideaNoteTextSize);
  white-space: pre-wrap;
  overflow-wrap: break-word;
}

.note__button {
  background-color: var(--primaryLight);
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
  padding: 0;
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

.comment hr {
  margin-top: 0.8rem;
  background-color: rgb(218, 218, 218);
  border: none;
  height: 0.1rem;
}

.comment__avatar {
  width: var(--avatarSize);
  border-radius: 50%;
}

.comment__textarea {
  height: 8rem;
  margin: auto;
  width: 100%;
  border: solid 0 transparent;
  background-color: white;
  resize: none;
  font-size: 1.6rem;
}

.comment__label {
  background-color: transparent;
  font-size: 1.6rem;
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
  padding: 0;
  margin: 0 0 0 1.6rem;
  color: rgba(0, 0, 0, 0.8);
  font-size: var(--authorLblSize);
  font-weight: bold;
}

.comment__date {
  padding: 0;
  margin: 0 0 0 var(--dateLblMargin);
  color: rgba(0, 0, 0, 0.5);
  font-size: var(--dateLblSize);
}

@media screen and (max-width: 76.8rem) {
  .card {
    margin: 0;
  }

  .card-top-row {
    flex-flow: column nowrap;
    align-items: flex-start;
  }

  .container-note {
    grid-template-columns: 5% auto 15% 15%;
  }

  .note__button {
    border-radius: 50%;
    min-width: 0;
    width: 5rem;
    height: 5rem;
  }

  .comments > p {
    display: none;
  }
}
</style>
