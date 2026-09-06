<template>
  <div class="login-page">
    <div class="login-card">
      <h1>Inventory Management</h1>
      <h2>Login</h2>

      <form @submit.prevent="login">
        <div class="form-group">
          <label>Email</label>
          <input
            v-model="email"
            type="email"
            placeholder="Enter your email"
            required
          />
        </div>

        <div class="form-group">
          <label>Password</label>
          <input
            v-model="password"
            type="password"
            placeholder="Enter your password"
            required
          />
        </div>

        <p v-if="error" class="error">
          {{ error }}
        </p>

        <button type="submit" :disabled="loading">
          {{ loading ? 'Logging in...' : 'Login' }}
        </button>
      </form>

      <p class="register-link">
        Don't have an account?
        <router-link to="/register">Register</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'

const router = useRouter()

const email = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

const login = async () => {
  error.value = ''
  loading.value = true

  try {
    const response = await api.post('Auth/login', {
      email: email.value,
      password: password.value
    })

    const token = response.data.token

    localStorage.setItem('token', token)

    router.push('/dashboard')
  } catch (err) {
    error.value =
      err.response?.data?.message || 'Login failed. Please check your credentials.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: #f4f6f8;
}

.login-card {
  width: 400px;
  padding: 30px;
  background: white;
  border-radius: 10px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
}

h1 {
  text-align: center;
  margin-bottom: 10px;
}

h2 {
  text-align: center;
  margin-bottom: 25px;
}

.form-group {
  margin-bottom: 18px;
}

label {
  display: block;
  margin-bottom: 6px;
  font-weight: bold;
}

input {
  width: 100%;
  padding: 10px;
  box-sizing: border-box;
  border: 1px solid #ccc;
  border-radius: 5px;
}

button {
  width: 100%;
  padding: 11px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 16px;
}

button:disabled {
  cursor: not-allowed;
}

.error {
  color: red;
  margin-bottom: 15px;
}

.register-link {
  text-align: center;
  margin-top: 20px;
}
</style>