/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./app/**/*.{js,vue,ts}",
  ],
  theme: {
    extend: {
      fontFamily: {
        // This makes Poppins the default font for your whole app!
        sans: ['Inter', 'sans-serif'],
      }
    },
  },
  plugins: [],
}