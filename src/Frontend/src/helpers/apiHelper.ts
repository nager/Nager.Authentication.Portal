import { Notify } from 'quasar'

import { User } from 'src/models/User'
import { UserAdd } from 'src/models/UserAdd'
import { UserEdit } from 'src/models/UserEdit'

import { MfaError } from 'src/models/MfaError'
import { MfaSuccess } from 'src/models/MfaSuccess'
import { MfaResponse } from 'src/models/MfaResponse'
import { MfaInformation } from 'src/models/MfaInformation'

import { tokenHelper } from './tokenHelper'
import { CacheItem } from 'src/models/CacheItem'

const apiBaseUrl = '/auth/api/v1/'

async function getUsers () : Promise<User[]> {
  const token = tokenHelper.getToken()

  const response = await fetch(`${apiBaseUrl}UserManagement`, {
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  })

  if (response.status !== 200) {
    Notify.create({
      type: 'negative',
      message: response.statusText,
      caption: `${response.status} - Cannot load users`
    })

    return []
  }

  return await response.json() as User[]
}

async function createUser (payload : UserAdd) : Promise<boolean> {
  const token = tokenHelper.getToken()

  const response = await fetch(`${apiBaseUrl}UserManagement/`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(payload)
  })

  if (response.status === 201) {
    return true
  }

  const responseText = await response.text()
  Notify.create({
    type: 'negative',
    message: response.statusText,
    caption: responseText

  })

  return false
}

async function updateUser (userId : string, payload : UserEdit) : Promise<boolean> {
  const token = tokenHelper.getToken()

  const response = await fetch(`${apiBaseUrl}UserManagement/${userId}`, {
    method: 'PUT',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(payload)
  })

  if (response.status === 204) {
    return true
  }

  const responseText = await response.text()
  Notify.create({
    type: 'negative',
    message: response.statusText,
    caption: responseText
  })

  return false
}

async function deleteUser (userId : string) : Promise<boolean> {
  const token = tokenHelper.getToken()

  const response = await fetch(`${apiBaseUrl}UserManagement/${userId}`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  })

  if (response.status === 204) {
    return true
  }

  Notify.create({
    type: 'negative',
    message: 'Request failure',
    caption: response.statusText
  })

  return false
}

async function addRoleToUser (userId : string, roleName : string) : Promise<boolean> {
  const token = tokenHelper.getToken()

  const response = await fetch(`${apiBaseUrl}UserManagement/${userId}/Role`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      roleName
    })
  })

  if (response.status === 204) {
    return true
  }

  const responseText = await response.text()
  Notify.create({
    type: 'negative',
    message: response.statusText,
    caption: responseText

  })

  return false
}

async function removeRoleFromUser (userId : string, roleName : string) : Promise<boolean> {
  const token = tokenHelper.getToken()

  const response = await fetch(`${apiBaseUrl}UserManagement/${userId}/Role`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      roleName
    })
  })

  if (response.status === 204) {
    return true
  }

  const responseText = await response.text()
  Notify.create({
    type: 'negative',
    message: response.statusText,
    caption: responseText

  })

  return false
}

async function changePassword (newPassword : string) : Promise<boolean> {
  const token = tokenHelper.getToken()

  const response = await fetch(`${apiBaseUrl}UserAccount/ChangePassword`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      password: newPassword
    })
  })

  if (response.status !== 204) {
    console.error('cannot change password')
    Notify.create({
      type: 'negative',
      message: response.statusText,
      caption: 'Cannot change password'
    })

    return false
  }

  Notify.create({
    type: 'positive',
    message: 'Password changed'
  })

  return true
}

async function mfaInfo () : Promise<MfaInformation> {
  const token = tokenHelper.getToken()

  const response = await fetch(`${apiBaseUrl}UserAccount/Mfa`, {
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  })

  return await response.json() as MfaInformation
}

async function mfaActivate (mfaToken: string) : Promise<MfaResponse> {
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

async function mfaDeactivate (mfaToken: string) : Promise<MfaResponse> {
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

async function getCache () : Promise<CacheItem[]> {
  const token = tokenHelper.getToken()

  const response = await fetch(`${apiBaseUrl}Monitoring/Cache`, {
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  })

  if (response.status !== 200) {
    Notify.create({
      type: 'negative',
      message: response.statusText,
      caption: `${response.status} - Cannot load users`
    })

    return []
  }

  return await response.json() as CacheItem[]
}

export const apiHelper = {
  getUsers,
  createUser,
  updateUser,
  deleteUser,
  addRoleToUser,
  removeRoleFromUser,
  changePassword,
  mfaInfo,
  mfaActivate,
  mfaDeactivate,
  getCache
}
