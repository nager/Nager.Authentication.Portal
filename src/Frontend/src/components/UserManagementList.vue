<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { QTableProps, useQuasar, date } from 'quasar'

import { User } from 'src/models/User'

import { apiHelper } from '../helpers/apiHelper'

import DefaultDialog from './DefaultDialog.vue'
import UserEditForm from './UserEditForm.vue'
import UserRoleManagement from './UserRoleManagement.vue'
import UserAddForm from './UserAddForm.vue'

const $q = useQuasar()

const loading = ref<boolean>()
const filter = ref<string>('')
const users = ref<User[]>([])
const editUser = ref<User>()
const showAddDialog = ref(false)
const showEditDialog = ref(false)

const filteredUsers = computed(() => {
  if (!users.value) {
    return []
  }

  if (!filter.value) {
    return users.value
  }

  return users.value.filter(o => {
    return o.emailAddress.toLowerCase().includes(filter.value.toLowerCase())
  })
})

const columns : QTableProps['columns'] = [
  {
    name: 'emailAddress',
    required: true,
    label: 'Email Address',
    align: 'left',
    field: row => row.emailAddress,
    format: val => `${val}`,
    style: 'width: 300px'
  },
  {
    name: 'name',
    required: true,
    label: 'Name',
    align: 'left',
    field: row => row,
    format: row => `${row.lastname} ${row.firstname}`,
    style: 'width: 300px'
  },
  {
    name: 'roles',
    required: true,
    label: 'Roles',
    align: 'left',
    field: row => row.roles
  },
  {
    name: 'lastFailedValidationTimestamp',
    required: true,
    label: 'Last Failed Validation',
    align: 'left',
    field: row => row.lastFailedValidationTimestamp,
    format: val => date.formatDate(val, 'YYYY-MM-DD HH:mm'),
    style: 'width: 180px'
  },
  {
    name: 'lastSuccessfulValidationTimestamp',
    required: true,
    label: 'Last Successful Validation',
    align: 'left',
    field: row => row.lastSuccessfulValidationTimestamp,
    format: val => date.formatDate(val, 'YYYY-MM-DD HH:mm'),
    style: 'width: 180px'
  },
  {
    name: 'mfaActive',
    required: true,
    label: 'Mfa Active',
    align: 'left',
    field: row => row.mfaActive,
    style: 'width: 180px'
  },
  {
    name: 'actions',
    required: true,
    label: 'Actions',
    align: 'right',
    field: 'actions',
    style: 'width: 120px'
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
  <q-table
    flat
    bordered
    :rows="filteredUsers"
    :rows-per-page-options="[20, 50, 0]"
    :columns="columns"
    row-key="emailAddress"
    :loading="loading"
  >
    <template #top>
      <q-input
        v-model="filter"
        outlined
        dense
        debounce="100"
        color="primary"
        style="width: 400px"
      >
        <template #append>
          <q-icon name="search" />
        </template>
      </q-input>

      <q-space />
      <q-btn
        class="q-mb-sm"
        outline
        icon="person"
        label="Add User"
        @click="showAddDialog = true"
      />
    </template>

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
          class="q-mr-sm q-pa-xs"
        />
      </q-td>
    </template>

    <template #body-cell-mfaActive="props">
      <q-td :props="props">
        <template v-if="props.row.mfaActive">
          <q-icon
            name="check_box"
            color="green"
            size="1.5rem"
          />
        </template>
        <template v-else>
          <q-icon
            name="check_box_outline_blank"
            color="red"
            size="1.5rem"
          />
        </template>
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
