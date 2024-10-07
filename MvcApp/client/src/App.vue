<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <header>
    <div class="wrapper">
      <nav>
        <RouterLink to="/">Home</RouterLink>
        <RouterLink to="/company">Company</RouterLink>
      </nav>
    </div>
  </header>

  <RouterView />
</template>

<style scoped>
.wrapper {
  margin-top: 20px;
  display: flex;
  justify-content: center;

  a {
    margin: 5px;
    font-size: large;
  }
}
</style>
