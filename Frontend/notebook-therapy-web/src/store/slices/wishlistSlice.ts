import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit'
import { wishlistApi } from '../../services/api'

export interface WishlistItem {
  id: number
  productId: number
  productName: string
  productImageUrl: string
  productPrice: number
  productDiscountPrice: number | null
}

interface WishlistState {
  items: WishlistItem[]
  loading: boolean
  error: string | null
}

const initialState: WishlistState = {
  items: [],
  loading: false,
  error: null,
}

export const fetchWishlist = createAsyncThunk('wishlist/fetch', async () => {
  const data = await wishlistApi.get()
  return data
})

export const addToWishlist = createAsyncThunk('wishlist/add', async (productId: number) => {
  await wishlistApi.add(productId)
  const data = await wishlistApi.get()
  return data
})

export const removeFromWishlist = createAsyncThunk('wishlist/remove', async (productId: number) => {
  await wishlistApi.remove(productId)
  return productId
})

const wishlistSlice = createSlice({
  name: 'wishlist',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchWishlist.pending, (state) => {
        state.loading = true
      })
      .addCase(fetchWishlist.fulfilled, (state, action: PayloadAction<WishlistItem[]>) => {
        state.loading = false
        state.items = action.payload
      })
      .addCase(fetchWishlist.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || 'Failed to fetch wishlist'
      })
      .addCase(addToWishlist.fulfilled, (state, action: PayloadAction<WishlistItem[]>) => {
        state.items = action.payload
      })
      .addCase(removeFromWishlist.fulfilled, (state, action: PayloadAction<number>) => {
        state.items = state.items.filter(item => item.id !== action.payload)
      })
  },
})

export default wishlistSlice.reducer
