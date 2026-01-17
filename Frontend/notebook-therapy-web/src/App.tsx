import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout/Layout'
import HomePage from './pages/HomePage'
import ProductsPage from './pages/ProductsPage'
import ProductDetailPage from './pages/ProductDetailPage'
import CartPage from './pages/CartPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import AdminDashboard from './pages/admin/AdminDashboard'
import AdminProducts from './pages/admin/AdminProducts'
import AdminCategories from './pages/admin/AdminCategories'
import AdminOrders from './pages/admin/AdminOrders'
import AdminUsers from './pages/admin/AdminUsers'
import RequireAdmin from './components/Admin/RequireAdmin'
import CheckoutPage from './pages/CheckoutPage'
import NotFoundPage from './pages/NotFoundPage'
import ErrorBoundary from './components/ErrorBoundary'
import CheckoutSuccessPage from './pages/CheckoutSuccessPage'
import ForgotPasswordPage from './pages/ForgotPasswordPage'
import ResetPasswordPage from './pages/ResetPasswordPage'
import VerifyEmailPage from './pages/VerifyEmailPage'
import ProfilePage from './pages/ProfilePage'
import MyOrdersPage from './pages/MyOrdersPage'
import WishlistPage from './pages/WishlistPage'
import { useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { AppDispatch, RootState } from './store/store'
import { fetchWishlist } from './store/slices/wishlistSlice'

function App() {
  const dispatch = useDispatch<AppDispatch>()
  const { user } = useSelector((state: RootState) => state.auth)

  useEffect(() => {
    if (user) {
      dispatch(fetchWishlist())
    }
  }, [user, dispatch])

  return (
    <ErrorBoundary>
      <Layout>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/products" element={<ProductsPage />} />
          <Route path="/products/:id" element={<ProductDetailPage />} />
          <Route path="/cart" element={<CartPage />} />
          <Route path="/checkout" element={<CheckoutPage />} />
          <Route path="/checkout/success" element={<CheckoutSuccessPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/forgot-password" element={<ForgotPasswordPage />} />
          <Route path="/reset-password" element={<ResetPasswordPage />} />
          <Route path="/verify-email" element={<VerifyEmailPage />} />
          <Route path="/profile" element={<ProfilePage />} />
          <Route path="/orders" element={<MyOrdersPage />} />
          <Route path="/wishlist" element={<WishlistPage />} />
          <Route path="/admin" element={<RequireAdmin><AdminDashboard /></RequireAdmin>} />
          <Route path="/admin/products" element={<RequireAdmin><AdminProducts /></RequireAdmin>} />
          <Route path="/admin/categories" element={<RequireAdmin><AdminCategories /></RequireAdmin>} />
          <Route path="/admin/orders" element={<RequireAdmin><AdminOrders /></RequireAdmin>} />
          <Route path="/admin/users" element={<RequireAdmin><AdminUsers /></RequireAdmin>} />
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </Layout>
    </ErrorBoundary>
  )
}

export default App
