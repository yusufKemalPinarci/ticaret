import { useState, useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { useNavigate, Link } from 'react-router-dom'
import { AppDispatch, RootState } from '../store/store'
import { register, clearError } from '../store/slices/authSlice'

export default function RegisterPage() {
  const dispatch = useDispatch<AppDispatch>()
  const navigate = useNavigate()
  const { accessToken, isLoading, error } = useSelector((state: RootState) => state.auth)
  
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    firstName: '',
    lastName: '',
  })

  useEffect(() => {
    if (accessToken) {
      navigate('/')
    }
    return () => {
      dispatch(clearError())
    }
  }, [accessToken, navigate, dispatch])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    dispatch(register(formData))
  }

  return (
    <div className="container mx-auto px-4 py-12">
      <div className="max-w-md mx-auto">
        <h1 className="text-3xl font-bold mb-8 text-center">Kayıt Ol</h1>
        
        <form onSubmit={handleSubmit} className="space-y-4">
          {error && (
            <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
              {error}
            </div>
          )}

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block mb-2 font-semibold">Ad</label>
              <input
                type="text"
                value={formData.firstName}
                onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                className="input"
                required
              />
            </div>
            <div>
              <label className="block mb-2 font-semibold">Soyad</label>
              <input
                type="text"
                value={formData.lastName}
                onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                className="input"
                required
              />
            </div>
          </div>

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
            <input
              type="password"
              value={formData.password}
              onChange={(e) => setFormData({ ...formData, password: e.target.value })}
              className="input"
              required
              minLength={6}
            />
          </div>

          <button
            type="submit"
            disabled={isLoading}
            className="btn btn-primary w-full disabled:opacity-50"
          >
            {isLoading ? 'Kayıt yapılıyor...' : 'Kayıt Ol'}
          </button>
        </form>

        <div className="mt-4 text-center text-gray-600 space-y-2">
          <p>
            Zaten hesabınız var mı?{' '}
            <Link to="/login" className="text-primary-600 hover:underline">
              Giriş Yap
            </Link>
          </p>
          <p className="text-sm text-gray-500">
            Kayıttan sonra doğrulama e-postası gelmediyse <Link to="/verify-email" className="text-primary-600 hover:underline">buradan yeniden isteyebilirsiniz</Link>.
          </p>
        </div>
      </div>
    </div>
  )
}
