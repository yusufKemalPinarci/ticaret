import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit'
import { productApi } from '../../services/api'

interface Product {
  id: number
  name: string
  description: string
  price: number
  discountPrice?: number
  imageUrl: string
  imageUrls: string[]
  stock: number
  isFeatured: boolean
  isNew: boolean
  isBackInStock: boolean
  collection: string
  categoryId: number
  categoryName: string
}

interface ProductState {
  products: Product[]
  featuredProducts: Product[]
  newProducts: Product[]
  backInStockProducts: Product[]
  currentProduct: Product | null
  isLoading: boolean
  error: string | null
}

const initialState: ProductState = {
  products: [],
  featuredProducts: [],
  newProducts: [],
  backInStockProducts: [],
  currentProduct: null,
  isLoading: false,
  error: null,
}

export const fetchProducts = createAsyncThunk('products/fetchAll', async () => {
  return await productApi.getAll()
})

export const fetchFeaturedProducts = createAsyncThunk('products/fetchFeatured', async () => {
  return await productApi.getFeatured()
})

export const fetchNewProducts = createAsyncThunk('products/fetchNew', async () => {
  return await productApi.getNew()
})

export const fetchBackInStockProducts = createAsyncThunk('products/fetchBackInStock', async () => {
  return await productApi.getBackInStock()
})

export const fetchProductById = createAsyncThunk('products/fetchById', async (id: number) => {
  return await productApi.getById(id)
})

export const searchProducts = createAsyncThunk('products/search', async (query: string) => {
  return await productApi.search(query)
})

const productSlice = createSlice({
  name: 'products',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchProducts.pending, (state) => {
        state.isLoading = true
      })
      .addCase(fetchProducts.fulfilled, (state, action: PayloadAction<Product[]>) => {
        state.isLoading = false
        state.products = action.payload
      })
      .addCase(fetchFeaturedProducts.fulfilled, (state, action: PayloadAction<Product[]>) => {
        state.featuredProducts = action.payload
      })
      .addCase(fetchNewProducts.fulfilled, (state, action: PayloadAction<Product[]>) => {
        state.newProducts = action.payload
      })
      .addCase(fetchBackInStockProducts.fulfilled, (state, action: PayloadAction<Product[]>) => {
        state.backInStockProducts = action.payload
      })
      .addCase(fetchProductById.fulfilled, (state, action: PayloadAction<Product>) => {
        state.currentProduct = action.payload
      })
      .addCase(searchProducts.fulfilled, (state, action: PayloadAction<Product[]>) => {
        state.products = action.payload
      })
      .addMatcher(
        (action) => action.type.endsWith('/rejected'),
        (state, action) => {
          state.isLoading = false
          state.error = action.error.message || 'An error occurred'
        }
      )
  },
})

export default productSlice.reducer
