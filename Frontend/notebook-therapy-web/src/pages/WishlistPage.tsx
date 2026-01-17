import PageMeta from '../components/PageMeta'
import Skeleton from '../components/Skeleton'
import { Heart, Trash2, ShoppingCart } from 'lucide-react'
import { Link } from 'react-router-dom'
import { useSelector } from 'react-redux'
import { useAppDispatch } from '../store/hooks'
import { RootState } from '../store/store'
import { addToCart } from '../store/slices/cartSlice'
import { removeFromWishlist } from '../store/slices/wishlistSlice'
import { publishNotice } from '../services/notifications'

export default function WishlistPage() {
  const { items, loading: isLoading } = useSelector((state: RootState) => state.wishlist)
  const dispatch = useAppDispatch()

  const handleRemove = async (productId: number) => {
    try {
      await dispatch(removeFromWishlist(productId)).unwrap()
      publishNotice({ kind: 'success', message: 'Ürün istek listesinden çıkarıldı.' })
    } catch (err) {
      publishNotice({ kind: 'warning', message: 'Ürün çıkarılamadı.' })
    }
  }

  const handleAddToCart = (item: any) => {
    dispatch(addToCart({ productId: item.productId, quantity: 1 }))
    publishNotice({ kind: 'success', message: 'Ürün sepete eklendi.' })
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <PageMeta title="İstek Listem | HediyeJoy" />
      <h1 className="text-3xl font-bold mb-6 flex items-center gap-2">
        <Heart className="text-red-500 fill-current" />
        İstek Listem
      </h1>

      {isLoading ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          {Array.from({ length: 4 }).map((_, i) => <Skeleton key={i} className="h-72" />)}
        </div>
      ) : items.length === 0 ? (
        <div className="text-center py-12 card">
          <Heart className="w-16 h-16 mx-auto text-gray-200 mb-4" />
          <p className="text-gray-500 text-lg">İstek listeniz henüz boş.</p>
          <Link to="/products" className="btn btn-primary mt-4 inline-block">Ürünleri Keşfet</Link>
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          {items.map((item) => (
            <div key={item.id} className="card group relative">
              <Link to={`/products/${item.productId}`}>
                <img
                  src={item.productImageUrl}
                  alt={item.productName}
                  className="w-full h-64 object-cover rounded-t-lg"
                />
              </Link>
              <button
                onClick={() => handleRemove(item.productId)}
                className="absolute top-2 right-2 p-2 bg-white rounded-full shadow-md text-gray-400 hover:text-red-500 transition-colors"
                title="Listeden çıkar"
              >
                <Trash2 className="w-5 h-5" />
              </button>
              <div className="p-4">
                <Link to={`/products/${item.productId}`}>
                  <h3 className="font-semibold text-lg line-clamp-1 hover:text-primary-600 transition-colors">
                    {item.productName}
                  </h3>
                </Link>
                <div className="flex items-center gap-2 mt-1 mb-4">
                  <span className="text-xl font-bold text-primary-600">
                    ${(item.productDiscountPrice || item.productPrice).toFixed(2)}
                  </span>
                  {item.productDiscountPrice && (
                    <span className="text-sm text-gray-400 line-through">
                      ${item.productPrice.toFixed(2)}
                    </span>
                  )}
                </div>
                <button
                  onClick={() => handleAddToCart(item)}
                  className="btn btn-primary w-full flex items-center justify-center gap-2"
                >
                  <ShoppingCart className="w-4 h-4" />
                  Sepete Ekle
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
