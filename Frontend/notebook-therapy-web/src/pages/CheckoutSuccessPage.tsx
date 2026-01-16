import { Link, useSearchParams } from 'react-router-dom'

export default function CheckoutSuccessPage() {
  const [params] = useSearchParams()
  const orderId = params.get('orderId')
  const amount = params.get('amount')

  return (
    <div className="container mx-auto px-4 py-12 text-center">
      <h1 className="text-3xl font-bold mb-3">Ödeme başarılı!</h1>
      <p className="text-gray-600 mb-6">Siparişiniz alındı. {orderId && <>Sipariş no: <strong>{orderId}</strong>.</>}</p>
      {amount && <p className="text-gray-700 mb-4">Tutar: {amount}</p>}
      <div className="space-x-3">
        <Link to="/orders" className="btn btn-primary">Siparişlerim</Link>
        <Link to="/products" className="btn btn-outline">Alışverişe devam et</Link>
      </div>
    </div>
  )
}
