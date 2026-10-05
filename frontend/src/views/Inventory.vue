<template>
  <div class="inventory-page">
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
          <h1>Inventory</h1>
          <p>Manage stock in and stock out transactions.</p>
        </div>

      </header>



      <div class="inventory-card">
        <h2>Stock Transaction</h2>

        <form @submit.prevent="submitTransaction">

          <div class="form-group">
            <label>Product</label>

            <select v-model="selectedProductId" required>
              <option value="" disabled>
                Select a product
              </option>

              <option
                v-for="product in products"
                :key="product.id"
                :value="product.id"
              >
                {{ product.name }} — {{ product.sku }}
              </option>
            </select>
          </div>

          <div v-if="selectedProduct" class="current-stock">
            Current Stock:
            <strong>{{ selectedProduct.stockQuantity }}</strong>
          </div>

          <div class="form-group">
            <label>Transaction Type</label>

            <div class="transaction-types">
              <label>
                <input
                  v-model="transactionType"
                  type="radio"
                  value="stock-in"
                />
                Stock In
              </label>

              <label>
                <input
                  v-model="transactionType"
                  type="radio"
                  value="stock-out"
                />
                Stock Out
              </label>
            </div>
          </div>

          <div class="form-group">
            <label>Quantity</label>

            <input
              v-model.number="quantity"
              type="number"
              min="1"
              required
            />
          </div>

          <div class="form-group">
            <label>Reason</label>

            <textarea
              v-model="reason"
              rows="3"
              placeholder="Optional reason"
            ></textarea>
          </div>

          <p v-if="success" class="success">
            {{ success }}
          </p>

          <p v-if="error" class="error">
            {{ error }}
          </p>

          <button
            class="submit-btn"
            type="submit"
            :disabled="saving"
          >
            {{ saving ? 'Processing...' : 'Submit Transaction' }}
          </button>
        </form>
      </div>

      <div class="products-card">
        <h2>Current Stock</h2>

        <div v-if="loading" class="message">
          Loading products...
        </div>

        <div v-else-if="products.length === 0" class="message">
          No products found.
        </div>

        <div v-else class="table-container">
          <table>
            <thead>
              <tr>
                <th>Product</th>
                <th>SKU</th>
                <th>Category</th>
                <th>Current Stock</th>
                <th>Minimum Stock</th>
                <th>Status</th>
              </tr>
            </thead>

            <tbody>
              <tr
                v-for="product in products"
                :key="product.id"
              >
                <td>{{ product.name }}</td>
                <td>{{ product.sku }}</td>
                <td>{{ product.category }}</td>
                <td>{{ product.stockQuantity }}</td>
                <td>{{ product.minimumStock }}</td>

                <td>
                  <span
                    v-if="product.stockQuantity <= product.minimumStock"
                    class="low-stock"
                  >
                    Low Stock
                  </span>

                  <span
                    v-else
                    class="in-stock"
                  >
                    In Stock
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { routeLocationKey, useRouter } from 'vue-router'
import api from '../services/api'
import { formatDate } from '../utils/formatDate'

const router = useRouter()

const products = ref([])
const selectedProductId = ref('')
const selectedProductIdForTransaction = ref('')
const transactionType = ref('stock-in')
const quantity = ref(1)
const reason = ref('')

const loading = ref(true)
const saving = ref(false)
const show = ref(false)
const transactions = ref([])
const loadingTransactions = ref(false)
const error = ref('')
const success = ref('')


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


const selectedProduct = computed(() => {
  return products.value.find(
    product => product.id === selectedProductId.value
  )
})


const loadProducts = async () => {
  loading.value = true
  error.value = ''

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

const getTransactions = async () => {
  error.value = ''
  transactions.value = []

  if (!selectedProductIdForTransaction.value) {
    error.value = 'Please select a Product'
    return
  }

  loadingTransactions.value = true

  try {
    const response = await api.get(
      `/Inventory/${encodeURIComponent(selectedProductIdForTransaction.value)}`
    )
    transactions.value = response.data
    console.log('hi')
  } catch (err) {
      console.error('Failed to load transactions:', err)

      if (err.response?.status === 401) {
        localStorage.removeItem('token')
        router.push('/login')
        return
      }

      if (err.response?.status === 403) {
        error.value =
          'You do not have permission to view transactions.'
        return
      }

      error.value =
        err.response?.data?.message ||
        'Failed to load transactions.'

    } finally {
      loadingTransactions.value = false
    }
}

const submitTransaction = async () => {
  error.value = ''
  success.value = ''

  if (!selectedProductId.value) {
    error.value = 'Please select a product.'
    return
  }

  if (!quantity.value || quantity.value <= 0) {
    error.value = 'Quantity must be greater than 0.'
    return
  }

  if (
    transactionType.value === 'stock-out' &&
    selectedProduct.value &&
    quantity.value > selectedProduct.value.stockQuantity
  ) {
    error.value = 'Insufficient stock.'
    return
  }

  saving.value = true

  try {
    const url =
      `/Inventory/${selectedProductId.value}/${transactionType.value}`

    await api.post(url, null, {
      params: {
        quantity: quantity.value,
        reason: reason.value || undefined
      }
    })

    success.value =
      transactionType.value === 'stock-in'
        ? 'Stock added successfully.'
        : 'Stock removed successfully.'

    quantity.value = 1
    reason.value = ''

    await loadProducts()
  } catch (err) {
    console.error('Transaction failed:', err)

    if (err.response?.status === 401) {
      localStorage.removeItem('token')
      router.push('/login')
      return
    }

    if (err.response?.status === 403) {
      error.value =
        'You do not have permission to perform this transaction.'
      return
    }

    error.value =
      err.response?.data?.message ||
      'Failed to process inventory transaction.'
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
.inventory-page {
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
  margin-bottom: 30px;
}

.page-header h1 {
  margin-bottom: 5px;
}

.page-header p {
  color: #6b7280;
}

.inventory-card,
.products-card {
  background: white;
  padding: 25px;
  margin-bottom: 25px;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.inventory-card h2,
.products-card h2 {
  margin-top: 0;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 7px;
  font-weight: 600;
}

select,
input[type="number"],
textarea {
  width: 100%;
  padding: 10px;
  box-sizing: border-box;
  border: 1px solid #ccc;
  border-radius: 5px;
  font-family: inherit;
}

.transaction-types {
  display: flex;
  gap: 25px;
}

.transaction-types label {
  font-weight: normal;
}

.transaction-types input {
  margin-right: 6px;
}

.current-stock {
  padding: 12px;
  margin-bottom: 20px;
  background: #f3f4f6;
  border-radius: 6px;
}

.submit-btn {
  padding: 11px 20px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.submit-btn:disabled {
  cursor: not-allowed;
}

.success {
  padding: 12px;
  color: #166534;
  background: #dcfce7;
  border-radius: 6px;
}

.error {
  padding: 12px;
  color: #b91c1c;
  background: #fee2e2;
  border-radius: 6px;
}

.table-container {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  padding: 14px;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
}

th {
  background: #f9fafb;
}

.low-stock {
  padding: 5px 9px;
  color: #b91c1c;
  background: #fee2e2;
  border-radius: 5px;
  font-size: 13px;
}

.in-stock {
  padding: 5px 9px;
  color: #166534;
  background: #dcfce7;
  border-radius: 5px;
  font-size: 13px;
}

.message {
  padding: 25px;
  text-align: center;
}
</style>