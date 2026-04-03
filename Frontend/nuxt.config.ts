// https://nuxt.com/docs/api/configuration/nuxt-config


console.log("API_URL:", process.env.API_URL)
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  modules: ['@nuxtjs/tailwindcss', '@nuxt/fonts'],
  routeRules: {
    "/api/**" : {
      proxy: process.env.API_URL + '/api/**'
    }
  },



})