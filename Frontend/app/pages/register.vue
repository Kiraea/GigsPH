<script setup lang="ts">

  definePageMeta({
    layout: 'auth' 
  })
</script>
<template>
  <div class="flex bg-white flex-row  rounded-2xl w-[80%] h-[60vh]">
    <div class="flex flex-col gap-y-10 py-10 px-20 flex-1">
      
      <h1 class="text-9xl font-bold self-center">Login</h1>

      <form class="flex flex-col gap-5">
        <div class="flex flex-col">
          <label class="text-4xl ">Username</label>
          <input type="text" class="border-black border-2">
        </div>
        <div class="flex flex-col">
          <label class="text-4xl">Password</label>
          <input type="password" class="border-black border-2">
        </div>

      </form>     
    </div>
    <div class="flex items-center justify-center flex-1 bg-[url('/mainBg.jpg')] bg-no-repeat bg-center bg-[length:100%_100%] bg-black/60 bg-blend-multiply">
      <div class="text-9xl font-bold text-white">GigsPH</div>
    </div>
   
  </div>
</template>
