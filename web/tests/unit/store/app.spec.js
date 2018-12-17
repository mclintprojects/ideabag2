import app from '../../../src/store/app';

describe('app.js', () => {
  let state, getters, mutations, actions, commit;

  beforeAll(() => {
    state = app.state;
    getters = app.getters;
    mutations = app.mutations;
    actions = app.actions;
  });

  beforeEach(() => (commit = jest.fn()));

  it('should correctly map getters from state', () => {
    let s = {
      navbarTitle: 'Test title',
      isLoading: true,
      selectedIdeaIndex: 1,
      categories: [],
      userDataDB: null
    };

    expect(getters.navbarTitle(s)).toEqual(s.navbarTitle);
    expect(getters.isLoading(s)).toEqual(s.isLoading);
    expect(getters.selectedIdeaIndex(s)).toEqual(s.selectedIdeaIndex);
    expect(getters.categories(s)).toEqual(s.categories);
    expect(getters.userDataDB(s)).toEqual(s.userDataDB);
  });

  it('should correctly set Navbar title', () => {
    let title = 'Test title';
    mutations.SET_TITLE(state, title);
    expect(state.navbarTitle).toEqual(title);
  });

  it('should correctly set app state as loading', () => {
    let loading = true;
    mutations.SET_LOADING(state, loading);
    expect(state.isLoading).toEqual(loading);
  });

  it('should correctly set selected idea index', () => {
    let selectedIdeaIndex = 5;
    mutations.SET_SELECTED_IDEA_INDEX(state, selectedIdeaIndex);
    expect(state.selectedIdeaIndex).toEqual(selectedIdeaIndex);
  });

  it('should correctly set categories', () => {
    let categories = [{ id: 1 }, { id: 2 }];
    mutations.SET_CATEGORIES(state, categories);
    expect(state.categories.length).toEqual(categories.length);
    expect(state.categories[0].id).toEqual(categories[0].id);
  });

  it('should correctly set userData db', () => {
    let db = { id: 1 };
    mutations.SET_USER_DATA_DB(state, db);
    expect(state.userDataDB.id).toEqual(db.id);
  });

  it('should correctly call the SET_LOADING mutation', () => {
    actions.setLoading({ commit }, true);
    expect(commit).toHaveBeenCalledWith('SET_LOADING', true);
  });

  it('should correctly call the SET_CATEGORIES mutation', () => {
    actions.setCategories({ commit }, [1, 2]);
    expect(commit).toHaveBeenCalledWith('SET_CATEGORIES', [1, 2]);
  });

  it('should correctly call the SET_SELECTED_IDEA_INDEX mutation', () => {
    actions.setSelectedIdeaIndex({ commit }, 1);
    expect(commit).toHaveBeenCalledWith('SET_SELECTED_IDEA_INDEX', 1);
  });

  it('should correctly call the SET_TITLE mutation', () => {
    actions.setTitle({ commit }, 'Test title');
    expect(commit).toHaveBeenCalledWith('SET_TITLE', 'Test title');
  });

  it('should correctly call the SET_USER_DATA_DB mutation', () => {
    actions.setUserDataDB({ commit }, {});
    expect(commit).toHaveBeenCalledWith('SET_USER_DATA_DB', {});
  });
});
