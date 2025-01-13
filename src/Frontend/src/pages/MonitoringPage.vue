<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { QTableProps } from 'quasar'

import { apiHelper } from '../helpers/apiHelper'

import { CacheItem } from 'src/models/CacheItem'

const loading = ref<boolean>()
const cacheItems = ref<CacheItem[]>([])

const columns : QTableProps['columns'] = [
  {
    name: 'key',
    label: 'Key',
    required: true,
    align: 'left',
    field: row => row.key,
    format: val => `${val}`,
    style: 'width: 300px'
  },
  {
    name: 'value',
    label: 'Value',
    required: true,
    align: 'left',
    field: row => row,
    format: row => `${row.value}`
  }
]

onMounted(async () => {
  try {
    loading.value = true
    cacheItems.value = await apiHelper.getCache()
  } finally {
    loading.value = false
  }
})

</script>

<template>
  <q-page padding>
    <q-table
      flat
      bordered
      :rows="cacheItems"
      :rows-per-page-options="[20, 50, 0]"
      :columns="columns"
      row-key="key"
      :loading="loading"
    />
  </q-page>
</template>
