import { shallowMount, createLocalVue } from '@vue/test-utils';
import Navbar from '../../../src/components/Navbar.vue';
import Vuex from 'vuex';
import StoreFactory from '../StoreFactory';

const localVue = createLocalVue();
localVue.use(Vuex);

describe('Navbar.vue', () => {
  let store;
  beforeEach(() => {
    store = StoreFactory({ navbarTitle: () => 'Ideabag 2 (TEST)' });
  });

  test('It should render app title correctly', () => {
    const wrapper = shallowMount(Navbar, {
      store,
      localVue,
      stubs: ['router-link', 'font-awesome-icon'],
      mocks: { $route: { path: '/' } }
    });

    expect(wrapper.find('#toolbar > h4').text()).toEqual('Ideabag 2 (TEST)');
  });

  test('It should not render back button if root route', () => {
    const wrapper = shallowMount(Navbar, {
      store,
      localVue,
      stubs: ['router-link', 'font-awesome-icon'],
      mocks: { $route: { path: '/' } }
    });

    expect(wrapper.find('#backBtn').exists()).toEqual(false);
  });

  test('It should render back button if not root route', () => {
    const wrapper = shallowMount(Navbar, {
      store,
      localVue,
      stubs: ['router-link', 'font-awesome-icon'],
      mocks: { $route: { path: '/categories/1' } }
    });

    expect(wrapper.find('#backBtn').exists()).toEqual(true);
  });

  test('It should render correct links when logged in', () => {
    let store = StoreFactory({
      userLoggedIn: () => true,
      userEmail: () => 'test@gmail.com'
    });

    const wrapper = shallowMount(Navbar, {
      store,
      localVue,
      stubs: ['router-link', 'font-awesome-icon'],
      mocks: { $route: { path: '/' } }
    });

    expect(wrapper.find('#links li:first-child').text()).toEqual(
      'Welcome, test@gmail.com!'
    );
    expect(wrapper.find('#links li:last-child').text()).toEqual('Log out');
  });

  test('It should render correct links when logged out', () => {
    let store = StoreFactory({
      userLoggedIn: () => false
    });

    const wrapper = shallowMount(Navbar, {
      store,
      localVue,
      stubs: ['router-link', 'font-awesome-icon'],
      mocks: { $route: { path: '/' } }
    });

    expect(wrapper.find('#links li:first-child').text()).toEqual('Login');
    expect(wrapper.find('#links li:last-child').text()).toEqual('Signup');
  });

  test('It should logout correctly', () => {
    let actions = {
      logout: jest.fn()
    };
    let store = StoreFactory({ userLoggedIn: () => true }, actions);

    let wrapper = shallowMount(Navbar, {
      store,
      localVue,
      stubs: ['router-link', 'font-awesome-icon'],
      mocks: { $route: { path: '/' } }
    });

    wrapper.find('#links li:last-child > a').trigger('click');
    expect(actions.logout).toHaveBeenCalled();
  });
});
