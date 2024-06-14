<script setup lang="ts">
import { ref } from 'vue'
import { useQuasar } from 'quasar'

import { apiHelper } from '../helpers/apiHelper'

const $q = useQuasar()

const newPassword = ref<string | undefined>()
const newPasswordConfirm = ref<string | undefined>()

async function changePassword () {
  if (!newPassword.value) {
    return
  }

  if (newPassword.value !== newPasswordConfirm.value) {
    $q.notify({
      type: 'negative',
      message: 'Passwords do not match',
      caption: 'Please re-enter the passwords'
    })

    return
  }

  const success = await apiHelper.changePassword(newPassword.value)

  if (!success) {
    $q.notify({
      type: 'negative',
      // message: success.statusText,
      caption: 'Cannot change password'
    })

    return
  }

  $q.notify({
    type: 'positive',
    message: 'Password changed'
  })

  newPassword.value = undefined
  newPasswordConfirm.value = undefined
}
</script>

<template>
  <h2>Password</h2>
  <div class="text-caption">
    Choose a strong password that you do not use for other accounts.
  </div>

  <q-form>
    <q-input
      v-model="newPassword"
      label="NewPassword"
      autocomplete="new-password"
      type="password"
      class="q-mb-sm text-white bg-white"
      outlined
    />
    <q-input
      v-model="newPasswordConfirm"
      label="Confirm NewPassword"
      autocomplete="new-password"
      type="password"
      class="q-mb-sm text-white bg-white"
      outlined
    />
    <q-btn
      color="black"
      outline
      label="Change"
      @click="changePassword()"
    />
  </q-form>
</template>
