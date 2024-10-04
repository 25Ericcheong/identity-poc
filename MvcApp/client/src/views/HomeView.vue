<script setup lang="ts">
import { ref } from 'vue'

const DUENDE_BFF_REVERSE_PROXY_URL = 'https://localhost:7095'

const isAuthenticated = ref<boolean>(false)
const responseMessage = ref<string>('')
const bffLogoutUrl = ref<string>('')

let RETURN_URL = import.meta.env.VITE_SPA_URL
if (import.meta.env.MODE === 'production') {
  RETURN_URL = import.meta.env.VITE_MVC_APP_URL
}

function login() {
  window.location.assign(`${DUENDE_BFF_REVERSE_PROXY_URL}/bff/login?returnUrl=${RETURN_URL}`)
}

function logout() {
  if (bffLogoutUrl.value.length === 0) {
    return
  }
  window.location.assign(
    DUENDE_BFF_REVERSE_PROXY_URL + bffLogoutUrl.value + '&returnUrl=' + RETURN_URL
  )
}

var req = new Request(`${DUENDE_BFF_REVERSE_PROXY_URL}/bff/user`, {
  headers: new Headers({
    'X-CSRF': '1',
    'Content-Type': 'application/json'
  }),
  credentials: 'include'
})

window.addEventListener('load', async () => {
  try {
    const resp = await fetch(req)
    console.log(resp)
    if (resp.status === 401 || !resp.ok) {
      let respText = await resp.text()
      console.log(respText)
      if (respText === undefined || respText.length === 0) {
        respText = 'Unauthorized access'
      }
      throw new Error(`Please re-login. Error code: ${resp.status}  Error: ${respText}`)
    }

    const claims = (await resp.json()) as Record<string, string>[]
    const logoutUrlClaim = claims.find((claim) => claim.type === 'bff:logout_url')
    if (logoutUrlClaim?.value !== undefined) {
      bffLogoutUrl.value = logoutUrlClaim.value
    }

    const endUserNameClaim = claims.find((claim) => claim.type === 'name')
    if (endUserNameClaim?.value !== undefined) {
      responseMessage.value = endUserNameClaim.value
    }

    console.log('user logged in \n ============', claims)
    isAuthenticated.value = true
  } catch (error: unknown) {
    isAuthenticated.value = false

    if (error instanceof Error) {
      responseMessage.value = `An error has occurred: ${error.message}`
    } else {
      responseMessage.value = `Unexpected error: ${error}`
    }
  }
})
</script>

<template>
  <div class="page">
    <div class="page__item">
      <h1>Home/Auth page - checks for authentication. Must be simple UI</h1>
      <button @click="login" :disabled="isAuthenticated">Login</button>
    </div>
    <div v-if="isAuthenticated" class="page__item">
      <p>User is logged in! Hi, {{ responseMessage }}! You may choose to logout</p>
      <button @click="logout">Logout</button>
    </div>
    <div v-else class="page__item">
      <p>User is NOT logged in! Could be an error or require user to action logging in</p>
      <p>{{ responseMessage }}</p>
    </div>
  </div>
</template>
