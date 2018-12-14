import { shallowMount, createLocalVue } from '@vue/test-utils';
import IdeaList from '../../../src/components/IdeaList';
import Vuex from 'vuex';
import StoreFactory from '../StoreFactory';
import { wrap } from 'module';

let localVue = createLocalVue();
localVue.use(Vuex);

global.window.matchMedia = () => {
  return { matches: true };
};

describe('IdeaList.vue', () => {
  let store, ideas;

  beforeEach(() => {
    store = StoreFactory({
      selectedIdeaIndex: () => 0
    });
    ideas = [
      {
        title: 'Test title',
        difficulty: 'Beginner',
        progress: 'done',
        bookmarked: true
      }
    ];
  });

  test('It should correctly render ideas', () => {
    const wrapper = shallowMount(IdeaList, {
      store,
      localVue,
      stubs: ['popper', 'progress-modal', 'font-awesome-icon'],
      propsData: { ideas }
    });

    expect(wrapper.findAll('.ideaItem').length).toEqual(ideas.length);
    expect(wrapper.find('#ideaTitle').text()).toEqual(ideas[0].title);
    expect(wrapper.find('#ideaDifficulty').text()).toEqual(ideas[0].difficulty);
    expect(
      wrapper
        .find('#ideaList li:first-child')
        .classes()
        .find(e => e == 'progress-done')
    ).toBeDefined();
  });
});
