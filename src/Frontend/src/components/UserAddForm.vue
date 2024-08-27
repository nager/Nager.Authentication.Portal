<script setup lang="ts">
import { ref, onMounted } from 'vue'

import { copyToClipboard } from 'quasar'

import { UserAdd } from 'src/models/UserAdd'

import { apiHelper } from '../helpers/apiHelper'

const emit = defineEmits
<{(e: 'close'): void
}>()

const isPassword = ref(true)
const form = ref<UserAdd>({})

onMounted(() => {
  createSecurePassword()
})

async function createUser () {
  if (await apiHelper.createUser(form.value)) {
    emit('close')
  }
}

function copyCredentials () {
  copyToClipboard(`Username: ${form.value.emailAddress}\r\nPassword: ${form.value.password}`)
}

function createSecurePassword () {
  const length = 20

  const allowedCharacters = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+!#_-.'
  const letters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'

  const randomPassword = Array.from(crypto.getRandomValues(new Uint32Array(length - 2)))
    .map((x) => allowedCharacters[x % allowedCharacters.length])
    .join('')

  const firstChar = letters[Math.floor(Math.random() * letters.length)]
  const lastChar = letters[Math.floor(Math.random() * letters.length)]

  form.value.password = `${firstChar}${randomPassword}${lastChar}`
}

</script>

<template>
  <q-form class="q-gutter-sm">
    <q-input
      v-model="form.emailAddress"
      type="email"
      autocomplete="email"
      label="Email Address"
      outlined
      autofocus
    />
    <q-input
      v-model="form.password"
      :type="isPassword ? 'password' : 'text'"
      label="Password"
      outlined
    >
      <template #append>
        <q-icon
          :name="isPassword ? 'visibility_off' : 'visibility'"
          class="cursor-pointer"
          @click="isPassword = !isPassword"
        />
      </template>
      <template #after>
        <q-btn
          dense
          stretch
          flat
          icon="refresh"
          title="Create random password"
          @click="createSecurePassword"
        />
      </template>
    </q-input>

    <q-input
      v-model="form.firstname"
      label="Firstname"
      outlined
    />
    <q-input
      v-model="form.lastname"
      label="Lastname"
      outlined
    />
    <q-btn
      label="Copy credentials"
      icon="content_copy"
      outline
      @click="copyCredentials"
    />
    <q-btn
      label="Create User"
      outline
      @click="createUser"
    />
  </q-form>
</template>
