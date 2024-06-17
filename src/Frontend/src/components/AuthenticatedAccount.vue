<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'

import { tokenHelper } from '../helpers/tokenHelper'
import { AuthenticationHelper } from '../helpers/authenticationHelper'

const authenticationHelper = new AuthenticationHelper()
const Router = useRouter()

const tokenInfo = computed(() => {
  const token = tokenHelper.getToken()

  if (token === null) {
    return
  }

  return tokenHelper.parseToken(token)
})

async function logout () {
  authenticationHelper.logout()
  await Router.push('/login')
}

</script>

<template>
  <q-btn-dropdown
    icon="account_circle"
    stretch
    flat
    label="Account"
  >
    <div
      v-if="tokenInfo"
      class="text-subtitle1 q-ma-md"
    >
      <div v-if="tokenInfo.firstname || tokenInfo.lastname">
        {{ tokenInfo.firstname }} {{ tokenInfo.lastname }}
      </div>
      <div class="text-weight-medium">
        {{ tokenInfo.emailAddress }}
      </div>
      <small style="line-height:14px; display:block;">
        Token valid at<br>
        {{ tokenInfo.validAt }}
      </small>
      <q-badge
        v-for="role in tokenInfo.roles"
        :key="role"
        outline
        color="primary"
        class="q-mr-sm"
        :label="role"
      />
    </div>
    <div>
      <q-btn
        v-close-popup
        to="/account"
        square
        icon="person"
        label="My Account"
        class="full-width q-pa-md"
        flat
      />
      <q-btn
        v-close-popup
        square
        icon="logout"
        label="Logout"
        class="full-width q-pa-md"
        flat
        @click="logout()"
      />
    </div>
  </q-btn-dropdown>
</template>
