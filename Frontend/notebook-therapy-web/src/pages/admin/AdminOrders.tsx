import { useEffect, useMemo, useState } from 'react'
import { Search } from 'lucide-react'

export default function AdminOrders() {
  const [orders, setOrders] = useState<any[]>([])
  const [query, setQuery] = useState('')
  const [statusFilter, setStatusFilter] = useState('')

  useEffect(() => { fetchOrders() }, [])

  const fetchOrders = async () => {
    const response = await (await import('../../services/api')).default.get('/admin/orders')
    setOrders(response.data || [])
  }

  const updateStatus = async (id: number, status: string) => {
    await (await import('../../services/api')).default.put(`/admin/orders/${id}/status`, status)
    fetchOrders()
  }

  const filtered = useMemo(() => {
    let list = [...orders]
    if (query.trim()) {
      const q = query.toLowerCase()
      list = list.filter((o) => o.orderNumber?.toLowerCase().includes(q) || o.customerEmail?.toLowerCase().includes(q))
    }
    if (statusFilter) list = list.filter((o) => o.status === statusFilter)
    return list
  }, [orders, query, statusFilter])

  return (
    <div className="container mx-auto p-6 space-y-4">
      <h1 className="text-2xl font-bold">Siparişler</h1>

      <div className="flex flex-wrap items-center gap-3">
        <div className="relative">
          <Search className="w-4 h-4 text-gray-400 absolute left-3 top-1/2 -translate-y-1/2" />
          <input className="input pl-8" placeholder="Sipariş no veya e-posta" value={query} onChange={(e) => setQuery(e.target.value)} />
        </div>
        <select className="input" value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
          <option value="">Tümü</option>
          <option>Pending</option>
          <option>Processing</option>
          <option>Shipped</option>
          <option>Completed</option>
          <option>Cancelled</option>
        </select>
      </div>

      <div className="space-y-3">
        {filtered.map(o => (
          <div key={o.id} className="card p-4">
            <div className="flex justify-between items-start">
              <div>
                <div className="font-semibold">{o.orderNumber} - {o.status}</div>
                <div className="text-sm text-gray-500">{new Date(o.createdAt).toLocaleString()}</div>
                <div className="mt-2">Toplam: ${o.totalAmount}</div>
                {o.customerEmail && <div className="text-sm text-gray-500">{o.customerEmail}</div>}
                <div className="mt-2">
                  {o.items.map((it: any) => (
                    <div key={it.productId} className="flex items-center space-x-3 mt-2">
                      <img src={it.productImageUrl} className="w-12 h-12 object-cover rounded" />
                      <div>
                        <div className="font-medium">{it.productName}</div>
                        <div className="text-sm text-gray-500">{it.quantity} x ${it.unitPrice}</div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
              <div className="space-y-2">
                <select className="input" defaultValue={o.status} onChange={e => updateStatus(o.id, e.target.value)}>
                  <option>Pending</option>
                  <option>Processing</option>
                  <option>Shipped</option>
                  <option>Completed</option>
                  <option>Cancelled</option>
                </select>
              </div>
            </div>
          </div>
        ))}
        {filtered.length === 0 && <div className="text-sm text-gray-500">Kayıt bulunamadı.</div>}
      </div>
    </div>
  )
}
