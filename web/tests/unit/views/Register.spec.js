import { shallowMount } from '@vue/test-utils';
import StoreFactory from '../StoreFactory';
import Register from '../../../src/views/Register';

describe('Register.vue', () => {
  let store, actions;

  beforeEach(() => {
    actions = {
      registerUser: jest.fn()
    };
    store = StoreFactory({}, actions);
  });

  test('It should attempt login if username and password are provided', () => {
    const wrapper = shallowMount(Register, {
      store,
      stubs: ['font-awesome-icon']
    });

    wrapper.find('#emailTb').setValue('test@gmail.com');
    wrapper.find('#passwordTb').setValue('p@ssword');
    wrapper.find('.appBtn').trigger('click');

    expect(actions.registerUser).toHaveBeenCalled();
  });
});
