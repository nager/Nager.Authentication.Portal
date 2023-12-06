<script setup lang="ts">
import { ref, onMounted } from 'vue'
import type { PropType } from 'vue'

import { User } from 'src/models/User'
import { UserEdit } from 'src/models/UserEdit'

import { apiHelper } from '../helpers/apiHelper'

const props = defineProps({
  user: {
    type: Object as PropType<User>,
    required: true
  }
})

const emit = defineEmits
<{(e: 'close'): void
}>()

const form = ref<UserEdit>({})

onMounted(() => {
  if (!form.value) {
    return
  }

  form.value.firstname = props.user.firstname
  form.value.lastname = props.user.lastname
})

async function updateUser () {
  if (await apiHelper.updateUser(props.user.id, form.value)) {
    emit('close')
  }
}

</script>

<template>
  <q-form
    v-if="form"
    class="q-gutter-sm"
  >
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
      @click="updateUser()"
    />
  </q-form>
</template>
