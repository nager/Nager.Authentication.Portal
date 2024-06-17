<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useQuasar, QStepper } from 'quasar'

import { MfaHelper } from '../helpers/mfaHelper'

import { instanceOfMfaError } from 'src/models/MfaResponse'
import { MfaError } from 'src/models/MfaError'
import { MfaInformation } from 'src/models/MfaInformation'

const $q = useQuasar()

const mfaHelper = new MfaHelper()

const step = ref(1)
const stepper = ref<QStepper>()
const mfa = ref<MfaInformation>()
const token = ref('')
const processing = ref(false)

onMounted(async () => {
  await getStatus()
})

async function getStatus () {
  mfa.value = await mfaHelper.getStatus()
}

async function activate () {
  try {
    processing.value = true
    const mfaResponse = await mfaHelper.activate(token.value)

    if (instanceOfMfaError(mfaResponse)) {
      $q.notify({
        type: 'negative',
        caption: (mfaResponse as MfaError).error
      })
    }
  } catch (e) {
    console.error(e)
  } finally {
    processing.value = false

    await getStatus()

    token.value = ''
    step.value = 1
  }
}

async function deactivate () {
  try {
    processing.value = true
    const mfaResponse = await mfaHelper.deactivate(token.value)

    if (instanceOfMfaError(mfaResponse)) {
      $q.notify({
        type: 'negative',
        caption: (mfaResponse as MfaError).error
      })
    }

    token.value = ''
  } catch (e) {
    console.error(e)
  } finally {
    processing.value = false
  }
  await getStatus()
}

async function stepperNext () {
  stepper.value?.next()

  if (step.value === 3) {
    await activate()
  }
}

function stepperPrevious () {
  stepper.value?.previous()
}

</script>

<template>
  <h2>Multi-factor authentication</h2>

  <div v-if="mfa">
    <q-stepper
      v-if="!mfa.isActive"
      ref="stepper"
      v-model="step"
      flat
      bordered
      color="primary"
    >
      <q-step
        :name="1"
        title="Scan code"
        icon="qr_code"
        active-icon="qr_code"
        :done="step > 1"
        class="text-center"
      >
        <div class="q-mb-md">
          Please scan the QR code with your Authenticator app to complete the MFA setup.
        </div>
        <img :src="mfa.activationQrCode">
      </q-step>

      <q-step
        :name="2"
        title="Activate"
        icon="password"
        :done="step > 2"
      >
        <q-form @submit.prevent="activate()">
          <q-input
            v-model="token"
            outlined
            label="Activation code"
            hint="Enter the code from your authenticator app"
          />
        </q-form>
      </q-step>

      <template #navigation>
        <q-stepper-navigation>
          <q-btn
            v-if="step < 2"
            outline
            color="black"
            label="Continue"
            @click="stepperNext()"
          />
          <q-btn
            v-if="step === 2"
            outline
            color="black"
            label="Activate"
            :loading="processing"
            @click="activate()"
          />
          <q-btn
            v-if="step > 1"
            outline
            color="black"
            label="Back"
            class="q-ml-sm"
            @click="stepperPrevious()"
          />
        </q-stepper-navigation>
      </template>
    </q-stepper>
    <div v-else>
      <q-card
        flat
        bordered
      >
        <q-card-section class="mfa-box bg-green q-pa-md rounded-borders">
          <q-icon
            name="security"
            color="white"
            size="3rem"
            class="q-pr-md"
          />
          MFA is active
        </q-card-section>
        <q-card-section>
          To deactivate MFA, please provide another code.
          <q-form @submit.prevent="deactivate()">
            <q-input
              v-model="token"
              outlined
              dense
              label="Deactivation code"
              hint="Enter the code from your authenticator app"
            />
            <q-btn
              type="submit"
              class="q-mt-md"
              color="black"
              outline
              label="Deactivate"
              :loading="processing"
            />
          </q-form>
        </q-card-section>
      </q-card>
    </div>
  </div>
</template>

<style scoped>
.mfa-box {
  font-size: 2rem;
  color: white;
}
</style>
