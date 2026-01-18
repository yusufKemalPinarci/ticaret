import { useEffect, useState } from 'react'
import { adminApi } from '../../services/api'
import { ShieldCheck, FileText, CheckCircle, AlertCircle, Search } from 'lucide-react'
import PageMeta from '../../components/PageMeta'

export default function AdminCompliance() {
  const [orders, setOrders] = useState<any[]>([])
  const [loading, setLoading] = useState(true)
  const [query, setQuery] = useState('')

  useEffect(() => {
    fetchOrders()
  }, [])

  const fetchOrders = async () => {
    try {
      // For now, reuse adminApi.getOrders or similar, but we filter for invoicing data
      const data = await adminApi.getOrders()
      setOrders(data || [])
    } catch (err) {
      console.error(err)
    } finally {
      setLoading(false)
    }
  }

  const filtered = orders.filter(o =>
    o.orderNumber.toLowerCase().includes(query.toLowerCase()) ||
    o.tcKimlikNo?.includes(query) ||
    o.taxNumber?.includes(query) ||
    o.companyName?.toLowerCase().includes(query.toLowerCase())
  )

  return (
    <div className="container mx-auto p-6 space-y-6">
      <PageMeta title="Yasal Uyumluluk | HediyeJoy Admin" />
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold flex items-center gap-2">
            <ShieldCheck className="text-emerald-600" />
            Yasal Uyumluluk ve Faturalandırma
        </h1>
      </div>

      <div className="card p-4 flex items-center gap-3">
        <Search className="text-gray-400 w-5 h-5" />
        <input
            className="input border-none focus:ring-0"
            placeholder="Sipariş no, TC No, Vergi No veya Firma ara..."
            value={query}
            onChange={(e) => setQuery(e.target.value)}
        />
      </div>

      <div className="space-y-4">
        {loading ? (
          <div>Yükleniyor...</div>
        ) : filtered.length === 0 ? (
          <div className="text-center py-12 text-gray-500">Kayıt bulunamadı.</div>
        ) : (
          <div className="overflow-x-auto card">
            <table className="w-full text-left text-sm">
              <thead className="bg-gray-50 border-b">
                <tr>
                  <th className="p-3">Sipariş No</th>
                  <th className="p-3">Tip</th>
                  <th className="p-3">Kimlik / Vergi Detayları</th>
                  <th className="p-3">KVKK</th>
                  <th className="p-3">İşlem</th>
                </tr>
              </thead>
              <tbody className="divide-y">
                {filtered.map(o => (
                  <tr key={o.id} className="hover:bg-gray-50">
                    <td className="p-3 font-medium">{o.orderNumber}</td>
                    <td className="p-3">
                      {o.isCorporate ? (
                        <span className="bg-purple-100 text-purple-700 px-2 py-0.5 rounded-full text-xs">Kurumsal</span>
                      ) : (
                        <span className="bg-blue-100 text-blue-700 px-2 py-0.5 rounded-full text-xs">Bireysel</span>
                      )}
                    </td>
                    <td className="p-3">
                      {o.isCorporate ? (
                        <div className="space-y-0.5">
                          <div className="font-bold">{o.companyName}</div>
                          <div className="text-gray-500">VN: {o.taxNumber}</div>
                          <div className="text-gray-500">VD: {o.taxOffice}</div>
                        </div>
                      ) : (
                        <div>TC: {o.tcKimlikNo || 'Belirtilmedi'}</div>
                      )}
                    </td>
                    <td className="p-3">
                      {o.kvkkApproved ? (
                        <CheckCircle className="text-green-500 w-5 h-5" />
                      ) : (
                        <AlertCircle className="text-red-500 w-5 h-5" />
                      )}
                    </td>
                    <td className="p-3">
                        <button className="text-primary-600 hover:underline flex items-center gap-1">
                            <FileText className="w-4 h-4" />
                            Fatura Hazırla
                        </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  )
}
