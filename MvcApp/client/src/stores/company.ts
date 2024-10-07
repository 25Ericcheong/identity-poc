import { ref } from 'vue'
import { defineStore } from 'pinia'
import { URLS } from '../constants'

export const useCompanyStore = defineStore('company', () => {
  const company = ref<string>('')
  const errorMessage = ref<string>('')

  async function getCompany() {
    const req = new Request(`${URLS.DUENDE_BFF_REVERSE_PROXY}/api/company`, {
      headers: new Headers({
        'X-CSRF': '1',
        'Content-Type': 'application/json'
      }),
      credentials: 'include'
    })

    try {
      const resp = await fetch(req)
      console.log(resp)
      if (resp.status === 401 || resp.status === 401 || !resp.ok) {
        throw Error('Please attempt to re-login')
      }

      const data = (await resp.text()) as string
      company.value = data
    } catch (error: unknown) {
      if (error instanceof Error) {
        errorMessage.value = `An error has occurred: ${error.message}`
      } else {
        errorMessage.value = `Unexpected error: ${error}`
      }
    }
  }

  return {
    company,
    errorMessage,
    getCompany
  }
})
