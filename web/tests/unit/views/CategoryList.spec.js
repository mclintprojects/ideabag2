import { shallowMount } from '@vue/test-utils';
import StoreFactory from '../StoreFactory';
import CategoryList from '../../../src/views/CategoryList';

describe('CategoryList.vue', () => {
  let store, wrapper;
  let $router = {
    push: jest.fn()
  };
  let categories = [
    {
      id: 1,
      categoryLbl: 'Test category title',
      categoryCount: 10,
      items: []
    }
  ];

  beforeEach(() => {
    store = StoreFactory({ categories: () => categories });
    wrapper = shallowMount(CategoryList, {
      store,
      stubs: ['router-link', 'font-awesome-icon'],
      mocks: { $router }
    });
  });

  test('It should render the list of categories correctly', () => {
    expect(wrapper.findAll('.categoryItem').length).toEqual(categories.length);
    expect(
      wrapper
        .find('#categoryList .categoryItem:first-child #categoryTitle')
        .text()
    ).toEqual(categories[0].categoryLbl);
    expect(
      wrapper
        .find('#categoryList .categoryItem:first-child .secondaryLbl')
        .text()
    ).toEqual(`Ideas: ${categories[0].categoryCount}`);
  });

  test('It should correctly navigate to category detail view', () => {
    wrapper.find('#categoryList li:first-child').trigger('click');
    expect(wrapper.vm.selectedIndex).toEqual(0);
    expect($router.push).toHaveBeenCalled();
  });
});
