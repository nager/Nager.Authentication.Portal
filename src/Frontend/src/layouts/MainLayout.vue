<script setup lang="ts">

import AuthenticatedAccount from '../components/AuthenticatedAccount.vue'

const baseUrl = document.baseURI

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
          stretch
          flat
          label="User Management"
          to="/usermanagement"
        />
        <q-btn
          stretch
          flat
          label="Swagger"
          :href="`${baseUrl}swagger`"
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
