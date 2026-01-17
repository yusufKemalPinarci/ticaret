import { useEffect, useState } from 'react'
import { ordersApi } from '../services/api'
import { Package, Download, Clock, CheckCircle, Truck, AlertCircle } from 'lucide-react'
import PageMeta from '../components/PageMeta'
import Skeleton from '../components/Skeleton'

export default function MyOrdersPage() {
  const [orders, setOrders] = useState<any[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetchOrders()
  }, [])

  const fetchOrders = async () => {
    try {
      const data = await ordersApi.getMyOrders()
      setOrders(data || [])
    } catch (err) {
      console.error('Siparişler yüklenemedi', err)
    } finally {
      setLoading(false)
    }
  }

  const handleDownloadInvoice = async (orderId: number) => {
    try {
      await ordersApi.downloadInvoice(orderId)
    } catch (err) {
      alert('Fatura indirilemedi.')
    }
  }

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'Completed': return <CheckCircle className="w-5 h-5 text-green-500" />
      case 'Shipped': return <Truck className="w-5 h-5 text-blue-500" />
      case 'Processing': return <Clock className="w-5 h-5 text-amber-500" />
      case 'Cancelled': return <AlertCircle className="w-5 h-5 text-red-500" />
      default: return <Package className="w-5 h-5 text-gray-500" />
    }
  }

  const getStatusText = (status: string) => {
    switch (status) {
      case 'Pending': return 'Beklemede'
      case 'Processing': return 'Hazırlanıyor'
      case 'Shipped': return 'Kargoda'
      case 'Completed': return 'Tamamlandı'
      case 'Cancelled': return 'İptal Edildi'
      case 'Refunded': return 'İade Edildi'
      default: return status
    }
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <PageMeta title="Siparişlerim | HediyeJoy" />
      <h1 className="text-3xl font-bold mb-8">Siparişlerim</h1>

      {loading ? (
        <div className="space-y-4">
          {[1, 2, 3].map(i => <Skeleton key={i} className="h-32 w-full" />)}
        </div>
      ) : orders.length === 0 ? (
        <div className="text-center py-12 card bg-gray-50">
          <Package className="w-16 h-16 mx-auto text-gray-300 mb-4" />
          <p className="text-gray-500">Henüz bir siparişiniz bulunmuyor.</p>
        </div>
      ) : (
        <div className="space-y-6">
          {orders.map((order) => (
            <div key={order.id} className="card overflow-hidden">
              <div className="bg-gray-50 p-4 border-b flex flex-wrap justify-between items-center gap-4">
                <div className="flex gap-6 text-sm">
                  <div>
                    <div className="text-gray-500 uppercase text-[10px] font-bold">Sipariş Tarihi</div>
                    <div>{new Date(order.createdAt).toLocaleDateString('tr-TR')}</div>
                  </div>
                  <div>
                    <div className="text-gray-500 uppercase text-[10px] font-bold">Toplam</div>
                    <div className="font-bold text-primary-600">₺{order.totalAmount.toFixed(2)}</div>
                  </div>
                  <div>
                    <div className="text-gray-500 uppercase text-[10px] font-bold">Sipariş No</div>
                    <div>#{order.orderNumber}</div>
                  </div>
                </div>
                <div className="flex items-center gap-3">
                  <div className={`flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-medium ${
                    order.status === 'Completed' ? 'bg-green-100 text-green-700' :
                    order.status === 'Shipped' ? 'bg-blue-100 text-blue-700' :
                    order.status === 'Processing' ? 'bg-amber-100 text-amber-700' :
                    'bg-gray-100 text-gray-700'
                  }`}>
                    {getStatusIcon(order.status)}
                    {getStatusText(order.status)}
                  </div>
                  <button
                    onClick={() => handleDownloadInvoice(order.id)}
                    className="btn btn-outline p-2 rounded-lg"
                    title="Faturayı İndir"
                  >
                    <Download className="w-4 h-4" />
                  </button>
                </div>
              </div>

              <div className="p-4">
                <div className="divide-y">
                  {order.items?.map((item: any) => (
                    <div key={item.id} className="py-4 flex gap-4">
                      <img
                        src={item.productImageUrl}
                        alt={item.productName}
                        className="w-20 h-20 object-cover rounded-lg"
                        onError={(e) => { (e.target as HTMLImageElement).src = '/fallback-product.svg' }}
                      />
                      <div className="flex-1">
                        <div className="font-semibold">{item.productName}</div>
                        <div className="text-sm text-gray-500">{item.quantity} adet x ₺{item.unitPrice.toFixed(2)}</div>
                      </div>
                    </div>
                  ))}
                </div>

                {order.trackingNumber && (
                  <div className="mt-4 p-3 bg-blue-50 border border-blue-100 rounded-lg flex items-center gap-3">
                    <Truck className="w-5 h-5 text-blue-600" />
                    <div className="text-sm">
                      <span className="font-semibold text-blue-800">Takip Numarası:</span> {order.trackingNumber}
                      {order.shippingProvider && <span className="ml-1">({order.shippingProvider})</span>}
                    </div>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
