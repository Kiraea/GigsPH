<script setup lang="ts">

  import {useAuth} from "~/composables/useAuth";

  definePageMeta({
    layout: 'auth' 
  })
  const email = ref("");
  const password = ref("");
  const isLoading = ref("");
  const error = ref<null | string>();
  const {user, setUser} = useAuth();

  interface LoginResponse {
    userId: string;
    email?: string;
    accessToken?: string;
  }
  const handleLogin = async () => {
    try{
      const response: LoginResponse = await $fetch("/api/auth/login", {
        method:'POST',
        body:{"email": email, "password": password}
      })
      setUser(response.userId)
      navigateTo("/home");
    }catch(e:any){
      console.log(e.data?.detail);
    }
  }
</script>
<template>
  <div class="bg-white">
    <div>Login</div>
    
    <div>Email</div>
    <input v-model="email"  type="email"></input>

    <div>Password</div>
    <input v-model="password"  type="password"></input>
  </div>
</template>