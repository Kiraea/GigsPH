<script setup lang="ts">



definePageMeta({
  layout:"auth"
})
const displayName = ref("");
const firstName = ref("");
const lastName = ref("");
const description = ref("");
const {onboard} = useUser()
const handleOnboard = async () => {
  const obj: OnboardPayload = {
    displayName: displayName.value,
    firstName: firstName.value,
    lastName:lastName.value,
    description:description.value
  }
  await onboard(obj)
  navigateTo("/home")
}



</script>

<template>
  <div class="">
    <form @submit.prevent="handleOnboard">
      <div>
        <label>Display Name</label>
        <input v-model="displayName"  required type="text">
      </div>
      <div>
        <label>First Name</label>
        <input v-model="firstName" required type="text">
      </div>
      <div>
        <label>Last Name</label>
        <input v-model="lastName" required type="text">
      </div>
      <div>
        <label>Description Name</label>
        <input v-model="description" required type="text">
      </div>    
      <button type="submit">Submit</button>
    </form>
   
  </div>
</template>
