import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { URLS } from '../constants'

export const useAuthStore = defineStore('auth', () => {
  const claims = ref<Record<string, string>[]>([])
  const isAuthenticated = ref(false)
  const errorMessage = ref<string>('')

  async function getUsersAuthStatus() {
    const req = new Request(`${URLS.DUENDE_BFF_REVERSE_PROXY}/bff/user`, {
      headers: new Headers({
        'X-CSRF': '1',
        'Content-Type': 'application/json'
      }),
      credentials: 'include'
    })

    try {
      const resp = await fetch(req)
      if (resp.status === 401 || resp.status === 401 || !resp.ok) {
        throw Error('Please attempt to re-login')
      }

      const respClaims = (await resp.json()) as Record<string, string>[]
      claims.value = respClaims

      console.log('user logged in \n ============', respClaims)
      isAuthenticated.value = true
    } catch (error: unknown) {
      isAuthenticated.value = false

      if (error instanceof Error) {
        errorMessage.value = `An error has occurred: ${error.message}`
      } else {
        errorMessage.value = `Unexpected error: ${error}`
      }
    }
  }

  const getBffLogoutUrl = computed(() => {
    const logoutUrl = claims.value.find((claim) => claim.type === 'bff:logout_url')
    if (logoutUrl?.value !== undefined) {
      return logoutUrl.value
    }
    return ''
  })

  const getEndUserName = computed(() => {
    const endUserName = claims.value.find((claim) => claim.type === 'name')
    if (endUserName?.value !== undefined) {
      return endUserName.value
    }
    return ''
  })

  return {
    claims,
    isAuthenticated,
    errorMessage,
    getBffLogoutUrl,
    getEndUserName,
    getUsersAuthStatus
  }
})
