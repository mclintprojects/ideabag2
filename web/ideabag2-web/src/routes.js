import CategoryList from './components/CategoryList';
import IdeaList from './components/IdeaList';
import IdeaDetail from './components/IdeaDetail';

export const routes = [
	{ path: '', component: CategoryList },
	{ path: '/ideas', component: IdeaList },
	{ path: '/ideas/detail', component: IdeaDetail }
];
