<script setup lang="ts">
import { computed } from 'vue'
import { tokenHelper } from '../helpers/tokenHelper'

import AuthenticatedAccount from '../components/AuthenticatedAccount.vue'

const isAdministrator = computed(() => {
  const token = tokenHelper.getToken()
  if (!token) {
    return false
  }

  const tokenInfo = tokenHelper.parseToken(token)
  if (!tokenInfo) {
    return false
  }

  return tokenInfo.roles?.map(role => role.toLocaleLowerCase())?.includes('administrator')
})

</script>

<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated>
      <q-toolbar>
        <div
          class="q-mr-sm cursor-pointer"
          style="position:relative;"
          @click="$router.push('/')"
        >
          <q-toolbar-title shrink>
            Nager Authentication
          </q-toolbar-title>
        </div>

        <q-btn
          v-if="isAdministrator"
          stretch
          flat
          label="User Management"
          to="/usermanagement"
        />
        <q-btn
          v-if="isAdministrator"
          stretch
          flat
          label="Monitoring"
          to="/monitoring"
        />
        <q-btn
          stretch
          flat
          label="Swagger"
          href="/swagger"
        />

        <q-space />

        <AuthenticatedAccount />
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>
