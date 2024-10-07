<script setup lang="ts">
import { ref } from 'vue'
import { useCompanyStore } from '../stores/company'

const companyStore = useCompanyStore()
const errorMessage = ref<string>('')
const company = ref<string>('')

async function getData() {
  await companyStore.getCompany()
  errorMessage.value = companyStore.errorMessage
  company.value = companyStore.company
}
</script>

<template>
  <div class="page">
    <div class="page__item">
      <h1>Time to get some companies</h1>
      <button @click="getData">Get Company Data</button>
    </div>
    <div v-if="company.length === 0" class="page__item">
      <p class="page__message">
        Either no company data imported yet or an unexpected error has occurred
      </p>
      <p v-if="errorMessage.length > 0">Error found: {{ errorMessage }}</p>
    </div>
    <div v-else class="page__item">
      <p>Data received from .NET Core Company API!</p>
      <p>{{ company }}</p>
    </div>
  </div>
</template>
