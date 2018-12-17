import app from '../../../src/store/app';

describe('app.js', () => {
  let state, mutations;

  beforeAll(() => {
    state = app.state;
    mutations = app.mutations;
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
});
