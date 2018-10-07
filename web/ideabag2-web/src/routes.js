import CategoryList from './components/CategoryList';
import IdeaList from './components/IdeaList';
import IdeaDetail from './components/IdeaDetail';
import Bookmarks from "./components/Bookmarks";
import Login from './components/Login';
import Register from './components/Register';

export const routes = [
	{ path: '', component: CategoryList },
	{ path: '/categories/:categoryId', component: IdeaList, name: 'categories' },
	{
		path: '/categories/:categoryId/ideas/:ideaId',
		component: IdeaDetail,
		name: 'ideas'
	},
	{ path: "/bookmarks", name: "bookmarks", component: Bookmarks},
	{ path: '/login', name: 'login', component: Login },
	{ path: '/register', name: 'register', component: Register },
	{ path: '*', redirect: '/' }
];
