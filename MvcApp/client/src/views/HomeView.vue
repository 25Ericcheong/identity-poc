<script setup lang="ts">
import { ref } from 'vue'

const DUENDE_BFF_REVERSE_PROXY_URL = 'https://localhost:7095/bff'

const isAuthenticated = ref<boolean>(false)
const responseMessage = ref<string>('')

let RETURN_URL = import.meta.env.VITE_SPA_URL
if (import.meta.env.MODE === 'production') {
  RETURN_URL = import.meta.env.VITE_MVC_APP_URL
}

function login() {
  window.location.assign(`${DUENDE_BFF_REVERSE_PROXY_URL}/login?returnUrl=${RETURN_URL}`)
}

var req = new Request(`${DUENDE_BFF_REVERSE_PROXY_URL}/user`, {
  headers: new Headers({
    'X-CSRF': '1',
    'Content-Type': 'application/json'
  }),
  credentials: 'include'
})

var resp = await fetch(req)
if (resp.ok) {
  const userClaims = await resp.json()
  console.log('user logged in', userClaims)
  isAuthenticated.value = true
} else if (resp.status === 401) {
  console.log('user not logged in')
  isAuthenticated.value = false
} else {
  console.error('error', resp.text())
  isAuthenticated.value = false
}
</script>

<template>
  <div class="page">
    <h1>Home/Auth page - checks for authentication. Must be simple UI</h1>
    <button @click="login">Login</button>
  </div>
</template>
