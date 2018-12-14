import { shallowMount } from '@vue/test-utils';
import StoreFactory from '../StoreFactory';
import Login from '../../../src/views/Login';

describe('Login.vue', () => {
  let store;
  let actions = {
    loginUser: jest.fn()
  };

  beforeEach(() => {
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
});
