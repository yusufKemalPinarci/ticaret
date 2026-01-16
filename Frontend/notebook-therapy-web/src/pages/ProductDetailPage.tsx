import { useEffect, useMemo, useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useDispatch, useSelector } from 'react-redux'
import { AppDispatch, RootState } from '../store/store'
import { fetchProductById, fetchProducts } from '../store/slices/productSlice'
import { addToCart } from '../store/slices/cartSlice'
import { ShoppingCart } from 'lucide-react'
import PageMeta from '../components/PageMeta'
import Skeleton from '../components/Skeleton'

export default function ProductDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const dispatch = useDispatch<AppDispatch>()
  const { currentProduct, isLoading, products } = useSelector((state: RootState) => state.products)
  const [quantity, setQuantity] = useState(1)

  useEffect(() => {
    if (id) {
      dispatch(fetchProductById(parseInt(id)))
    }
    if (!products || products.length === 0) {
      dispatch(fetchProducts())
    }
  }, [dispatch, id])

  const related = useMemo(() => {
    if (!currentProduct || !products) return []
    return products
      .filter((p) => p.id !== currentProduct.id && (p.categoryId === currentProduct.categoryId || p.collection === currentProduct.collection))
      .slice(0, 4)
  }, [products, currentProduct])

  const handleAddToCart = () => {
    if (currentProduct) {
      dispatch(addToCart({ productId: currentProduct.id, quantity }))
      navigate('/cart')
    }
  }

  if (isLoading) {
    return (
      <div className="container mx-auto px-4 py-12">
        <PageMeta title="Ürün detayları | HediyeJoy" />
        <div className="grid grid-cols-1 md:grid-cols-2 gap-8" aria-busy="true">
          <Skeleton className="h-80" />
          <div className="space-y-4">
            <Skeleton className="h-8 w-2/3" />
            <Skeleton className="h-6 w-1/3" />
            <Skeleton className="h-4 w-full" />
            <Skeleton className="h-10 w-full" />
          </div>
        </div>
      </div>
    )
  }

  if (!currentProduct) {
    return <div className="container mx-auto px-4 py-12 text-center">Ürün bulunamadı.</div>
  }

  const displayPrice = currentProduct.discountPrice || currentProduct.price
  const originalPrice = currentProduct.discountPrice ? currentProduct.price : null

  const heroSrcSet = [600, 900, 1200, 1600]
    .map((w) => `${currentProduct.imageUrl}?w=${w} ${w}w`)
    .join(', ')

  return (
    <div className="container mx-auto px-4 py-8">
      <PageMeta title={`${currentProduct.name} | HediyeJoy`} description={currentProduct.description?.slice(0, 150)} />
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        <div>
          <img
            src={currentProduct.imageUrl}
            srcSet={heroSrcSet}
            sizes="(min-width: 768px) 50vw, 100vw"
            loading="lazy"
            decoding="async"
            onError={(e) => { (e.target as HTMLImageElement).src = '/fallback-product.svg' }}
            alt={currentProduct.name}
            className="w-full rounded-lg shadow-lg"
          />
        </div>
        <div>
          <h1 className="text-4xl font-bold mb-4">{currentProduct.name}</h1>
          <div className="mb-4">
            <div className="flex items-center space-x-2 mb-2">
              <span className="text-3xl font-bold text-primary-600">
                ${displayPrice.toFixed(2)}
              </span>
              {originalPrice && (
                <span className="text-xl text-gray-500 line-through">
                  ${originalPrice.toFixed(2)}
                </span>
              )}
            </div>
            {currentProduct.collection && (
              <span className="text-sm text-gray-600">Koleksiyon: {currentProduct.collection}</span>
            )}
          </div>

          <p className="text-gray-700 mb-6">{currentProduct.description}</p>

          <div className="mb-6">
            <label className="block mb-2 font-semibold">Adet:</label>
            <div className="flex items-center space-x-4">
              <button
                onClick={() => setQuantity(Math.max(1, quantity - 1))}
                className="w-10 h-10 border rounded-lg hover:bg-gray-100"
              >
                -
              </button>
              <span className="text-xl font-semibold">{quantity}</span>
              <button
                onClick={() => setQuantity(Math.min(currentProduct.stock, quantity + 1))}
                className="w-10 h-10 border rounded-lg hover:bg-gray-100"
              >
                +
              </button>
              <span className="text-gray-600">Stok: {currentProduct.stock}</span>
            </div>
          </div>

          <button
            onClick={handleAddToCart}
            disabled={currentProduct.stock === 0}
            className="btn btn-primary w-full flex items-center justify-center space-x-2 text-lg py-3 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <ShoppingCart className="w-5 h-5" />
            <span>Sepete Ekle</span>
          </button>

          {currentProduct.stock === 0 && (
            <p className="text-red-600 mt-2 text-center">Ürün stokta yok</p>
          )}
        </div>
      </div>

      {related.length > 0 && (
        <div className="mt-12">
          <h2 className="text-2xl font-bold mb-4">İlgili Ürünler</h2>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
            {related.map((p) => (
              <div key={p.id} className="card">
                <img
                  src={p.imageUrl}
                  srcSet={[320, 480, 640].map((w) => `${p.imageUrl}?w=${w} ${w}w`).join(', ')}
                  sizes="(min-width:1024px) 25vw, (min-width:640px) 33vw, 100vw"
                  alt={p.name}
                  className="w-full h-40 object-cover rounded-t"
                  loading="lazy"
                  decoding="async"
                />
                <div className="p-3">
                  <div className="font-semibold line-clamp-1">{p.name}</div>
                  <div className="text-sm text-gray-600">₺{(p.discountPrice || p.price).toFixed(2)}</div>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}
