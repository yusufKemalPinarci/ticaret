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
  const [isCorporate, setIsCorporate] = useState(false)
  const [tcKimlikNo, setTcKimlikNo] = useState('')
  const [taxNumber, setTaxNumber] = useState('')
  const [taxOffice, setTaxOffice] = useState('')
  const [companyName, setCompanyName] = useState('')
  const [isLoading, setIsLoading] = useState(false)

  useEffect(() => {
    userApi.getMe().then(data => {
        setFirstName(data.firstName || '')
        setLastName(data.lastName || '')
        setPhoneNumber(data.phoneNumber || '')
        setIsCorporate(data.isCorporate || false)
        setTcKimlikNo(data.tcKimlikNo || '')
        setTaxNumber(data.taxNumber || '')
        setTaxOffice(data.taxOffice || '')
        setCompanyName(data.companyName || '')
    }).catch(() => {})
  }, [])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsLoading(true)
    try {
      await userApi.updateProfile({
        firstName,
        lastName,
        phoneNumber,
        isCorporate,
        tcKimlikNo: !isCorporate ? tcKimlikNo : '',
        taxNumber: isCorporate ? taxNumber : '',
        taxOffice: isCorporate ? taxOffice : '',
        companyName: isCorporate ? companyName : ''
      })
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

        <div className="pt-4 border-t">
          <h3 className="font-bold mb-3 text-gray-700">Fatura Tercihleri</h3>
          <div className="flex gap-4 mb-4">
            <label className="flex items-center gap-2 cursor-pointer">
              <input type="radio" checked={!isCorporate} onChange={() => setIsCorporate(false)} name="billingType" />
              <span className="text-sm">Bireysel</span>
            </label>
            <label className="flex items-center gap-2 cursor-pointer">
              <input type="radio" checked={isCorporate} onChange={() => setIsCorporate(true)} name="billingType" />
              <span className="text-sm">Kurumsal</span>
            </label>
          </div>

          {!isCorporate ? (
            <div>
              <label className="block text-sm font-medium mb-1 text-gray-600">TC Kimlik No</label>
              <input
                className="input"
                value={tcKimlikNo}
                onChange={(e) => setTcKimlikNo(e.target.value)}
                maxLength={11}
              />
            </div>
          ) : (
            <div className="space-y-3">
              <div>
                <label className="block text-sm font-medium mb-1 text-gray-600">Firma Adı</label>
                <input className="input" value={companyName} onChange={(e) => setCompanyName(e.target.value)} />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-1 text-gray-600">Vergi No</label>
                  <input className="input" value={taxNumber} onChange={(e) => setTaxNumber(e.target.value)} maxLength={11} />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1 text-gray-600">Vergi Dairesi</label>
                  <input className="input" value={taxOffice} onChange={(e) => setTaxOffice(e.target.value)} />
                </div>
              </div>
            </div>
          )}
        </div>

        <button type="submit" className="btn btn-primary w-full pt-4" disabled={isLoading}>
          {isLoading ? 'Güncelleniyor...' : 'Bilgileri Kaydet'}
        </button>
      </form>
    </div>
  )
}
