import { useEffect, useState } from 'react'
import { adminApi } from '../../services/api'
import { Trash2, Plus, Tag, AlertCircle } from 'lucide-react'
import { publishNotice } from '../../services/notifications'

export default function AdminCoupons() {
  const [coupons, setCoupons] = useState<any[]>([])
  const [loading, setLoading] = useState(true)
  const [showAdd, setShowAdd] = useState(false)
  const [form, setForm] = useState({
    code: '',
    discountType: 'fixed',
    amount: 0,
    minOrderAmount: 0,
    maxUsageCount: 0,
    isActive: true
  })

  useEffect(() => { fetchCoupons() }, [])

  const fetchCoupons = async () => {
    try {
      const data = await adminApi.getCoupons()
      setCoupons(data || [])
    } catch (err) {
      console.error(err)
    } finally {
      setLoading(false)
    }
  }

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      await adminApi.createCoupon(form)
      publishNotice({ kind: 'success', message: 'Kupon başarıyla oluşturuldu.' })
      setShowAdd(false)
      setForm({ code: '', discountType: 'fixed', amount: 0, minOrderAmount: 0, maxUsageCount: 0, isActive: true })
      fetchCoupons()
    } catch (err) {
      publishNotice({ kind: 'warning', message: 'Kupon oluşturulamadı.' })
    }
  }

  const handleDelete = async (id: number) => {
    if (!confirm('Bu kuponu silmek istediğinize emin misiniz?')) return
    try {
      await adminApi.deleteCoupon(id)
      publishNotice({ kind: 'success', message: 'Kupon silindi.' })
      fetchCoupons()
    } catch (err) {
      publishNotice({ kind: 'warning', message: 'Kupon silinemedi.' })
    }
  }

  return (
    <div className="container mx-auto p-6 space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-800">Kupon Yönetimi</h1>
        <button
          onClick={() => setShowAdd(!showAdd)}
          className="btn btn-primary flex items-center gap-2"
        >
          {showAdd ? 'İptal' : <><Plus className="w-4 h-4" /> Yeni Kupon</>}
        </button>
      </div>

      {showAdd && (
        <div className="card p-6 bg-gray-50 border-2 border-dashed border-gray-200 animate-in fade-in slide-in-from-top-4">
          <form onSubmit={handleCreate} className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="space-y-1">
              <label className="text-xs font-bold text-gray-500 uppercase">Kupon Kodu</label>
              <input
                className="input"
                placeholder="ÖRN: YAZ20"
                value={form.code}
                onChange={e => setForm({...form, code: e.target.value.toUpperCase()})}
                required
              />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-bold text-gray-500 uppercase">İndirim Tipi</label>
              <select className="input" value={form.discountType} onChange={e => setForm({...form, discountType: e.target.value})}>
                <option value="fixed">Sabit Tutar (₺)</option>
                <option value="percentage">Yüzde (%)</option>
              </select>
            </div>
            <div className="space-y-1">
              <label className="text-xs font-bold text-gray-500 uppercase">Miktar</label>
              <input
                type="number"
                className="input"
                value={form.amount}
                onChange={e => setForm({...form, amount: Number(e.target.value)})}
                required
              />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-bold text-gray-500 uppercase">Min. Sepet Tutarı</label>
              <input
                type="number"
                className="input"
                value={form.minOrderAmount}
                onChange={e => setForm({...form, minOrderAmount: Number(e.target.value)})}
              />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-bold text-gray-500 uppercase">Max Kullanım</label>
              <input
                type="number"
                className="input"
                value={form.maxUsageCount}
                onChange={e => setForm({...form, maxUsageCount: Number(e.target.value)})}
              />
            </div>
            <div className="flex items-end">
              <button type="submit" className="btn btn-primary w-full py-2.5">Kuponu Kaydet</button>
            </div>
          </form>
        </div>
      )}

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {coupons.map(c => (
          <div key={c.id} className="card p-5 group relative">
            <div className="flex justify-between items-start mb-4">
              <div className="flex items-center gap-2">
                <div className="p-2 bg-rose-100 text-rose-600 rounded-lg">
                  <Tag className="w-5 h-5" />
                </div>
                <div>
                  <h3 className="font-bold text-lg text-gray-800 tracking-wider">{c.code}</h3>
                  <span className={`text-[10px] font-bold uppercase px-1.5 py-0.5 rounded ${c.isActive ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}`}>
                    {c.isActive ? 'Aktif' : 'Pasif'}
                  </span>
                </div>
              </div>
              <button onClick={() => handleDelete(c.id)} className="text-gray-300 hover:text-red-500 transition-colors">
                <Trash2 className="w-5 h-5" />
              </button>
            </div>

            <div className="space-y-2 text-sm text-gray-600">
              <div className="flex justify-between">
                <span>İndirim:</span>
                <span className="font-bold text-gray-800">
                  {c.discountType === 'fixed' ? `₺${c.amount}` : `%${c.amount}`}
                </span>
              </div>
              <div className="flex justify-between">
                <span>Min. Tutar:</span>
                <span>₺{c.minOrderAmount || 0}</span>
              </div>
              <div className="flex justify-between">
                <span>Kullanım:</span>
                <span>{c.usageCount} / {c.maxUsageCount || '∞'}</span>
              </div>
            </div>

            {c.maxUsageCount > 0 && c.usageCount >= c.maxUsageCount && (
              <div className="mt-3 flex items-center gap-1.5 text-xs text-amber-600 font-medium bg-amber-50 p-2 rounded">
                <AlertCircle className="w-3.5 h-3.5" />
                Kullanım limitine ulaşıldı
              </div>
            )}
          </div>
        ))}

        {coupons.length === 0 && !loading && (
          <div className="col-span-full py-12 text-center text-gray-400">
            <Tag className="w-12 h-12 mx-auto mb-3 opacity-20" />
            <p>Henüz kupon oluşturulmadı.</p>
          </div>
        )}
      </div>
    </div>
  )
}
