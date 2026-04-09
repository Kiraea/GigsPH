<script setup lang="ts">
import ProfileDisplay from "~/components/user/ProfileDisplay.vue";





const route = useRoute()
console.log(route.params.id);



const {pubUser, pubUserPending, pubUserError} = useUser(route.params.id as string)
const {user} = useAuth()
const isOwner = computed(()=> route.params.id === user.value?.id)

</script>

<template>
  <div>
    <div>

      <div v-if="pubUserPending">Loading...</div>
      <div v-else-if="pubUserError">Error</div>
      <template v-else-if="pubUser">
        <ProfileDisplay :pub-user="pubUser"/>
      </template>
    </div>

  </div>


</template>

<style scoped>

</style>