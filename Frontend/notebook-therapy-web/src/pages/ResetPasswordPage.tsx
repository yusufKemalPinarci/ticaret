import { useEffect, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { authApi } from '../services/api'

export default function ResetPasswordPage() {
  const [searchParams] = useSearchParams()
  const [email, setEmail] = useState('')
  const [token, setToken] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [message, setMessage] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const emailParam = searchParams.get('email') || ''
    const tokenParam = searchParams.get('token') || ''
    if (emailParam) setEmail(emailParam)
    if (tokenParam) setToken(tokenParam)
  }, [searchParams])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsSubmitting(true)
    setError(null)
    setMessage(null)
    try {
      await authApi.resetPassword({ email, token, newPassword })
      setMessage('Şifreniz güncellendi. Şimdi giriş yapabilirsiniz.')
      setNewPassword('')
    } catch (err: any) {
      const msg = err?.response?.data?.message || 'Şifre sıfırlanamadı. Lütfen bilgileri kontrol edin.'
      setError(msg)
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="container mx-auto px-4 py-12">
      <div className="max-w-md mx-auto">
        <h1 className="text-3xl font-bold mb-4 text-center">Şifreyi Sıfırla</h1>
        <p className="text-center text-gray-600 mb-6">E-postana gelen bağlantıdaki token ile yeni şifreni belirle.</p>

        <form onSubmit={handleSubmit} className="space-y-4">
          {message && <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded text-sm">{message}</div>}
          {error && <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded text-sm">{error}</div>}

          <div>
            <label className="block mb-2 font-semibold">E-posta</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="input"
              required
            />
          </div>

          <div>
            <label className="block mb-2 font-semibold">Token</label>
            <input
              type="text"
              value={token}
              onChange={(e) => setToken(e.target.value)}
              className="input"
              required
            />
          </div>

          <div>
            <label className="block mb-2 font-semibold">Yeni Şifre</label>
            <input
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              className="input"
              required
              minLength={6}
            />
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="btn btn-primary w-full disabled:opacity-50"
          >
            {isSubmitting ? 'Güncelleniyor...' : 'Şifreyi Güncelle'}
          </button>
        </form>

        <p className="mt-4 text-center text-gray-600">
          Şifreni hatırladın mı? <Link to="/login" className="text-primary-600 hover:underline">Giriş Yap</Link>
        </p>
      </div>
    </div>
  )
}
