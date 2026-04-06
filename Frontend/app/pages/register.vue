<script setup lang="ts">

  definePageMeta({
    layout: 'auth' 
  });
  
  const email = ref("");
  const password = ref("");
  const isLoading = ref(false);
  const error = ref<null|string>(null)
  const {setErrors} = useErrorHandler();
  const handleRegister= async () => {
    isLoading.value = true;
    
    try{
      const response = await $fetch("/api/auth/register", {
        method: 'POST',
        body: {email: email.value, password: password.value},
      })
      await navigateTo("/login")
    }catch(e :any){
      console.log(e.data?.detail);
      error.value = e.data?.detail;
      setErrors(e);
      
    }finally{
      isLoading.value=false
    }
  }
  
</script>
<template>
  <div class="flex bg-white flex-row  rounded-2xl w-[80%] h-[60vh]">
    <div class="flex flex-col gap-y-10 py-10 px-20 flex-1">
      
      <h1 class="text-9xl font-bold self-center">Register</h1>

      <form class="flex flex-col gap-5" @submit.prevent="handleRegister">
        <div class="flex flex-col">
          <label class="text-4xl ">Email
          </label>
          <input v-model="email" required  type="email" class="border-black border-2">
         
        </div>
        <div class="flex flex-col">
          <label class="text-4xl">Password</label>
          <input  v-model="password" required  type="password" class="border-black border-2">
        </div>
        <button type="submit" :disabled="isLoading">Confirm</button>
      </form>
      <div v-if="error">{{error}}</div>
    </div>
    <div class="flex items-center justify-center flex-1 bg-[url('/mainBg.jpg')] bg-no-repeat bg-center bg-[length:100%_100%] bg-black/60 bg-blend-multiply">
      <div class="text-9xl font-bold text-white">GigsPH</div>
    </div>
   
  </div>
</template>
