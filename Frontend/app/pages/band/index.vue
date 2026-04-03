<script setup lang="ts">
interface Todo {
  id: number,
  title: string,
  completed:boolean 
}
  const {data:todos, status, error } = useFetch<Todo[]>("https://jsonplaceholder.typicode.com/todos",
      {
        lazy:true,
      })
</script>

<template>
  <div>
    <div v-if="status === 'pending'" class="text-6xl font-bold">Loading....</div>
    <div v-else-if="error">Error Rip</div>
    <div v-else>
      <div v-for="todo in todos" :key="todo.id">
        <div>{{todo.id}}</div>
        <div>{{todo.title}}</div>
        <div>{{todo.completed}}</div>
      </div>
    </div>
    
  </div>
  
</template>
