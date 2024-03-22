<script setup lang="ts">
import { ref } from 'vue'

import { UserAdd } from 'src/models/UserAdd'

import { apiHelper } from '../helpers/apiHelper'

const emit = defineEmits
<{(e: 'close'): void
}>()

const isPassword = ref(true)
const form = ref<UserAdd>({})

async function create () {
  if (await apiHelper.createUser(form.value)) {
    emit('close')
  }
}

function createPassword () {
  const length = 20
  const allowedCharacters = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+!@-#.'

  const randomPassword = Array.from(crypto.getRandomValues(new Uint32Array(length)))
    .map((x) => allowedCharacters[x % allowedCharacters.length])
    .join('')

  form.value.password = randomPassword
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
          icon="password"
          title="Create random password"
          @click="createPassword"
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
      label="Save"
      outline
      @click="create"
    />
  </q-form>
</template>
