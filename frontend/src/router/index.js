import { createRouter, createWebHistory } from 'vue-router'

import Login from '../views/Login.vue'
import Register from '../views/Register.vue'
import Dashboard from '../views/Dashboard.vue'
import Products from '../views/Products.vue'
import Inventory from '../views/Inventory.vue'

const routes = [
  {
    path: '/login',
    component: Login,
    meta: { guestOnly: true }
  },
  {
    path: '/register',
    component: Register,
    meta: { guestOnly: true }
  },
  {
    path: '/dashboard',
    component: Dashboard,
    meta: { requiresAuth: true }
  },
  {
    path: '/products',
    component: Products,
    meta: { requiresAuth: true }
  },
  {
    path: '/inventory',
    component: Inventory,
    meta: { requiresAuth: true }
  },
  {
    path: '/',
    redirect: '/dashboard'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to) => {
  const token = localStorage.getItem('token')

  // User is already logged in
  // Don't allow them to visit login/register
  if (to.meta.guestOnly && token) {
    return '/dashboard'
  }

  // User is not logged in
  // Don't allow access to protected pages
  if (to.meta.requiresAuth && !token) {
    return '/login'
  }

  return true
})

export default router