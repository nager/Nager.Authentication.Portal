<script setup lang="ts">

import PasswordChangeBox from '../components/PasswordChangeBox.vue'
import MfaConfigurationBox from '../components/MfaConfigurationBox.vue'

</script>

<template>
  <q-page padding>
    <h1>Account</h1>

    <div class="row q-col-gutter-xl">
      <div class="col-12 col-md-5 col-lg-4">
        <PasswordChangeBox />
      </div>
      <div class="col-12 col-md-5 col-lg-4">
        <MfaConfigurationBox />
      </div>
    </div>
  </q-page>
</template>
