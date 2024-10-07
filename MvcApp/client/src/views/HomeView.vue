<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { URLS } from '../constants'
import { useAuthStore } from '../stores/auth'

const authStore = useAuthStore()
const isAuthenticated = ref<boolean>(false)
const errorMessage = ref<string>('')
const bffLogoutUrl = ref<string>('')
const endUserName = ref<string>('')

onMounted(async () => {
  await authStore.getUsersAuthStatus()
  isAuthenticated.value = authStore.isAuthenticated
  errorMessage.value = authStore.errorMessage
  bffLogoutUrl.value = authStore.getBffLogoutUrl
  endUserName.value = authStore.getEndUserName
})

let RETURN_URL = import.meta.env.VITE_SPA_URL
if (import.meta.env.MODE === 'production') {
  RETURN_URL = import.meta.env.VITE_MVC_APP_URL
}

function login() {
  window.location.assign(`${URLS.DUENDE_BFF_REVERSE_PROXY}/bff/login?returnUrl=${RETURN_URL}`)
}

function logout() {
  // if this is empty and somehow logout button is enabled - redirect user to login page on identity provider
  if (bffLogoutUrl.value.length === 0) {
    window.location.assign(`${URLS.DUENDE_BFF_REVERSE_PROXY}/bff/login?returnUrl=${RETURN_URL}`)
  }
  window.location.assign(
    URLS.DUENDE_BFF_REVERSE_PROXY + bffLogoutUrl.value + '&returnUrl=' + RETURN_URL
  )
}
</script>

<template>
  <div class="page">
    <div class="page__item">
      <h1>Home/Auth page - checks for authentication. Must be simple UI</h1>
      <button @click="login" :disabled="isAuthenticated">Login</button>
    </div>
    <div v-if="isAuthenticated" class="page__item">
      <p class="page__message">
        User is logged in! Hi, {{ endUserName }}! You may choose to logout
      </p>
      <button @click="logout">Logout</button>
    </div>
    <div v-else class="page__item">
      <p class="page__message">
        User is NOT logged in! Could be an error or require user to action logging in
      </p>
      <p>{{ errorMessage }}</p>
    </div>
  </div>
</template>
