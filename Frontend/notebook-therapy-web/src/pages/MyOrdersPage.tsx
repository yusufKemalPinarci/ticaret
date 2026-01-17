import { useState, useEffect } from 'react'
import { ordersApi } from '../services/api'
import PageMeta from '../components/PageMeta'
import Skeleton from '../components/Skeleton'
import { Package } from 'lucide-react'
import { Link } from 'react-router-dom'

export default function MyOrdersPage() {
  const [orders, setOrders] = useState<any[]>([])
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    ordersApi.getMyOrders()
      .then(data => setOrders(data))
      .catch(() => {})
      .finally(() => setIsLoading(false))
  }, [])

  return (
    <div className="container mx-auto px-4 py-8 max-w-4xl">
      <PageMeta title="Siparişlerim | HediyeJoy" />
      <h1 className="text-3xl font-bold mb-6">Siparişlerim</h1>

      {isLoading ? (
        <div className="space-y-4">
          <Skeleton className="h-24 w-full" />
          <Skeleton className="h-24 w-full" />
          <Skeleton className="h-24 w-full" />
        </div>
      ) : orders.length === 0 ? (
        <div className="text-center py-12 card">
          <Package className="w-16 h-16 mx-auto text-gray-300 mb-4" />
          <p className="text-gray-500 text-lg">Henüz bir siparişiniz bulunmuyor.</p>
          <Link to="/products" className="btn btn-primary mt-4 inline-block">Alışverişe Başla</Link>
        </div>
      ) : (
        <div className="space-y-4">
          {orders.map((order) => (
            <div key={order.id} className="card p-4 flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
              <div>
                <div className="font-bold text-lg">Sipariş #{order.orderNumber}</div>
                <div className="text-sm text-gray-500">
                  {new Date(order.createdAt).toLocaleDateString('tr-TR')}
                </div>
              </div>
              <div className="flex flex-col items-end">
                <div className="font-bold text-primary-600">${order.totalAmount.toFixed(2)}</div>
                <div className={`text-sm px-2 py-1 rounded ${
                    order.status === 'Completed' ? 'bg-green-100 text-green-800' :
                    order.status === 'Shipped' ? 'bg-blue-100 text-blue-800' :
                    order.status === 'Cancelled' ? 'bg-red-100 text-red-800' :
                    'bg-yellow-100 text-yellow-800'
                }`}>
                  {order.status}
                </div>
              </div>
              {/* <Link to={`/orders/${order.id}`} className="btn btn-outline p-2">
                <ChevronRight className="w-5 h-5" />
              </Link> */}
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
