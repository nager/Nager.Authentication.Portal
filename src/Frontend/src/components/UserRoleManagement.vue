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
  }
}

async function removeRoleFromUser (roleName : string) {
  if (await apiHelper.removeRoleFromUser(props.user.id, roleName)) {
    emit('roleChanged')
  }
}

</script>

<template>
  <div class="text-subtitle1 q-mt-xl">
    Roles
  </div>
  <q-card
    flat
    bordered
  >
    <q-card-section class="q-pa-none">
      <q-list
        v-if="user.roles && user.roles.length > 0"
        separator
      >
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
              outline
              icon="delete"
              @click="removeRoleFromUser(role)"
            />
          </q-item-section>
        </q-item>
      </q-list>
    </q-card-section>
  </q-card>

  <div class="text-subtitle2 q-mt-md">
    Add Role
  </div>
  <q-input
    v-model="newRoleName"
    label="Role"
    outlined
  />
  <q-btn
    class="q-mt-sm"
    label="Add"
    outline
    @click="addRoleToUser()"
  />
</template>
