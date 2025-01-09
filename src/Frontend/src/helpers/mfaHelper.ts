const apiBaseUrl = '/auth/api/v1/'

import { MfaError } from 'src/models/MfaError'
import { MfaSuccess } from 'src/models/MfaSuccess'
import { MfaResponse } from 'src/models/MfaResponse'
import { MfaInformation } from 'src/models/MfaInformation'

import { tokenHelper } from './tokenHelper'

export class MfaHelper {
  async getStatus () : Promise<MfaInformation> {
    const token = tokenHelper.getToken()

    const response = await fetch(`${apiBaseUrl}UserAccount/Mfa`, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    })

    return await response.json() as MfaInformation
  }

  async activate (mfaToken: string) : Promise<MfaResponse> {
    const token = tokenHelper.getToken()

    const response = await fetch(`${apiBaseUrl}UserAccount/Mfa/Activate`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        token: mfaToken
      })
    })

    if (response.status === 204) {
      return { success: true } as MfaSuccess
    }

    return await response.json() as MfaError
  }

  async deactivate (mfaToken: string) : Promise<MfaResponse> {
    const token = tokenHelper.getToken()

    const response = await fetch(`${apiBaseUrl}UserAccount/Mfa/Deactivate`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        token: mfaToken
      })
    })

    if (response.status === 204) {
      return { success: true } as MfaSuccess
    }

    return await response.json() as MfaError
  }
}
