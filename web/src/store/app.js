const state = {
  categories: [],
  isLoading: true,
  selectedIdeaIndex: -1,
  navbarTitle: 'IdeaBag 2 (BETA)',
  userDataDB: null
};

const getters = {
  categories: state => {
    return state.categories;
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
  }
};

const actions = {
  setLoading({ commit }, isLoading) {
    commit('SET_LOADING', isLoading);
  },
  setCategories({ commit }, categories) {
    commit('SET_CATEGORIES', categories);
  },
  setSelectedIdeaIndex({ commit }, index) {
    commit('SET_SELECTED_IDEA_INDEX', index);
  },
  setTitle({ commit }, title) {
    commit('SET_TITLE', title);
  },
  setUserDataDB({ commit }, db) {
    commit('SET_USER_DATA_DB', db);
  }
};

export default {
  state,
  getters,
  actions,
  mutations
};
