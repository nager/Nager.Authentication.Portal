import { date, LocalStorage } from 'quasar'

import { AuthenticationResponse } from 'src/models/AuthenticationResponse'
import { TokenInfo } from 'src/models/TokenInfo'

const authenticationTokenKey = 'authenticationToken'

function getToken () : string | null {
  return LocalStorage.getItem<string>(authenticationTokenKey)
}

function setToken (authenticationResponse : AuthenticationResponse) {
  LocalStorage.set(authenticationTokenKey, authenticationResponse.token)
}

function removeToken () {
  return LocalStorage.remove(authenticationTokenKey)
}

function parseToken (token: string) : TokenInfo | undefined {
  const parts = token.split('.')

  if (parts.length !== 3) {
    return
  }

  const tokenObject = JSON.parse(atob(parts[1]))

  const validAt = date.formatDate(new Date(tokenObject.exp * 1000), 'YYYY-MM-DDTHH:mm')

  const roles = []
  const roleKey = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
  if (Object.prototype.hasOwnProperty.call(tokenObject, roleKey)) {
    if (Array.isArray(tokenObject[roleKey])) {
      roles.push(...tokenObject[roleKey])
    } else {
      roles.push(tokenObject[roleKey])
    }

    console.log(tokenObject[roleKey])
  }

  return {
    emailAddress: tokenObject.unique_name,
    firstname: tokenObject.given_name,
    lastname: tokenObject.family_name,
    roles,
    validAt
  }
}

export const tokenHelper = {
  getToken,
  setToken,
  removeToken,
  parseToken
}
