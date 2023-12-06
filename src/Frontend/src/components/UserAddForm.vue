<script setup lang="ts">
import { ref } from 'vue'

import { UserAdd } from 'src/models/UserAdd'

import { apiHelper } from '../helpers/apiHelper'

const emit = defineEmits
<{(e: 'close'): void
}>()

const form = ref<UserAdd>({})

async function create () {
  if (await apiHelper.createUser(form.value)) {
    emit('close')
  }
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
      type="password"
      label="Password"
      outlined
    />
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
