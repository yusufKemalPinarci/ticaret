import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit'
import { cartApi, ensureCartSessionId } from '../../services/api'

interface CartItem {
  id: number
  productId: number
  productName: string
  productImageUrl: string
  quantity: number
  unitPrice: number
  totalPrice: number
}

interface CartState {
  items: CartItem[]
  totalAmount: number
  itemCount: number
  isLoading: boolean
  error: string | null
}

const initialState: CartState = {
  items: [],
  totalAmount: 0,
  itemCount: 0,
  isLoading: false,
  error: null,
}

export const fetchCart = createAsyncThunk(
  'cart/fetchCart',
  async (sessionId?: string) => {
    // manage client session id for anonymous users
    let sid = sessionId || ensureCartSessionId()
    const response = await cartApi.getCart(sid)
    return response
  }
)

export const addToCart = createAsyncThunk(
  'cart/addToCart',
  async ({ productId, quantity, sessionId }: { productId: number; quantity: number; sessionId?: string }) => {
    // ensure we have a session id for anonymous carts
    const sid = sessionId || ensureCartSessionId()
    const response = await cartApi.addItem(productId, quantity, sid)
    return response
  }
)

export const removeFromCart = createAsyncThunk(
  'cart/removeFromCart',
  async (cartItemId: number) => {
    await cartApi.removeItem(cartItemId)
    return cartItemId
  }
)

export const clearCart = createAsyncThunk(
  'cart/clearCart',
  async (sessionId?: string) => {
    let sid = sessionId
    if (!sid) sid = localStorage.getItem('cartSessionId') || undefined
    await cartApi.clearCart(sid)
  }
)

export const updateCartItem = createAsyncThunk(
  'cart/updateCartItem',
  async ({ cartItemId, quantity }: { cartItemId: number; quantity: number }) => {
    const response = await cartApi.updateItem(cartItemId, quantity)
    return response
  }
)

const cartSlice = createSlice({
  name: 'cart',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchCart.pending, (state) => {
        state.isLoading = true
      })
      .addCase(fetchCart.fulfilled, (state, action: PayloadAction<any>) => {
        state.isLoading = false
        state.items = action.payload.items || []
        state.totalAmount = action.payload.totalAmount || 0
        state.itemCount = action.payload.itemCount || 0
      })
      .addCase(fetchCart.rejected, (state, action) => {
        state.isLoading = false
        state.error = action.error.message || 'Failed to fetch cart'
      })
      .addCase(addToCart.fulfilled, (state, action: PayloadAction<any>) => {
        state.items = action.payload.items || []
        state.totalAmount = action.payload.totalAmount || 0
        state.itemCount = action.payload.itemCount || 0
      })
      .addCase(updateCartItem.fulfilled, (state, action: PayloadAction<any>) => {
        state.items = action.payload.items || []
        state.totalAmount = action.payload.totalAmount || 0
        state.itemCount = action.payload.itemCount || 0
      })
      .addCase(removeFromCart.fulfilled, (state, action: PayloadAction<number>) => {
        state.items = state.items.filter(item => item.id !== action.payload)
        state.itemCount = state.items.reduce((sum, item) => sum + item.quantity, 0)
        state.totalAmount = state.items.reduce((sum, item) => sum + item.totalPrice, 0)
      })
      .addCase(clearCart.fulfilled, (state) => {
        state.items = []
        state.totalAmount = 0
        state.itemCount = 0
      })
  },
})

export default cartSlice.reducer
