import { useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { Link } from 'react-router-dom'
import { AppDispatch, RootState } from '../store/store'
import { fetchCart, removeFromCart, clearCart, updateCartItem } from '../store/slices/cartSlice'
import { Trash2, ShoppingBag } from 'lucide-react'

export default function CartPage() {
  const dispatch = useDispatch<AppDispatch>()
  const { items, totalAmount, itemCount, isLoading } = useSelector(
    (state: RootState) => state.cart
  )

  useEffect(() => {
    dispatch(fetchCart())
  }, [dispatch])

  const handleRemoveItem = (cartItemId: number) => {
    dispatch(removeFromCart(cartItemId))
  }

  const handleUpdateQuantity = (cartItemId: number, quantity: number) => {
    if (quantity < 1) return
    dispatch(updateCartItem({ cartItemId, quantity }))
  }

  const handleClearCart = () => {
    if (window.confirm('Sepeti temizlemek istediğinize emin misiniz?')) {
      dispatch(clearCart())
    }
  }

  if (isLoading) {
    return <div className="container mx-auto px-4 py-12 text-center">Yükleniyor...</div>
  }

  if (items.length === 0) {
    return (
      <div className="container mx-auto px-4 py-12 text-center">
        <ShoppingBag className="w-24 h-24 mx-auto text-gray-300 mb-4" />
        <h2 className="text-2xl font-bold mb-4">Sepetiniz boş</h2>
        <Link to="/products" className="btn btn-primary">
          Alışverişe Başla
        </Link>
      </div>
    )
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-4xl font-bold">Sepetim</h1>
        <button onClick={handleClearCart} className="text-red-600 hover:text-red-700">
          Sepeti Temizle
        </button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div className="lg:col-span-2 space-y-4">
          {items.map((item) => (
            <div key={item.id} className="card p-4 flex items-center space-x-4">
              <img
                src={item.productImageUrl}
                onError={(e) => { (e.target as HTMLImageElement).src = '/fallback-product.svg' }}
                alt={item.productName}
                className="w-24 h-24 object-cover rounded-lg"
              />
              <div className="flex-grow">
                <h3 className="font-semibold text-lg">{item.productName}</h3>
                <div className="flex items-center space-x-4 mt-2">
                  <div className="flex items-center border rounded-lg overflow-hidden">
                    <button
                      onClick={() => handleUpdateQuantity(item.id, Math.max(1, item.quantity - 1))}
                      className="px-3 py-1 hover:bg-gray-100"
                    >
                      -
                    </button>
                    <div className="px-4 py-1">{item.quantity}</div>
                    <button
                      onClick={() => handleUpdateQuantity(item.id, item.quantity + 1)}
                      className="px-3 py-1 hover:bg-gray-100"
                    >
                      +
                    </button>
                  </div>
                  <p className="text-primary-600 font-bold">${item.totalPrice.toFixed(2)}</p>
                </div>
              </div>
              <button
                onClick={() => handleRemoveItem(item.id)}
                className="text-red-600 hover:text-red-700 p-2"
              >
                <Trash2 className="w-5 h-5" />
              </button>
            </div>
          ))}
        </div>

        <div className="lg:col-span-1">
          <div className="card p-6 sticky top-20">
            <h2 className="text-2xl font-bold mb-4">Sipariş Özeti</h2>
            <div className="space-y-2 mb-4">
              <div className="flex justify-between">
                <span>Toplam Ürün:</span>
                <span>{itemCount} adet</span>
              </div>
              <div className="flex justify-between">
                <span>Ara Toplam:</span>
                <span>${totalAmount.toFixed(2)}</span>
              </div>
              <div className="flex justify-between">
                <span>Kargo:</span>
                <span className="text-green-600">Ücretsiz</span>
              </div>
              <div className="border-t pt-2 mt-2">
                <div className="flex justify-between text-xl font-bold">
                  <span>Toplam:</span>
                  <span>${totalAmount.toFixed(2)}</span>
                </div>
              </div>
            </div>
            <Link to="/checkout" className="btn btn-primary w-full text-lg py-3 text-center inline-block">
              Ödemeye Geç
            </Link>
          </div>
        </div>
      </div>
    </div>
  )
}
