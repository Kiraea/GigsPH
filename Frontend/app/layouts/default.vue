<script setup lang="ts">
</script>
<template>
  <div>
    <header class="flex flex-row justify-between items-center gap-5 p-5 drop-shadow-2xl shadow-black shadow-md">
      <h1 class="">GigsPH</h1>
      <div class="flex flex-row justify-between items-center gap-x-60 ">
        <div>Option 1</div>
        <div>Option 2</div>
        <div>Option 3</div>
      </div>
      <div>User</div>
    </header>

    <slot/>
  </div>
  
</template>