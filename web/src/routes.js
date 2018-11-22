import CategoryList from "./views/CategoryList";
import IdeasList from "./views/IdeasList";
import IdeaDetail from "./views/IdeaDetail";
import Bookmarks from "./views/Bookmarks";
import Login from "./views/Login";
import Register from "./views/Register";

export const routes = [
  { path: "", component: CategoryList },
  { path: "/categories/:categoryId", component: IdeasList, name: "categories" },
  {
    path: "/categories/:categoryId/ideas/:ideaId",
    component: IdeaDetail,
    name: "ideas"
  },
  { path: "/bookmarks", name: "bookmarks", component: Bookmarks },
  { path: "/login", name: "login", component: Login },
  { path: "/register", name: "register", component: Register },
  { path: "*", redirect: "/" }
];
