import { date } from 'quasar'
import { boot } from 'quasar/wrappers'

import { tokenHelper } from '../helpers/tokenHelper'

function isLoggedIn () : boolean {
  const token = tokenHelper.getToken()
  if (token === null) {
    return false
  }

  const tokenInfo = tokenHelper.parseToken(token)
  if (!tokenInfo) {
    return false
  }

  if (!tokenInfo.validAt) {
    return false
  }

  const tokenValidityInSeconds = date.getDateDiff(new Date(tokenInfo.validAt), new Date().toISOString(), 'seconds')

  if (tokenValidityInSeconds < 0) {
    console.log(`Token is expired ${tokenValidityInSeconds}`)
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
