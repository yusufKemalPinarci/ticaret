import { useEffect, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { authApi } from '../services/api'

export default function VerifyEmailPage() {
  const [searchParams] = useSearchParams()
  const [email, setEmail] = useState('')
  const [token, setToken] = useState('')
  const [isSending, setIsSending] = useState(false)
  const [isVerifying, setIsVerifying] = useState(false)
  const [message, setMessage] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const emailParam = searchParams.get('email') || ''
    const tokenParam = searchParams.get('token') || ''
    if (emailParam) setEmail(emailParam)
    if (tokenParam) setToken(tokenParam)
  }, [searchParams])

  const sendLink = async () => {
    setIsSending(true)
    setError(null)
    setMessage(null)
    try {
      await authApi.sendVerifyEmail(email)
      setMessage('Eğer kayıtlıysa doğrulama e-postası gönderildi.')
    } catch (err: any) {
      const msg = err?.response?.data?.message || 'E-posta gönderilemedi. Lütfen tekrar deneyin.'
      setError(msg)
    } finally {
      setIsSending(false)
    }
  }

  const verify = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsVerifying(true)
    setError(null)
    setMessage(null)
    try {
      await authApi.verifyEmail({ email, token })
      setMessage('E-posta doğrulandı. Artık giriş yapabilirsiniz.')
    } catch (err: any) {
      const msg = err?.response?.data?.message || 'Doğrulama başarısız. Bilgileri kontrol edin.'
      setError(msg)
    } finally {
      setIsVerifying(false)
    }
  }

  return (
    <div className="container mx-auto px-4 py-12">
      <div className="max-w-md mx-auto">
        <h1 className="text-3xl font-bold mb-4 text-center">E-posta Doğrulama</h1>
        <p className="text-center text-gray-600 mb-6">E-postana gelen token ile hesabını doğrula veya yeniden gönder.</p>

        {message && <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded text-sm mb-4">{message}</div>}
        {error && <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded text-sm mb-4">{error}</div>}

        <form onSubmit={verify} className="space-y-4">
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

          <button
            type="submit"
            disabled={isVerifying}
            className="btn btn-primary w-full disabled:opacity-50"
          >
            {isVerifying ? 'Doğrulanıyor...' : 'Doğrula'}
          </button>
        </form>

        <div className="mt-6">
          <button
            type="button"
            onClick={sendLink}
            disabled={!email || isSending}
            className="btn btn-outline w-full disabled:opacity-50"
          >
            {isSending ? 'Gönderiliyor...' : 'Doğrulama bağlantısını yeniden gönder'}
          </button>
        </div>

        <p className="mt-4 text-center text-gray-600">
          Doğrulandı mı? <Link to="/login" className="text-primary-600 hover:underline">Giriş yap</Link>
        </p>
      </div>
    </div>
  )
}
