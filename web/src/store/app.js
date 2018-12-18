import axios from 'axios';

const state = {
  categories: [],
  newIdeas: [],
  isLoading: true,
  selectedIdeaIndex: -1,
  navbarTitle: 'IdeaBag 2 (BETA)',
  userDataDB: null
};

const getters = {
  categories: state => {
    return state.categories;
  },
  newIdeas: state => {
    return state.newIdeas;
  },
  isLoading: state => {
    return state.isLoading;
  },
  selectedIdeaIndex: state => {
    return state.selectedIdeaIndex;
  },
  navbarTitle: state => {
    return state.navbarTitle;
  },
  userDataDB: state => {
    return state.userDataDB;
  }
};

const mutations = {
  SET_LOADING(state, isLoading) {
    state.isLoading = isLoading;
  },
  SET_CATEGORIES(state, categories) {
    state.categories = categories;
  },
  SET_SELECTED_IDEA_INDEX(state, index) {
    state.selectedIdeaIndex = index;
  },
  SET_TITLE(state, title) {
    state.navbarTitle = title;
  },
  SET_USER_DATA_DB(state, db) {
    state.userDataDB = db;
  },
  SET_NEW_IDEAS(state, newIdeas) {
    state.newIdeas = newIdeas;
    localStorage.setItem('newIdeas', JSON.stringify(newIdeas));
  }
};

const actions = {
  setLoading(context, isLoading) {
    context.commit('SET_LOADING', isLoading);
  },
  setCategories(context, categories) {
    context.commit('SET_CATEGORIES', categories);
  },
  setSelectedIdeaIndex(context, index) {
    context.commit('SET_SELECTED_IDEA_INDEX', index);
  },
  setTitle(context, title) {
    context.commit('SET_TITLE', title);
  },
  setUserDataDB(context, db) {
    context.commit('SET_USER_DATA_DB', db);
  },
  async getNewIdeas({ commit }) {
    const response = await axios.get(
      'https://docs.google.com/document/d/1JHIGHjD7A_sGVelLongEV3I-n-i-e3am-q7x96eRyOA/export?format=txt'
    );

    if (response.status == 200) {
      const ideas = response.data.split(',').map(e => {
        const d = e.split('-');
        return {
          categoryId: parseInt(d[0]),
          ideaId: parseInt(d[1])
        };
      });
      commit('SET_NEW_IDEAS', ideas);
    }
  }
};

export default {
  state,
  getters,
  actions,
  mutations
};
