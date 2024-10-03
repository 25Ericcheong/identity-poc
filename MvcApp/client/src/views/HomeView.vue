<script setup lang="ts">
import { ref } from 'vue'

const isAuthenticated = ref(false)

function login() {
  window.location.assign('https://localhost:7095/bff/login?returnUrl=http://localhost:5173/')
}
</script>

<template>
  <div class="page">
    <h1>Home/Auth page - checks for authentication. Must be simple UI</h1>
    <button @click="login">Login</button>
  </div>
</template>
