<script setup lang="ts">

defineProps<{
  pubUser: GetPublicProfileResponse
}>()
</script>

<template>
  <!-- Name -->
  <span v-if="pubUser.firstName || pubUser.lastName">
      {{ [pubUser.firstName, pubUser.lastName].filter(Boolean).join(' ') }}
      </span>
  <span v-else>No name provided</span>

  <!-- Location -->
  <div v-if="pubUser.location">
    <div>{{pubUser.location.city}}</div>
    <div>{{pubUser.location.country}}</div>
    <div>{{pubUser.location.provinceState}}</div>
  </div>

  <!-- Genres -->
  <template v-if="pubUser.genres?.length">
    <div v-for="genre in pubUser.genres" :key="genre.id">
      <div>{{genre.id}}</div>
      <div>{{genre.name}}</div>
    </div>
  </template>
  <div v-else>No genres</div>

  <!-- Instruments -->
  <template v-if="pubUser.instruments?.length">
    <div v-for="instrument in pubUser.instruments" :key="instrument.id">
      <div>{{instrument.name}}</div>
      <div>{{instrument.id}}</div>
    </div>
  </template >
  <div v-else>No instruments</div>

  <!-- Social Links -->
  <template v-if="pubUser.socialLinks?.length">
    <a v-for="link in pubUser.socialLinks" :key="link.id" :href="link.url">
      {{ link.url}}
    </a>
  </template>
  <div v-else>No social links</div>


</template>

<style scoped>

</style>