import axios, { AxiosError, AxiosRequestConfig } from 'axios'
import { publishNotice } from './notifications'

type TokenBundle = {
  accessToken: string | null
  refreshToken: string | null
  accessTokenExpiresAt?: string | null
  refreshTokenExpiresAt?: string | null
}

export const ensureCartSessionId = () => {
  let sid = localStorage.getItem('cartSessionId') || undefined
  if (!sid) {
    sid = `s_${Date.now()}_${Math.floor(Math.random() * 100000)}`
    localStorage.setItem('cartSessionId', sid)
  }
  return sid
}

export const generateIdempotencyKey = () => {
  if (typeof crypto !== 'undefined' && crypto.randomUUID) return crypto.randomUUID()
  return `idem_${Date.now()}_${Math.floor(Math.random() * 1_000_000)}`
}

let tokens: TokenBundle = {
  accessToken: localStorage.getItem('accessToken'),
  refreshToken: localStorage.getItem('refreshToken'),
  accessTokenExpiresAt: localStorage.getItem('accessTokenExpiresAt'),
  refreshTokenExpiresAt: localStorage.getItem('refreshTokenExpiresAt'),
}

const setTokens = (bundle: TokenBundle) => {
  tokens = bundle
  if (bundle.accessToken) localStorage.setItem('accessToken', bundle.accessToken)
  else localStorage.removeItem('accessToken')

  if (bundle.refreshToken) localStorage.setItem('refreshToken', bundle.refreshToken)
  else localStorage.removeItem('refreshToken')

  if (bundle.accessTokenExpiresAt) localStorage.setItem('accessTokenExpiresAt', bundle.accessTokenExpiresAt)
  else localStorage.removeItem('accessTokenExpiresAt')

  if (bundle.refreshTokenExpiresAt) localStorage.setItem('refreshTokenExpiresAt', bundle.refreshTokenExpiresAt)
  else localStorage.removeItem('refreshTokenExpiresAt')
}

export const clearTokens = () => {
  setTokens({ accessToken: null, refreshToken: null, accessTokenExpiresAt: null, refreshTokenExpiresAt: null })
}

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

api.interceptors.request.use((config) => {
  if (tokens.accessToken) {
    config.headers.Authorization = `Bearer ${tokens.accessToken}`
  }
  return config
})

let isRefreshing = false
let queue: Array<(token: string | null) => void> = []

const processQueue = (token: string | null) => {
  queue.forEach((resolve) => resolve(token))
  queue = []
}

api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const original = error.config as AxiosRequestConfig & { _retry?: boolean }
    if (error.response?.status === 429) {
      const retryAfter = Number(error.response.headers?.['retry-after'] ?? 0) || undefined
      publishNotice({
        kind: 'rate-limit',
        message: error.response.data?.message || 'Çok fazla istek gönderdiniz. Lütfen biraz bekleyip tekrar deneyin.',
        retryAfterSeconds: retryAfter,
      })
    }

    if (!error.response) {
      publishNotice({ kind: 'network', message: 'Bağlantı sorunu oluştu. Lütfen internetinizi kontrol edin.' })
    }
    if (error.response?.status === 401 && tokens.refreshToken && !original._retry) {
      original._retry = true

      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          queue.push((newToken) => {
            if (newToken) {
              original.headers = { ...(original.headers || {}), Authorization: `Bearer ${newToken}` }
              resolve(api(original))
            } else {
              reject(error)
            }
          })
        })
      }

      isRefreshing = true
      try {
        const refreshed = await api.post('/auth/refresh', { refreshToken: tokens.refreshToken })
        const { accessToken, refreshToken, accessTokenExpiresAt, refreshTokenExpiresAt } = refreshed.data
        setTokens({ accessToken, refreshToken, accessTokenExpiresAt, refreshTokenExpiresAt })
        processQueue(accessToken)
        original.headers = { ...(original.headers || {}), Authorization: `Bearer ${accessToken}` }
        return api(original)
      } catch (refreshError) {
        processQueue(null)
        clearTokens()
        publishNotice({ kind: 'warning', message: 'Oturum süreniz doldu. Lütfen yeniden giriş yapın.' })
        return Promise.reject(refreshError)
      } finally {
        isRefreshing = false
      }
    }

    if (error.response?.status === 401 && !tokens.refreshToken) {
      publishNotice({ kind: 'warning', message: 'Oturum süreniz sona erdi. Lütfen yeniden giriş yapın.' })
    }

    return Promise.reject(error)
  }
)

