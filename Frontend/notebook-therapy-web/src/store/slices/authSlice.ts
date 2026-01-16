import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit'
import { authApi, clearTokens as clearStoredTokens } from '../../services/api'

export interface AuthUser {
  email: string
  firstName: string
  lastName: string
  role: string
}

interface AuthState {
  user: AuthUser | null
  accessToken: string | null
  refreshToken: string | null
  accessTokenExpiresAt: string | null
  refreshTokenExpiresAt: string | null
  isLoading: boolean
  error: string | null
}

export function parseToken(token: string | null): AuthUser | null {
  if (!token) return null
  try {
    const payload = token.split('.')[1]
    const decoded = JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')))
    const role = decoded.role || decoded.Role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.roles || null
    const email = decoded.email || decoded.Email || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/emailaddress'] || null
    return {
      email,
      firstName: decoded.firstName || decoded.FirstName || null,
      lastName: decoded.lastName || decoded.LastName || null,
      role,
    }
  } catch {
    return null
  }
}

const initialAccess = localStorage.getItem('accessToken')
const initialState: AuthState = {
  user: parseToken(initialAccess),
  accessToken: initialAccess,
  refreshToken: localStorage.getItem('refreshToken'),
  accessTokenExpiresAt: localStorage.getItem('accessTokenExpiresAt'),
  refreshTokenExpiresAt: localStorage.getItem('refreshTokenExpiresAt'),
  isLoading: false,
  error: null,
}

export const login = createAsyncThunk(
  'auth/login',
  async (credentials: { email: string; password: string }, { rejectWithValue }) => {
    try {
      const response = await authApi.login(credentials)
      return response
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Login failed')
    }
  }
)

export const register = createAsyncThunk(
  'auth/register',
  async (userData: { email: string; password: string; firstName: string; lastName: string }, { rejectWithValue }) => {
    try {
      const response = await authApi.register(userData)
      return response
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Registration failed')
    }
  }
)

export const refreshTokens = createAsyncThunk(
  'auth/refresh',
  async (refreshToken: string, { rejectWithValue }) => {
    try {
      const response = await authApi.refresh(refreshToken)
      return response
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Session expired')
    }
  }
)

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    logout: (state) => {
      state.user = null
      state.accessToken = null
      state.refreshToken = null
      state.accessTokenExpiresAt = null
      state.refreshTokenExpiresAt = null
      clearStoredTokens()
    },
    clearError: (state) => {
      state.error = null
    },
    setUser: (state, action: PayloadAction<AuthUser | null>) => {
      state.user = action.payload
    },
    hydrateFromToken: (state, action: PayloadAction<string | null>) => {
      state.accessToken = action.payload
      state.user = parseToken(action.payload)
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(login.pending, (state) => {
        state.isLoading = true
        state.error = null
      })
      .addCase(login.fulfilled, (state, action: PayloadAction<any>) => {
        state.isLoading = false
        const token = action.payload.accessToken || action.payload.token || null
        state.user = parseToken(token) || {
          email: action.payload.email,
          firstName: action.payload.firstName,
          lastName: action.payload.lastName,
          role: action.payload.role,
        }
        state.accessToken = token
        state.refreshToken = action.payload.refreshToken || null
        state.accessTokenExpiresAt = action.payload.accessTokenExpiresAt || null
        state.refreshTokenExpiresAt = action.payload.refreshTokenExpiresAt || null
      })
      .addCase(login.rejected, (state, action) => {
        state.isLoading = false
        state.error = action.payload as string
      })
      .addCase(register.pending, (state) => {
        state.isLoading = true
        state.error = null
      })
      .addCase(register.fulfilled, (state, action: PayloadAction<any>) => {
        state.isLoading = false
        const token = action.payload.accessToken || action.payload.token || null
        state.user = parseToken(token) || {
          email: action.payload.email,
          firstName: action.payload.firstName,
          lastName: action.payload.lastName,
          role: action.payload.role,
        }
        state.accessToken = token
        state.refreshToken = action.payload.refreshToken || null
        state.accessTokenExpiresAt = action.payload.accessTokenExpiresAt || null
        state.refreshTokenExpiresAt = action.payload.refreshTokenExpiresAt || null
      })
      .addCase(register.rejected, (state, action) => {
        state.isLoading = false
        state.error = action.payload as string
      })
      .addCase(refreshTokens.fulfilled, (state, action: PayloadAction<any>) => {
        state.accessToken = action.payload.accessToken || action.payload.token
        state.refreshToken = action.payload.refreshToken || state.refreshToken
        state.accessTokenExpiresAt = action.payload.accessTokenExpiresAt || state.accessTokenExpiresAt
        state.refreshTokenExpiresAt = action.payload.refreshTokenExpiresAt || state.refreshTokenExpiresAt
        state.user = parseToken(state.accessToken)
      })
      .addCase(refreshTokens.rejected, (state, action) => {
        state.accessToken = null
        state.refreshToken = null
        state.accessTokenExpiresAt = null
        state.refreshTokenExpiresAt = null
        state.user = null
        state.error = action.payload as string
        localStorage.removeItem('accessToken')
        localStorage.removeItem('refreshToken')
        localStorage.removeItem('accessTokenExpiresAt')
        localStorage.removeItem('refreshTokenExpiresAt')
      })
  },
})

export const { logout, clearError, setUser, hydrateFromToken } = authSlice.actions
export default authSlice.reducer
