<script setup lang="ts">

import PasswordChangeBox from '../components/PasswordChangeBox.vue'
import MfaConfigurationBox from '../components/MfaConfigurationBox.vue'

</script>

<template>
  <q-page padding>
    <h1>Account</h1>

    <div class="row q-col-gutter-md">
      <div class="col-6">
        <PasswordChangeBox />
      </div>
      <div class="col-6">
        <MfaConfigurationBox />
      </div>
    </div>
  </q-page>
</template>