export const fileApi = {
  upload: async (file: File) => {
    const formData = new FormData()
    formData.append('file', file)

    const response = await api.post('/files/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    return response.data as { path: string; url: string }
  },
}

export const productApi = {
  getAll: async () => {
    const response = await api.get('/products')
    return response.data
  },
  create: async (data: any) => {
    const response = await api.post('/products', data)
    return response.data
  },
  update: async (id: number, data: any) => {
    const response = await api.put(`/products/${id}`, data)
    return response.data
  },
  delete: async (id: number) => {
    await api.delete(`/products/${id}`)
  },
  getById: async (id: number) => {
    const response = await api.get(`/products/${id}`)
    return response.data
  },
  getFeatured: async () => {
    const response = await api.get('/products/featured')
    return response.data
  },
  getNew: async () => {
    const response = await api.get('/products/new')
    return response.data
  },
  getBackInStock: async () => {
    const response = await api.get('/products/back-in-stock')
    return response.data
  },
  getByCategory: async (categoryId: number) => {
    const response = await api.get(`/products/category/${categoryId}`)
    return response.data
  },
  getByCollection: async (collection: string) => {
    const response = await api.get(`/products/collection/${collection}`)
    return response.data
  },
  search: async (query: string) => {
    const response = await api.get('/products/search', { params: { q: query } })
    return response.data
  },
}

export const categoryApi = {
  getAll: async () => {
    const response = await api.get('/categories')
    return response.data
  },
  create: async (data: any) => {
    const response = await api.post('/categories', data)
    return response.data
  },
  update: async (id: number, data: any) => {
    const response = await api.put(`/categories/${id}`, data)
    return response.data
  },
  delete: async (id: number) => {
    await api.delete(`/categories/${id}`)
  },
  getById: async (id: number) => {
    const response = await api.get(`/categories/${id}`)
    return response.data
  },
  getBySlug: async (slug: string) => {
    const response = await api.get(`/categories/slug/${slug}`)
    return response.data
  },
}

export const cartApi = {
  getCart: async (sessionId?: string) => {
    const params = sessionId ? { sessionId } : {}
    const response = await api.get('/cart', { params })
    return response.data
  },
  addItem: async (productId: number, quantity: number, sessionId?: string) => {
    const params = sessionId ? { sessionId } : {}
    const response = await api.post('/cart/items', { productId, quantity }, { params })
    return response.data
  },
  updateItem: async (cartItemId: number, quantity: number) => {
    const response = await api.put(`/cart/items/${cartItemId}`, { quantity })
    return response.data
  },
  removeItem: async (cartItemId: number) => {
    await api.delete(`/cart/items/${cartItemId}`)
  },
  clearCart: async (sessionId?: string) => {
    const params = sessionId ? { sessionId } : {}
    await api.delete('/cart', { params })
  },
  mergeCart: async (sessionId: string) => {
    const response = await api.post(`/cart/merge`, null, { params: { sessionId } })
    return response.data
  },
}

export const authApi = {
  login: async (credentials: { email: string; password: string }) => {
    const response = await api.post('/auth/login', credentials)
    const { accessToken, refreshToken, accessTokenExpiresAt, refreshTokenExpiresAt } = response.data
    setTokens({ accessToken, refreshToken, accessTokenExpiresAt, refreshTokenExpiresAt })
    return response.data
  },
  register: async (userData: { email: string; password: string; firstName: string; lastName: string }) => {
    const response = await api.post('/auth/register', userData)
    const { accessToken, refreshToken, accessTokenExpiresAt, refreshTokenExpiresAt } = response.data
    setTokens({ accessToken, refreshToken, accessTokenExpiresAt, refreshTokenExpiresAt })
    return response.data
  },
  refresh: async (refreshToken: string) => {
    const response = await api.post('/auth/refresh', { refreshToken })
    const { accessToken, refreshToken: newRefresh, accessTokenExpiresAt, refreshTokenExpiresAt } = response.data
    setTokens({ accessToken, refreshToken: newRefresh, accessTokenExpiresAt, refreshTokenExpiresAt })
    return response.data
  },
  forgotPassword: async (email: string) => {
    await api.post('/auth/forgot-password', { email })
  },
  resetPassword: async (payload: { email: string; token: string; newPassword: string }) => {
    await api.post('/auth/reset-password', payload)
  },
  sendVerifyEmail: async (email: string) => {
    await api.post('/auth/verify-email/send', { email })
  },
  verifyEmail: async (payload: { email: string; token: string }) => {
    await api.post('/auth/verify-email', payload)
  },
  logout: async (refreshToken?: string) => {
    const rt = refreshToken ?? tokens.refreshToken ?? localStorage.getItem('refreshToken')
    if (rt) {
      try { await api.post('/auth/logout', { refreshToken: rt }) } catch { /* ignore */ }
    }
    clearTokens()
    publishNotice({ kind: 'success', message: 'Oturum sonlandırıldı.' })
  },
}

export const ordersApi = {
  checkout: async (payload: any) => {
    const response = await api.post('/orders/checkout', payload)
    return response.data
  },
  createPaymentIntent: async (orderId: number, idempotencyKey: string, sessionId?: string) => {
    const response = await api.post(`/orders/${orderId}/payment-intent`, { idempotencyKey, sessionId })
    return response.data
  },
}

export default api
