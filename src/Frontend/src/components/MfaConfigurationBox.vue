<script setup lang="ts">
import { ref, onMounted } from 'vue'

import { apiHelper } from '../helpers/apiHelper'

const mfa = ref()
const token = ref('')

onMounted(async () => {
  mfa.value = await apiHelper.mfa()
})

function activate () {
  apiHelper.mfaActivate(token.value)
}

function deactivate () {
  apiHelper.mfaDeactivate(token.value)
}

</script>

<template>
  <h2>Multi-factor authentication</h2>

  <div v-if="mfa">
    <q-checkbox
      v-model="mfa.isActive"
      disable
      label="multi-factor authentication active"
    />
    <br>

    <img :src="mfa.activationQrCode">

    <q-input
      v-model="token"
      outlined
      label="Activation code"
      hint="Enter the code from your authenticator app"
    />
    <div class="q-mt-md">
      <q-btn
        v-if="!mfa.isActive"
        color="black"
        outline
        label="Activate"
        @click="activate()"
      />
      <q-btn
        v-if="mfa.isActive"
        color="black"
        outline
        label="DEactivate"
        @click="deactivate()"
      />
    </div>
  </div>
</template>
