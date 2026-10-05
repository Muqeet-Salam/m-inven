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
        <div class="header-action">
          <button
            v-if="canManageProducts"
            class="add-btn"
            @click="showForm = !showForm"
          >
            {{ showForm ? 'Cancel' : '+ Add Product' }}
          </button>

        </div>
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
              <th>Price</th>
              <th>Stock</th>
              <th>Last Updated Time</th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="product in products" :key="product.id" class="product-row" @click="openProduct(product)">
              <td>{{ product.name }}</td>
              <td>₹{{ product.price }}</td>
              <td>{{ product.stockQuantity }}</td>
              <td>{{ formatDate(product.updatedAt) }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Product Details Modal -->
      <div v-if="showProductModal" class="modal-overlay" @click.self="closeProductModal">
        <div class="modal">
          <div class="modal-header">
            <div>
              <h2>{{ selectedProduct.name }}</h2>
              <p>{{ selectedProduct.sku }}</p>
            </div>

            <button class="close-btn" @click="closeProductModal">
              ×
            </button>
          </div>

          <div class="product-details">
            <div class="detail-item">
              <span>Category</span>
              <strong>{{ selectedProduct.category }}</strong>
            </div>

            <div class="detail-item">
              <span>Price</span>
              <strong>₹{{ selectedProduct.price }}</strong>
            </div>

            <div class="detail-item">
              <span>Current Stock</span>
              <strong>{{ selectedProduct.stockQuantity }}</strong>
            </div>

            <div class="detail-item">
              <span>Minimum Stock</span>
              <strong>{{ selectedProduct.minimumStock }}</strong>
            </div>

            <div class="detail-item">
              <span>Created</span>
              <strong>{{ formatDate(selectedProduct.createdAt) }}</strong>
            </div>

            <div class="detail-item">
              <span>Last Updated</span>
              <strong>{{ formatDate(selectedProduct.updatedAt) }}</strong>
            </div>

            <div class="detail-item description">
              <span>Description</span>
              <p>
                {{ selectedProduct.description || 'No description provided.' }}
              </p>
            </div>
          </div>

          <div class="modal-actions">
            <button v-if="canManageProducts" class="action-btn" @click="openUpdateModal(selectedProduct)">
              Update Product
            </button>

            <button v-if="role === 'Admin'" class="action-btn" @click="openTransactionsModal(selectedProduct)">
              View Transactions
            </button>
          </div>
        </div>
      </div>

      <!-- Update Product Modal -->
      <div
        v-if="showUpdateModal"
        class="modal-overlay"
        @click.self="closeUpdateModal"
      >
        <div class="modal">
          <div class="modal-header">
            <h2>Update Product</h2>

            <button class="close-btn" @click="closeUpdateModal">
              ×
            </button>
          </div>

          <form @submit.prevent="updateProduct">
            <div class="form-grid">
              <div class="form-group">
                <label>Name</label>
                <input
                  v-model="updateForm.name"
                  type="text"
                  required
                />
              </div>

              <div class="form-group">
                <label>SKU</label>
                <input
                  v-model="updateForm.sku"
                  type="text"
                  required
                />
              </div>

              <div class="form-group">
                <label>Category</label>
                <input
                  v-model="updateForm.category"
                  type="text"
                  required
                />
              </div>

              <div class="form-group">
                <label>Price</label>
                <input
                  v-model.number="updateForm.price"
                  type="number"
                  min="0"
                  step="0.01"
                  required
                />
              </div>

              <div class="form-group">
                <label>Minimum Stock</label>
                <input
                  v-model.number="updateForm.minimumStock"
                  type="number"
                  min="0"
                  required
                />
              </div>
            </div>

            <div class="form-group">
              <label>Description</label>
              <textarea
                v-model="updateForm.description"
                rows="4"
              ></textarea>
            </div>

            <p v-if="updateError" class="error">
              {{ updateError }}
            </p>

            <button class="submit-btn" type="submit" :disabled="updating || !updateForm.name">
              {{ updating ? 'Updating...' : 'Update Product' }}
            </button>
          </form>
        </div>
      </div>

      <!-- Transactions Modal -->
      <div
        v-if="showTransactionsModal"
        class="modal-overlay"
        @click.self="closeTransactionsModal"
      >
        <div class="modal transactions-modal">
          <div class="modal-header">
            <div>
              <h2>Transaction History</h2>
              <p>{{ selectedProduct.name }}</p>
            </div>

            <button class="close-btn" @click="closeTransactionsModal">
              ×
            </button>
          </div>

          <div v-if="loadingTransactions" class="message">
            Loading transactions...
          </div>

          <div
            v-else-if="transactions.length === 0"
            class="message"
          >
            No transactions found for this product.
          </div>

          <div v-else class="transaction-table">
            <table>
              <thead>
                <tr>
                  <th>Type</th>
                  <th>Quantity</th>
                  <th>Reason</th>
                  <th>User</th>
                  <th>Date</th>
                </tr>
              </thead>

              <tbody>
                <tr
                  v-for="transaction in transactions"
                  :key="transaction.id"
                >
                  <td>
                    <span
                      :class="
                        transaction.type === 'StockIn'
                          ? 'in-stock'
                          : 'stock-out'
                      "
                    >
                      {{
                        transaction.type === 'StockIn'
                          ? 'Stock In'
                          : 'Stock Out'
                      }}
                    </span>
                  </td>

                  <td>{{ transaction.quantity }}</td>

                  <td>
                    {{ transaction.reason || '—' }}
                  </td>

                  <td>{{ transaction.userId }}</td>

                  <td>
                    {{ formatDate(transaction.createdAt) }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'
import { formatDate } from '../utils/formatDate'

const router = useRouter()

const products = ref([])
const loading = ref(true)
const error = ref('')


const showForm = ref(false)
const saving = ref(false)
const formError = ref('')
const showProductModal = ref(false)
const showUpdateModal = ref(false)
const showTransactionsModal = ref(false)

const selectedProduct = ref(null)

const transactions = ref([])
const loadingTransactions = ref(false)
const updating = ref(false)
const updateError = ref('')
const form = ref({
  name: '',
  sku: '',
  description: '',
  category: '',
  price: 0,
  stockQuantity: 0,
  minimumStock: 0
})
const updateForm = ref({
  id: '',
  name: '',
  sku: '',
  description: '',
  category: '',
  price: 0,
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

const openProduct = (product) => {
  selectedProduct.value = product
  showProductModal.value = true
}

const closeProductModal = () => {
  showProductModal.value = false
  selectedProduct.value = null
}

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

const openUpdateModal = (product) => {
  updateForm.value = {
    id: product.id,
    name: product.name,
    sku: product.sku,
    description: product.description,
    category: product.category,
    price: product.price,
    minimumStock: product.minimumStock
  }

  updateError.value = ''

  showProductModal.value = false
  showUpdateModal.value = true
}

const closeUpdateModal = () => {
  showUpdateModal.value = false

  updateForm.value = {
    id: '',
    name: '',
    sku: '',
    description: '',
    category: '',
    price: 0,
    minimumStock: 0
  }

  updateError.value = ''
}

const openTransactionsModal = async (product) => {
  selectedProduct.value = product
  showProductModal.value = false
  showTransactionsModal.value = true

  transactions.value = []
  loadingTransactions.value = true

  try {
    const response = await api.get(
      `/Inventory/${encodeURIComponent(product.id)}`
    )

    transactions.value = response.data
  } catch (err) {
    console.error('Failed to load transactions:', err)

    if (err.response?.status === 401) {
      localStorage.removeItem('token')
      router.push('/login')
      return
    }

    if (err.response?.status === 403) {
      transactions.value = []
      return
    }
  } finally {
    loadingTransactions.value = false
  }
}

const closeTransactionsModal = () => {
  showTransactionsModal.value = false
  transactions.value = []
}

const updateProduct = async () => {
  updateError.value = ''
  updating.value = true

  try {
    await api.put(
      `/Products/${encodeURIComponent(updateForm.value.id)}`,
      {
        name: updateForm.value.name,
        sku: updateForm.value.sku,
        description: updateForm.value.description,
        category: updateForm.value.category,
        price: updateForm.value.price,
        minimumStock: updateForm.value.minimumStock, 
      }
    )

    showUpdForm.value = false

    updateForm.value = {
      id: '',
      name: '',
      sku: '',
      description: '',
      category: '',
      price: 0,
      minimumStock: 0
    }

    await loadProducts()
  } catch (err) {
    console.error('Failed to update product', err)

    if (err.response?.status === 401) {
      localStorage.removeItem('token')
      router.push('/login')
      return
    }

    if (err.response?.status === 403) {
      updateError.value = 'You do not have permission to update'
      return
    }

    updateError.value = err.response?.data?.message || 'Failed to update product'
  } finally {
    updating.value = false
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

.header-action {
  display: flex;
  gap: 10px;
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
textarea,
select {
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

/* Product row */

.product-row {
  cursor: pointer;
  transition: background 0.15s ease;
}

.product-row:hover {
  background: #f3f4f6;
}


/* Modal */

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);

  display: flex;
  align-items: center;
  justify-content: center;

  z-index: 1000;
  padding: 20px;
}

.modal {
  width: 100%;
  max-width: 650px;
  max-height: 90vh;
  overflow-y: auto;

  background: white;
  border-radius: 12px;
  padding: 25px;

  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
}

.transactions-modal {
  max-width: 900px;
}

.modal-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;

  margin-bottom: 25px;
}

.modal-header h2 {
  margin: 0 0 5px;
}

.modal-header p {
  margin: 0;
  color: #6b7280;
}

.close-btn {
  border: none;
  background: none;

  font-size: 28px;
  line-height: 1;

  cursor: pointer;
  color: #6b7280;
}

.close-btn:hover {
  color: #111827;
}


/* Product details */

.product-details {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 18px;

  margin-bottom: 25px;
}

.detail-item {
  padding: 14px;
  background: #f9fafb;
  border-radius: 8px;
}

.detail-item span {
  display: block;
  margin-bottom: 5px;

  font-size: 13px;
  color: #6b7280;
}

.detail-item strong {
  font-size: 15px;
}

.detail-item.description {
  grid-column: 1 / -1;
}

.detail-item.description p {
  margin: 5px 0 0;
  line-height: 1.5;
}


/* Modal actions */

.modal-actions {
  display: flex;
  gap: 10px;

  padding-top: 20px;
  border-top: 1px solid #e5e7eb;
}

.action-btn {
  padding: 11px 18px;

  border: none;
  border-radius: 6px;

  cursor: pointer;
  font-weight: 500;
}


/* Transactions */

.transaction-table {
  overflow-x: auto;
}

.transaction-table table {
  width: 100%;
  border-collapse: collapse;
}

.transaction-table th,
.transaction-table td {
  padding: 13px;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
}

.transaction-table th {
  background: #f9fafb;
}

.in-stock {
  color: #15803d;
  font-weight: 600;
}

.stock-out {
  color: #dc2626;
  font-weight: 600;
}


/* Mobile */

@media (max-width: 700px) {
  .modal {
    padding: 20px;
  }

  .product-details {
    grid-template-columns: 1fr;
  }

  .detail-item.description {
    grid-column: auto;
  }

  .modal-actions {
    flex-direction: column;
  }
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