import { shallowMount, createLocalVue } from '@vue/test-utils';
import StoreFactory from '../StoreFactory';
import Login from '../../../src/views/Login';
import Toasted from 'vue-toasted';
import Vue from 'vue';

Vue.use(Toasted);

describe('Login.vue', () => {
  let store, actions;

  beforeEach(() => {
    actions = {
      loginUser: jest.fn()
    };
    store = StoreFactory({}, actions);
  });

  test('It should attempt login if username and password are provided', () => {
    const wrapper = shallowMount(Login, {
      store,
      stubs: ['font-awesome-icon']
    });

    wrapper.find('#emailTb').setValue('test@gmail.com');
    wrapper.find('#passwordTb').setValue('p@ssword');
    wrapper.find('.appBtn').trigger('click');

    expect(actions.loginUser).toHaveBeenCalled();
  });

  /*test('It should not attempt login if username or password is not provided', () => {
    const wrapper = shallowMount(Login, {
      store,
      stubs: ['font-awesome-icon']
    });

    wrapper.find('#emailTb').setValue('');
    wrapper.find('#passwordTb').setValue('');
    wrapper.find('.appBtn').trigger('click');

    expect(actions.loginUser).toHaveBeenCalledTimes(0);
  });*/
});
