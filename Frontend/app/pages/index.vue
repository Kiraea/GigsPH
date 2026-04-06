<script setup lang="ts">
import cities from '~/../public/city.json'
import regions from '~/../public/region.json'
import provinces from '~/../public/province.json'

const selectedCity = ref<any>(null)
const selectedRegion = ref<any>(null)
const selectedProvince = ref<any>(null)

const filteredProvinces = computed(() => {
  const region = selectedRegion.value
  if (!region) return []
  return provinces.filter(p => p.region_code === region.region_code)
})

const filteredCities = computed(() => {
  const province = selectedProvince.value
  if (!province) return []
  return cities.filter(c => c.province_code === province.province_code)
})

watch(selectedRegion, () => {
  selectedProvince.value = null
  selectedCity.value = null
})

watch(selectedProvince, () => {
  selectedCity.value = null
})
</script>

<template>
  <select v-model="selectedRegion">
    <option :value="null" disabled>Select Region</option>
    <option v-for="region in regions" :key="region.region_code" :value="region">
      {{ region.region_name }}
    </option>
  </select>

  <select v-model="selectedProvince" :disabled="!selectedRegion">
    <option :value="null" disabled>Select Province</option>
    <option v-for="province in filteredProvinces" :key="province.province_code" :value="province">
      {{ province.province_name }}
    </option>
  </select>

  <select v-model="selectedCity" :disabled="!selectedProvince">
    <option :value="null" disabled>Select City</option>
    <option v-for="city in filteredCities" :key="city.city_code" :value="city">
      {{ city.city_name }}
    </option>
  </select>
</template>