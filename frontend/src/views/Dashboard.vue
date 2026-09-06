<template>
  <div class="dashboard">
    <aside class="sidebar">
      <h2>Inventory</h2>

      <nav>
        <router-link to="/dashboard">Dashboard</router-link>
        <router-link to="/products">Products</router-link>
        <router-link to="/inventory">Inventory</router-link>
      </nav>

      <button @click="logout">Logout</button>
    </aside>

    <main class="main-content">
      <header>
        <h1>Dashboard</h1>
        <p>Welcome to your inventory management system.</p>
      </header>

      <section class="stats">
        <div class="card">
          <h3>Total Products</h3>
          <p>{{ totalProducts }}</p>
        </div>

        <div class="card">
          <h3>Total Stock</h3>
          <p>{{ totalStock }}</p>
        </div>

        <div class="card">
          <h3>Low Stock</h3>
          <p>{{ lowStock }}</p>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'

const router = useRouter()

const totalProducts = ref(0)
const totalStock = ref(0)
const lowStock = ref(0)

const loadDashboard = async () => {
  try {
    const response = await api.get('/Products')

    const products = response.data

    totalProducts.value = products.length

    totalStock.value = products.reduce(
      (total, product) => total + product.stockQuantity,
      0
    )

    lowStock.value = products.filter(
      product => product.stockQuantity <= product.minimumStock
    ).length
  } catch (error) {
    console.error('Failed to load dashboard:', error)
  }
}

const logout = () => {
  localStorage.removeItem('token')
  router.push('/login')
}

onMounted(() => {
  loadDashboard()
})
</script>

<style scoped>
.dashboard {
  min-height: 100vh;
  display: flex;
  background: #f4f6f8;
}

.sidebar {
  width: 220px;
  min-height: 100vh;
  padding: 25px 20px;
  background: #1f2937;
  color: white;
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
}

.sidebar h2 {
  margin-bottom: 30px;
}

.sidebar nav {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.sidebar a {
  padding: 12px;
  color: white;
  text-decoration: none;
  border-radius: 5px;
}

.sidebar a.router-link-active {
  background: #374151;
}

.sidebar button {
  margin-top: auto;
  padding: 11px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.main-content {
  flex: 1;
  padding: 35px;
}

header {
  margin-bottom: 30px;
}

header h1 {
  margin-bottom: 5px;
}

header p {
  color: #6b7280;
}

.stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
}

.card {
  padding: 25px;
  background: white;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.card h3 {
  margin-top: 0;
  color: #6b7280;
}

.card p {
  font-size: 32px;
  font-weight: bold;
  margin-bottom: 0;
}
</style>