// https://nuxt.com/docs/api/configuration/nuxt-config

import tailwindcss from "@tailwindcss/vite";
//console.log("API_URL:", process.env.API_URL)
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  modules: [ '@nuxt/fonts', '@nuxt/icon'],
  routeRules: {
    "/api/**" : {
      proxy: process.env.API_URL + '/api/**'
    }
  },
  vite: {
    plugins: [
      tailwindcss(),
    ],
  },
  css:['./app/assets/reset.css', './app/assets/css/main.css'],
  icon: {
    mode: 'css',
    cssLayer: 'base',
    localApiEndpoint: '/_nuxt_icon', // moves it away from /api/
  }



})