import { LoginAction } from 'src/models/LoginAction'
import { AuthenticationResponse } from 'src/models/AuthenticationResponse'
import { AuthenticationMfaResponse } from 'src/models/AuthenticationMfaResponse'

import { tokenHelper } from './tokenHelper'

const apiBaseUrl = '/auth/api/v1/'

export class AuthenticationHelper {
  mfaIdentifier: string | undefined

  constructor () {
    this.mfaIdentifier = undefined
  }

  public async tokenLogin (token : string) : Promise<LoginAction> {
    const response = await fetch(`${apiBaseUrl}Authentication/Token`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        mfaIdentifier: this.mfaIdentifier,
        token
      })
    })

    if (response.status === 200) {
      const authenticationResponse = await response.json() as AuthenticationResponse
      tokenHelper.setToken(authenticationResponse.token)

      this.mfaIdentifier = undefined

      return LoginAction.Forward
    }

    return LoginAction.ClearPassword
  }

  public async login (emailAddress : string, password : string) : Promise<LoginAction> {
    const response = await fetch(`${apiBaseUrl}Authentication`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        emailAddress,
        password
      })
    })

    if (response.status === 200) {
      const authenticationResponse = await response.json() as AuthenticationResponse
      tokenHelper.setToken(authenticationResponse.token)

      return LoginAction.Forward
    }

    if (response.status === 401) {
      const authenticationMfaResponse = await response.json() as AuthenticationMfaResponse
      this.mfaIdentifier = authenticationMfaResponse.mfaIdentifier

      return LoginAction.TimeBasedOneTimePasswordRequired
    }

    if (response.status === 404) {
      return LoginAction.Failure
    }

    if (response.status === 504) {
      return LoginAction.Failure
    }

    return LoginAction.ClearPassword
  }

  logout () {
    tokenHelper.removeToken()
  }
}
