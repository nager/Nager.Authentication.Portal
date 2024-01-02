<script setup lang="ts">
import { ref } from 'vue'
import type { PropType } from 'vue'

import { User } from 'src/models/User'

import { apiHelper } from '../helpers/apiHelper'

const props = defineProps({
  user: {
    type: Object as PropType<User>,
    required: true
  }
})

const emit = defineEmits
<{(e: 'roleChanged'): void
}>()

const newRoleName = ref<string>()

async function addRoleToUser () {
  if (!newRoleName.value) {
    return
  }

  if (await apiHelper.addRoleToUser(props.user.id, newRoleName.value)) {
    emit('roleChanged')
    newRoleName.value = ''
  }
}

async function removeRoleFromUser (roleName : string) {
  if (await apiHelper.removeRoleFromUser(props.user.id, roleName)) {
    emit('roleChanged')
  }
}

</script>

<template>
  <q-input
    v-model="newRoleName"
    class="q-mb-md"
    label="Add new Role"
    outlined
  >
    <template #append>
      <q-btn
        dense
        flat
        icon="add"
        @click="addRoleToUser()"
      />
    </template>
  </q-input>

  <q-card
    v-if="user.roles && user.roles.length > 0"
    flat
    bordered
  >
    <q-card-section class="q-pa-none">
      <q-list separator>
        <q-item-label header>
          Current Roles
        </q-item-label>
        <q-separator />
        <q-item
          v-for="role in user.roles"
          :key="role"
        >
          <q-item-section>
            <q-item-label>{{ role }}</q-item-label>
          </q-item-section>

          <q-item-section side>
            <q-btn
              flat
              dense
              outline
              icon="delete"
              @click="removeRoleFromUser(role)"
            />
          </q-item-section>
        </q-item>
      </q-list>
    </q-card-section>
  </q-card>
</template>
