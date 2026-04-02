<script setup lang="ts">
</script>
<template>
  <Section class="flex flex-row bg-[url('/mainBg.jpg')] bg-black/60 bg-blend-multiply bg-no-repeat bg-center bg-[length:100%_100%]">
    <div class="flex-1 flex flex-col gap-10 p-10 text-white"   >
      <h1 class="text-9xl font-bold font-sans ">GigsPH</h1>
      <h2 class="text-6xl font-semibold">Is a platform where you can connect with different musicians around the Philippines.</h2>
      <div>
        <NuxtLink to="/register">
          <button class="rounded-2xl bg-white text-black  p-4 text-4xl drop-shadow-sm shadow-400 shadow-md">Join Now</button>
        </NuxtLink>  
      </div>
      
    </div>
    <div class="flex-1 ">
      <div></div>
      <div></div>
    </div>
  </Section>
  <section class="flex flex-row"> 
    <div class="flex-1 flex flex-col gap-10 p-10">
      <h1 class="text-6xl font-bold ">connect with others</h1>
      <h2 class="text-4xl font-semibold">meet new people through</h2>
      <div class="flex flex-row justify-around">
        <div class="flex flex-col w-1/4 items-center">
          <div class="w-full aspect-square bg-[url('/mainbg.jpg')] bg-[length:100%_100%]" ></div>
          <div class="text-4xl  font-bold">Chats & Music</div>  
        </div>
        <div class="flex flex-col w-1/4 items-center">
          <div class="w-full bg-[url('/mainbg.jpg')] aspect-square bg-[length:100%_100%]" ></div>
          <div class="text-4xl font-bold">Bands</div>
        </div>

      </div>
    </div>
  </section>
</template>