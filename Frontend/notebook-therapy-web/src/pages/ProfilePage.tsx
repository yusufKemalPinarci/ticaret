import { useState, useEffect } from 'react'
import { useSelector, useDispatch } from 'react-redux'
import { RootState } from '../store/store'
import { userApi } from '../services/api'
import { setUser } from '../store/slices/authSlice'
import { publishNotice } from '../services/notifications'
import PageMeta from '../components/PageMeta'

export default function ProfilePage() {
  const { user } = useSelector((state: RootState) => state.auth)
  const dispatch = useDispatch()
  const [firstName, setFirstName] = useState(user?.firstName || '')
  const [lastName, setLastName] = useState(user?.lastName || '')
  const [phoneNumber, setPhoneNumber] = useState('')
  const [isLoading, setIsLoading] = useState(false)

  useEffect(() => {
    if (user) {
      setFirstName(user.firstName)
      setLastName(user.lastName)
    }
    userApi.getMe().then(data => {
        setPhoneNumber(data.phoneNumber || '')
    }).catch(() => {})
  }, [user])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsLoading(true)
    try {
      await userApi.updateProfile({ firstName, lastName, phoneNumber })
      dispatch(setUser({ ...user!, firstName, lastName }))
      publishNotice({ kind: 'success', message: 'Profil başarıyla güncellendi.' })
    } catch (err: any) {
      publishNotice({ kind: 'warning', message: 'Profil güncellenirken bir hata oluştu.' })
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="container mx-auto px-4 py-8 max-w-2xl">
      <PageMeta title="Profilim | HediyeJoy" />
      <h1 className="text-3xl font-bold mb-6">Profil Bilgileri</h1>
      <form onSubmit={handleSubmit} className="card p-6 space-y-4">
        <div>
          <label className="block text-sm font-medium mb-1">E-posta</label>
          <input type="text" className="input bg-gray-50" value={user?.email || ''} disabled />
          <p className="text-xs text-gray-500 mt-1">E-posta adresi değiştirilemez.</p>
        </div>
        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium mb-1">Ad</label>
            <input
              type="text"
              className="input"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Soyad</label>
            <input
              type="text"
              className="input"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
              required
            />
          </div>
        </div>
        <div>
          <label className="block text-sm font-medium mb-1">Telefon</label>
          <input
            type="text"
            className="input"
            value={phoneNumber}
            onChange={(e) => setPhoneNumber(e.target.value)}
            placeholder="05xx xxx xx xx"
          />
        </div>
        <button type="submit" className="btn btn-primary w-full" disabled={isLoading}>
          {isLoading ? 'Güncelleniyor...' : 'Bilgileri Kaydet'}
        </button>
      </form>
    </div>
  )
}
