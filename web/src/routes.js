const CategoryList = () => import('./views/CategoryList');
const IdeasList = () => import('./views/IdeasList');
const IdeaDetail = () => import('./views/IdeaDetail');
const Bookmarks = () => import('./views/Bookmarks');
const NewIdea = () => import('./views/NewIdea');
const Login = () => import('./views/Login');
const Register = () => import('./views/Register');

export const routes = [
  { path: '/', component: CategoryList },
  { path: '/categories/:categoryId', component: IdeasList, name: 'categories' },
  {
    path: '/categories/:categoryId/ideas/:ideaId',
    component: IdeaDetail,
    name: 'ideas'
  },
  { path: '/bookmarks', name: 'bookmarks', component: Bookmarks },
  { path: '/addidea', name: 'addidea', component: NewIdea},
  { path: '/login', name: 'login', component: Login },
  { path: '/register', name: 'register', component: Register },
  { path: '*', redirect: '/' }
];
