import { RouteRecordRaw } from 'vue-router'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      {
        path: '',
        component: () => import('pages/IndexPage.vue')
      },
      {
        path: 'account',
        component: () => import('pages/AccountPage.vue')
      },
      {
        path: 'usermanagement',
        component: () => import('pages/UserManagementPage.vue')
      },
      {
        path: 'monitoring',
        component: () => import('pages/MonitoringPage.vue')
      }
    ]
  },
  {
    path: '/login',
    component: () => import('layouts/AuthenticationLayout.vue'),
    children: [{ path: '', component: () => import('pages/LoginPage.vue') }]
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue')
  }
]

export default routes
