import { boot } from 'quasar/wrappers'

import { tokenHelper } from '../helpers/tokenHelper'

function isLoggedIn () : boolean {
  const token = tokenHelper.getToken()
  if (token === null) {
    return false
  }

  return true
}

export default boot(({ router }) => {
  router.beforeEach(async (to, from, next) => {
    if (!isLoggedIn()) {
      if (to.path !== '/login') {
        router.push('/login')
      }
      next()
      return
    }

    if (isLoggedIn() && to.path === '/login') {
      router.push('/')
      next()
      return
    }

    next()
  })
})
