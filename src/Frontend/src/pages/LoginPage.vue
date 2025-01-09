<script setup lang="ts">
import { ref } from 'vue'
import { useQuasar } from 'quasar'
import { useRouter } from 'vue-router'

import { LoginAction } from 'src/models/LoginAction'

import { AuthenticationHelper } from '../helpers/authenticationHelper'

const authenticationHelper = new AuthenticationHelper()

const $q = useQuasar()
const Router = useRouter()

const loading = ref(false)
const emailAddress = ref('')
const password = ref('')
const oneTimePasswordRequired = ref(false)
const totpToken = ref<undefined | string>(undefined)

async function login () {
  loading.value = true

  try {
    const loginAction = await authenticationHelper.login(emailAddress.value, password.value)
    switch (loginAction) {
      case LoginAction.Forward:
        await Router.push('/')
        break
      case LoginAction.TimeBasedOneTimePasswordRequired:
        oneTimePasswordRequired.value = true
        break
      case LoginAction.ClearPassword:
        password.value = ''
        break
      case LoginAction.Failure:
        $q.notify({
          type: 'negative',
          message: 'Endpoint failure',
          caption: 'Not Available'
        })
        break
      default:
        break
    }
  } finally {
    loading.value = false
  }
}

async function tokenLogin () {
  if (!totpToken.value) {
    return
  }

  loading.value = true

  try {
    const loginAction = await authenticationHelper.tokenLogin(totpToken.value)
    switch (loginAction) {
      case LoginAction.Forward:
        await Router.push('/')
        break
      default:
        break
    }
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <q-layout>
    <q-page-container>
      <q-page class="row bg-primary justify-center items-center">
        <div class="column">
          <div class="text-center text-white q-mb-md">
            <span class="text-h4 text-uppercase">
              NAGER
            </span>
            <div class="text-weight-thin text-uppercase">
              Authentication
            </div>
          </div>
          <div>
            <q-spinner-cube
              v-if="loading"
              color="white"
              size="12em"
            />
            <q-card
              v-else
              square
              bordered
              class="q-pa-lg shadow-1"
            >
              <template v-if="oneTimePasswordRequired">
                <q-form @submit.prevent="tokenLogin()">
                  <q-card-section class="q-gutter-md">
                    <q-input
                      v-model="totpToken"
                      square
                      filled
                      type="text"
                      autocomplete="one-time-password"
                      label="One Time Password"
                    />
                  </q-card-section>
                  <q-card-actions class="q-px-md">
                    <q-btn
                      type="submit"
                      unelevated
                      color="primary"
                      size="lg"
                      class="full-width"
                      label="Login"
                    />
                  </q-card-actions>
                </q-form>
              </template>
              <template v-else>
                <q-form @submit.prevent="login()">
                  <q-card-section class="q-gutter-md">
                    <q-input
                      v-model="emailAddress"
                      square
                      filled
                      type="email"
                      autocomplete="email"
                      label="Email"
                    />
                    <q-input
                      v-model="password"
                      square
                      filled
                      type="password"
                      autocomplete="current-password"
                      label="Password"
                    />
                  </q-card-section>
                  <q-card-actions class="q-px-md">
                    <q-btn
                      type="submit"
                      unelevated
                      color="primary"
                      size="lg"
                      class="full-width"
                      label="Login"
                    />
                  </q-card-actions>
                </q-form>
              </template>
            </q-card>
          </div>
          <div class="q-mt-sm text-white text-right">
            <a
              class="text-white"
              href="https://github.com/nager/Nager.AuthenticationService"
            >Nager.AuthenticationService</a>
          </div>
        </div>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<style scoped>
.q-card {
  width: 360px;
}
</style>
