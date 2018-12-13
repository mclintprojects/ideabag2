import { mount } from '@vue/test-utils';
import Navbar from '../../src/components/Navbar.vue';

describe('Navbar.vue', () => {
  test('it should render content correctly', () => {
    const wrapper = mount(Navbar);
    expect(wrapper.find('#toolbar > h4').text()).toEqual('Ideabag 2 (BETA)');
  });
});
