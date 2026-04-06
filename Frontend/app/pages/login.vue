<script setup lang="ts">

  import {useAuth} from "~/composables/useAuth";
  import {useErrorHandler} from "~/composables/useErrorHandler";

  definePageMeta({
    layout: 'auth' 
  })
  const email = ref("");
  const password = ref("");
  const isLoading = ref("");
  const error = ref<null | string>();
  const {user, setUser} = useAuth();
  const {setErrors}= useErrorHandler()

  interface LoginResponse {
    userId: string;
    email: string;
    accessToken: string;
  }
  const handleLogin = async () => {
    try{
      const response: LoginResponse = await $fetch("/api/auth/login", {
        method:'POST',
        body:{"usernameOrEmail": email.value, "password": password.value}
      })
      setUser(response.userId)
      navigateTo("/home");
    }catch(e:any){
      console.log(e.data?.detail);
      console.log(e);
      setErrors(e);
    }
  }
</script>
<template>
  <div class="bg-white rounded-lg shadow-sm">
    <form @submit.prevent="handleLogin" class="flex flex-col p-5 gap-5  ">
      <div>Login</div>
    
      <div class="flex flex-col">
        <div>Email</div>
        <input v-model="email"  type="email" ></input>  
      </div>
      
      <div class="flex flex-col">
        <div>Password</div>
        <input v-model="password"  type="password"></input>  
      </div>
      
      <button type="submit">Confirm</button>
      
      <div class="">Dont have an Account yet?<nuxtLink to="/register" class="font-bold text-lg"><button>Register</button></nuxtLink></div>
    </form>
    
  </div>
</template>