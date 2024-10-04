<script setup lang="ts">
import { ref } from 'vue'

const isAuthenticated = ref(false)

let RETURN_URL = import.meta.env.VITE_SPA_URL
if (import.meta.env.MODE === 'production') {
  RETURN_URL = import.meta.env.VITE_MVC_APP_URL
}

function login() {
  window.location.assign(`https://localhost:7095/bff/login?returnUrl=${RETURN_URL}`)
}
</script>

<template>
  <div class="page">
    <h1>Home/Auth page - checks for authentication. Must be simple UI</h1>
    <button @click="login">Login</button>
  </div>
</template>
