<script lang="ts" setup>
import { ref, shallowRef, watch, onMounted } from 'vue'

// 1. Basic Types to replace 'any'
type LocationItem = { name: string; iso2?: string; [key: string]: any }

definePageMeta({
  layout:"auth"
})
const displayName = ref("");
const firstName = ref("");
const lastName = ref("");
const description = ref("");


// 2. State
// Tip: shallowRef is better than ref for massive lists (like cities/countries). 
// It tells Vue "replace the whole array when it changes, but don't track every single object inside it for changes." 
// This makes rendering large dropdowns noticeably faster.
const countries = shallowRef<LocationItem[]>([])
const states = shallowRef<LocationItem[]>([])
const cities = shallowRef<LocationItem[]>([])

const selectedCountry = ref('')
const selectedState = ref('')
const selectedCity= ref('')

const isLoading = ref(true)
const error = ref<string | null>(null)


const {onboard} = useUser()
const handleOnboard = async () => {
  const obj: OnboardResponse = {
    displayName: displayName.value,
    firstName: firstName.value,
    lastName:lastName.value,
    description:description.value,
    city: selectedCity.value,
    country: selectedCountry.value,
    provinceState: selectedState.value
  }
  await onboard(obj)
  navigateTo("/home")
}



// 3. Store the entire module outside Vue's reactivity.
// We don't want Vue trying to make a massive external library reactive.
let geoAPI: any = null

onMounted(async () => {
  try {
    geoAPI = await import('@countrystatecity/countries-browser')
    // ADDED AWAIT HERE
    countries.value = await geoAPI.getCountries()
  } catch (err: any) {
    console.error('Failed to load countries package:', err)
    error.value = err.message || 'Failed to load country data'
  } finally {
    isLoading.value = false
  }
})

watch(selectedCountry, async (newCountry) => {
  console.log('1. Country changed to:', newCountry) // See if the click registered

  selectedState.value = ''
  cities.value = []

  if (newCountry && geoAPI) {
    try {
      console.log('2. Fetching states for:', newCountry)
      const fetchedStates = await geoAPI.getStatesOfCountry(newCountry)

      console.log('3. States received:', fetchedStates) // See what the API actually handed back

      // Fallback: If the API returns undefined for some reason, default to an empty array
      states.value = fetchedStates || []
    } catch (err) {
      console.error('🔥 Error fetching states:', err)
    }
  } else {
    states.value = []
  }
})

watch(selectedState, async (newState) => {
  console.log('4. State changed to:', newState)
  if (newState && selectedCountry.value && geoAPI) {
    try {
      const fetchedCities = await geoAPI.getCitiesOfState(selectedCountry.value, newState)
      console.log('5. Cities received:', fetchedCities)
      cities.value = fetchedCities || []
    } catch (err) {
      console.error('🔥 Error fetching cities:', err)
    }
  } else {
    cities.value = []
  }
})
</script>

<template>

  <div class="flex  rounded-3xl p-5 shadow-md shadow-gray-300">
    <form @submit.prevent="handleOnboard">
      <div class="flex flex-row">
        <label>Display Name</label>
        <input v-model="displayName"  required type="text">
      </div >
      <div class="flex flex-row">
        <label>First Name</label>
        <input v-model="firstName" required type="text">
      </div>
      <div>
        <label>Last Name</label>
        <input v-model="lastName" required type="text">
      </div>
      <div class="flex flex-row">
        <label>Description Name</label>
        <input v-model="description" required type="text">
      </div>

      <div v-if="isLoading" class="loading">Loading countries...</div>

      <div v-else-if="error" class="error">
        Error: {{ error }}
      </div>

      <div v-else class="flex flex-col gap-5">
        
        <select v-model="selectedCountry">
          gray-200<option value="">Select Country</option>
          <option v-for="c in countries" :key="c.iso2" :value="c.iso2">
            {{ c.name }}
          </option>
        </select>

        <select v-model="selectedState">
          <option value="">Select State</option>
          <option v-for="s in states" :key="s.iso2" :value="s.iso2">
            {{ s.name }}
          </option>
        </select>

        <select v-model="selectedCity">
          <option value="">Select City</option>
          <option v-for="city in cities" :key="city.name" :value="city.name">
            {{ city.name }}
          </option>
        </select>
      </div>
      <button type="submit">Submit</button>
    </form>

  </div>
  
  
</template>
