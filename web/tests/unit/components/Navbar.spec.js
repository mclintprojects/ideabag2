import { shallowMount, createLocalVue } from '@vue/test-utils';
import Navbar from '../../../src/components/Navbar.vue';
import Vuex from 'vuex';

const localVue = createLocalVue();
localVue.use(Vuex);

describe('Navbar.vue', () => {
  let store;
  beforeEach(() => {
    store = new Vuex.Store({
      modules: {
        app: {
          getters: {
            navbarTitle: () => 'Ideabag 2 (TEST)'
          }
        }
      }
    });
  });

  test('It should render app title correctly', () => {
    const wrapper = shallowMount(Navbar, {
      store,
      localVue,
      stubs: ['router-link', 'font-awesome-icon'],
      mocks: { $route: { path: '/' } }
    });

    expect(wrapper.find('.toolbar-left h4').text()).toEqual('Ideabag 2 (TEST)');
  });

  test('It should not render back button if root route', () => {
    const wrapper = shallowMount(Navbar, {
      store,
      localVue,
      stubs: ['router-link', 'font-awesome-icon'],
      mocks: { $route: { path: '/' } }
    });

    expect(wrapper.find('.toolbar-left > .icon-button').exists()).toEqual(
      false
    );
  });

  test('It should render back button if not root route', () => {
    const wrapper = shallowMount(Navbar, {
      store,
      localVue,
      stubs: ['router-link', 'font-awesome-icon'],
      mocks: { $route: { path: '/categories/1' } }
    });

    expect(wrapper.find('.toolbar-left > .icon-button').exists()).toEqual(true);
  });
});
