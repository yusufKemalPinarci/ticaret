import { useState, useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { useNavigate, Link } from 'react-router-dom'
import { AppDispatch, RootState } from '../store/store'
import { login, clearError } from '../store/slices/authSlice'
import { Eye, EyeOff } from 'lucide-react'

export default function LoginPage() {
  const dispatch = useDispatch<AppDispatch>()
  const navigate = useNavigate()
  const { accessToken, user, isLoading, error } = useSelector((state: RootState) => state.auth)
  
  const [formData, setFormData] = useState({
    email: '',
    password: '',
  })
  const [showPassword, setShowPassword] = useState(false)

  useEffect(() => {
    const doAfterLogin = async () => {
      if (accessToken) {
        // if anonymous cart exists, merge it into user cart
        const sid = localStorage.getItem('cartSessionId')
        if (sid) {
          try {
            const { cartApi } = await import('../services/api')
            await cartApi.mergeCart(sid)
            localStorage.removeItem('cartSessionId')
            // refresh cart
            const { store } = await import('../store/store')
            const { fetchCart } = await import('../store/slices/cartSlice')
            store.dispatch(fetchCart())
          } catch (err) {
            console.error('Cart merge failed', err)
          }
        }
        const role = user?.role
        if (role === 'Admin') navigate('/admin')
        else navigate('/')
      }
    }
    doAfterLogin()
    return () => {
      dispatch(clearError())
    }
  }, [accessToken, user, navigate, dispatch])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    dispatch(login(formData))
  }

  return (
    <div className="container mx-auto px-4 py-12">
      <div className="max-w-md mx-auto">
        <h1 className="text-3xl font-bold mb-8 text-center">Giriş Yap</h1>
        
        <form onSubmit={handleSubmit} className="space-y-4">
          {error && (
            <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded space-y-1 text-sm">
              <div>{error}</div>
              {error.toLowerCase().includes('locked') && (
                <div className="text-red-800/80">Çok sayıda başarısız deneme tespit edildi. Lütfen 15 dakika sonra tekrar deneyin.</div>
              )}
              {error.toLowerCase().includes('too many requests') && (
                <div className="text-red-800/80">Kısa bir süre bekleyip yeniden deneyebilirsiniz.</div>
              )}
            </div>
          )}

          <div>
            <label className="block mb-2 font-semibold">E-posta</label>
            <input
              type="email"
              value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })}
              className="input"
              required
            />
          </div>

          <div>
            <label className="block mb-2 font-semibold">Şifre</label>
            <div className="relative">
              <input
                type={showPassword ? 'text' : 'password'}
                value={formData.password}
                onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                className="input pr-10"
                required
              />
              <button
                type="button"
                className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-500 hover:text-primary-600"
                onClick={() => setShowPassword(!showPassword)}
              >
                {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
              </button>
            </div>
          </div>

          <button
            type="submit"
            disabled={isLoading}
            className="btn btn-primary w-full disabled:opacity-50"
          >
            {isLoading ? 'Giriş yapılıyor...' : 'Giriş Yap'}
          </button>
        </form>

        <div className="mt-4 text-center text-gray-600 space-y-2">
          <p>
            <Link to="/forgot-password" className="text-primary-600 hover:underline">
              Şifreni mi unuttun?
            </Link>
          </p>
          <p>
            Hesabınız yok mu?{' '}
            <Link to="/register" className="text-primary-600 hover:underline">
              Kayıt Ol
            </Link>
          </p>
          <p className="text-sm text-gray-500">
            Doğrulama e-postasına mı ihtiyacın var?{' '}
            <Link to="/verify-email" className="text-primary-600 hover:underline">Tekrar gönder</Link>
          </p>
        </div>
      </div>
    </div>
  )
}
