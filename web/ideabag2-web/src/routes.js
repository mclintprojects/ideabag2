import CategoryList from './components/CategoryList';
import IdeaList from './components/IdeaList';
import IdeaDetail from './components/IdeaDetail';

export const routes = [
	{ path: '', component: CategoryList },
	{ path: '/categories/:categoryId', component: IdeaList, name: 'categories' },
	{
		path: '/categories/:categoryId/ideas/:ideaId',
		component: IdeaDetail,
		name: 'ideas'
	},
	{ path: '*', redirect: '/' }
];
