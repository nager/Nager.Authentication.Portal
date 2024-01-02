<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { QTableProps, useQuasar } from 'quasar'

import { User } from 'src/models/User'

import { apiHelper } from '../helpers/apiHelper'

import DefaultDialog from './DefaultDialog.vue'
import UserEditForm from './UserEditForm.vue'
import UserRoleManagement from './UserRoleManagement.vue'
import UserAddForm from './UserAddForm.vue'

const $q = useQuasar()

const loading = ref<boolean>()
const users = ref<User[]>()
const editUser = ref<User>()
const showAddDialog = ref(false)
const showEditDialog = ref(false)

const columns : QTableProps['columns'] = [
  {
    name: 'emailAddress',
    required: true,
    label: 'Email Address',
    align: 'left',
    field: row => row.emailAddress,
    format: val => `${val}`
  },
  {
    name: 'roles',
    required: true,
    label: 'Roles',
    align: 'left',
    field: row => row.roles,
    format: val => `${val}`
  },
  {
    name: 'firstname',
    required: true,
    label: 'Firstname',
    align: 'left',
    field: row => row.firstname,
    format: val => `${val}`
  },
  {
    name: 'lastname',
    required: true,
    label: 'Lastname',
    align: 'left',
    field: row => row.lastname,
    format: val => `${val}`
  },
  {
    name: 'actions',
    required: true,
    label: 'Actions',
    align: 'right',
    field: 'actions'
  }
]

async function getUsers () {
  try {
    loading.value = true

    users.value = await apiHelper.getUsers()
  } finally {
    loading.value = false
  }
}

async function removeRow (row : User) {
  $q.dialog({
    title: 'Delete User',
    message: 'Do you really want to delete?',
    cancel: true,
    persistent: true
  }).onOk(async () => {
    if (await apiHelper.deleteUser(row.id)) {
      await getUsers()
    }
  })
}

async function editRow (row : User) {
  editUser.value = row
  showEditDialog.value = true
}

async function editDone () {
  await getUsers()
  editUser.value = users.value?.find(o => o.id === editUser.value?.id)
}

async function addDone () {
  showAddDialog.value = false
  await getUsers()
}

onMounted(async () => {
  await getUsers()
})

</script>

<template>
  <div class="text-right">
    <q-btn
      class="q-mb-sm"
      outline
      icon="person"
      label="Add User"
      @click="showAddDialog = true"
    />
  </div>

  <q-table
    flat
    bordered
    title="Users"
    :rows="users"
    :columns="columns"
    row-key="name"
    :loading="loading"
  >
    <template #body-cell-firstname="props">
      <q-td :props="props">
        {{ props.row.firstname }}
      </q-td>
    </template>

    <template #body-cell-lastname="props">
      <q-td :props="props">
        {{ props.row.lastname }}
      </q-td>
    </template>

    <template #body-cell-roles="props">
      <q-td :props="props">
        <q-badge
          v-for="role in props.row.roles"
          :key="role"
          outline
          color="blue"
          :label="role"
          class="q-mr-sm q-pa-sm"
        />
      </q-td>
    </template>

    <template #body-cell-actions="props">
      <q-td :props="props">
        <q-btn
          dense
          flat
          color="grey"
          icon="edit"
          @click="editRow(props.row)"
        />

        <q-btn
          dense
          flat
          color="grey"
          icon="delete"
          @click="removeRow(props.row)"
        />
      </q-td>
    </template>
  </q-table>

  <DefaultDialog
    :dialog-visible="showEditDialog"
    :padding="false"
    title="Edit User"
    @hide="showEditDialog = false"
  >
    <div class="text-subtitle2 q-pa-md bg-grey-4">
      Common
    </div>

    <div class="q-pa-md q-mb-md">
      <UserEditForm
        v-if="editUser"
        :user="editUser"
        @close="editDone()"
      />
    </div>

    <div class="text-subtitle2 q-pa-md bg-grey-4">
      Roles
    </div>
    <div class="q-pa-md">
      <UserRoleManagement
        v-if="editUser"
        class="q-mt-xl"
        :user="editUser"
        @role-changed="editDone()"
      />
    </div>
  </DefaultDialog>

  <DefaultDialog
    :dialog-visible="showAddDialog"
    title="Add User"
    @hide="showAddDialog = false"
  >
    <UserAddForm @close="addDone()" />
  </DefaultDialog>
</template>
