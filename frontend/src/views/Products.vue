<template>
  <div class="products-page">
    <aside class="sidebar">
      <h2>Inventory</h2>

      <nav>
        <router-link to="/dashboard">Dashboard</router-link>
        <router-link to="/products">Products</router-link>
        <router-link to="/inventory">Inventory</router-link>
      </nav>

      <button class="logout-btn" @click="logout">Logout</button>
    </aside>

    <main class="main-content">
      <header class="page-header">
        <div>
          <h1>Products</h1>
          <p>Manage your inventory products.</p>
        </div>

        <button
          v-if="canManageProducts"
          class="add-btn"
          @click="showForm = !showForm"
        >
          {{ showForm ? 'Cancel' : '+ Add Product' }}
        </button>
      </header>

      <!-- Add Product Form -->
      <div v-if="showForm" class="form-card">
        <h2>Add Product</h2>

        <form @submit.prevent="createProduct">
          <div class="form-grid">
            <div class="form-group">
              <label>Name</label>
              <input v-model="form.name" type="text" required />
            </div>

            <div class="form-group">
              <label>SKU</label>
              <input v-model="form.sku" type="text" required />
            </div>

            <div class="form-group">
              <label>Category</label>
              <input v-model="form.category" type="text" required />
            </div>

            <div class="form-group">
              <label>Price</label>
              <input
                v-model.number="form.price"
                type="number"
                min="0"
                step="0.01"
                required
              />
            </div>

            <div class="form-group">
              <label>Initial Stock</label>
              <input
                v-model.number="form.stockQuantity"
                type="number"
                min="0"
                required
              />
            </div>

            <div class="form-group">
              <label>Minimum Stock</label>
              <input
                v-model.number="form.minimumStock"
                type="number"
                min="0"
                required
              />
            </div>
          </div>

          <div class="form-group">
            <label>Description</label>
            <textarea
              v-model="form.description"
              rows="3"
            ></textarea>
          </div>

          <p v-if="formError" class="error">
            {{ formError }}
          </p>

          <button class="submit-btn" type="submit" :disabled="saving">
            {{ saving ? 'Creating...' : 'Create Product' }}
          </button>
        </form>
      </div>

      <!-- Products -->
      <div v-if="loading" class="message">
        Loading products...
      </div>

      <div v-else-if="error" class="error">
        {{ error }}
      </div>

      <div v-else-if="products.length === 0" class="message">
        No products found.
      </div>

      <div v-else class="table-container">
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>SKU</th>
              <th>Category</th>
              <th>Price</th>
              <th>Stock</th>
              <th>Minimum Stock</th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="product in products" :key="product.id">
              <td>{{ product.name }}</td>
              <td>{{ product.sku }}</td>
              <td>{{ product.category }}</td>
              <td>₹{{ product.price }}</td>
              <td>{{ product.stockQuantity }}</td>
              <td>{{ product.minimumStock }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'

const router = useRouter()

const products = ref([])
const loading = ref(true)
const error = ref('')

const showForm = ref(false)
const saving = ref(false)
const formError = ref('')

const form = ref({
  name: '',
  sku: '',
  description: '',
  category: '',
  price: 0,
  stockQuantity: 0,
  minimumStock: 0
})

const getRoleFromToken = () => {
  const token = localStorage.getItem('token')

  if (!token) {
    return null
  }

  try {
    const payload = JSON.parse(atob(token.split('.')[1]))

    return payload[
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
    ]
  } catch {
    return null
  }
}

const role = computed(() => getRoleFromToken())

const canManageProducts = computed(() => {
  return role.value === 'Admin' || role.value === 'Manager'
})

const loadProducts = async () => {
  try {
    const response = await api.get('/Products')
    products.value = response.data
  } catch (err) {
    console.error('Failed to load products:', err)

    if (err.response?.status === 401) {
      localStorage.removeItem('token')
      router.push('/login')
      return
    }

    error.value = 'Failed to load products.'
  } finally {
    loading.value = false
  }
}

const createProduct = async () => {
  formError.value = ''
  saving.value = true

  try {
    await api.post('/Products', form.value)

    showForm.value = false

    form.value = {
      name: '',
      sku: '',
      description: '',
      category: '',
      price: 0,
      stockQuantity: 0,
      minimumStock: 0
    }

    await loadProducts()
  } catch (err) {
    console.error('Failed to create product:', err)

    if (err.response?.status === 401) {
      localStorage.removeItem('token')
      router.push('/login')
      return
    }

    if (err.response?.status === 403) {
      formError.value = 'You do not have permission to create products.'
      return
    }

    formError.value =
      err.response?.data?.message || 'Failed to create product.'
  } finally {
    saving.value = false
  }
}

const logout = () => {
  localStorage.removeItem('token')
  router.push('/login')
}

onMounted(() => {
  loadProducts()
})
</script>

<style scoped>
.products-page {
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

.logout-btn {
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

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.page-header h1 {
  margin-bottom: 5px;
}

.page-header p {
  color: #6b7280;
}

.add-btn,
.submit-btn {
  padding: 11px 18px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.form-card {
  background: white;
  padding: 25px;
  margin-bottom: 25px;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.form-card h2 {
  margin-top: 0;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 18px;
}

.form-group {
  margin-bottom: 18px;
}

.form-group label {
  display: block;
  margin-bottom: 6px;
  font-weight: 600;
}

input,
textarea {
  width: 100%;
  padding: 10px;
  box-sizing: border-box;
  border: 1px solid #ccc;
  border-radius: 5px;
  font-family: inherit;
}

textarea {
  resize: vertical;
}

.submit-btn {
  margin-top: 5px;
}

.table-container {
  background: white;
  border-radius: 10px;
  overflow-x: auto;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  padding: 15px;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
}

th {
  background: #f9fafb;
}

.message {
  padding: 30px;
  text-align: center;
  background: white;
  border-radius: 10px;
}

.error {
  padding: 12px;
  margin-bottom: 15px;
  color: #b91c1c;
  background: #fee2e2;
  border-radius: 6px;
}
</style>