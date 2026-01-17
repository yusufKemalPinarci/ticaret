import { Link } from 'react-router-dom'
import { Product } from '../store/slices/productSlice'
import { useDispatch } from 'react-redux'
import { AppDispatch } from '../store/store'
import { addToCart } from '../store/slices/cartSlice'

interface ProductCardProps {
  product: Product
}

export default function ProductCard({ product }: ProductCardProps) {
  const dispatch = useDispatch<AppDispatch>()
  const displayPrice = product.discountPrice || product.price
  const originalPrice = product.discountPrice ? product.price : null
  const srcSet = [360, 540, 720, 960]
    .map((w) => `${product.imageUrl}?w=${w} ${w}w`)
    .join(', ')

  return (
    <Link to={`/products/${product.id}`} className="card group relative">
      <div className="relative overflow-hidden rounded-t-2xl">
        <img
          src={product.imageUrl}
          srcSet={srcSet}
          loading="lazy"
          sizes="(min-width:1024px) 25vw, (min-width:640px) 50vw, 100vw"
          alt={product.name}
          decoding="async"
          onError={(e) => { (e.target as HTMLImageElement).src = '/fallback-product.svg' }}
          className="w-full h-64 object-cover group-hover:scale-105 transition-transform duration-300 rounded-t-2xl"
        />
        {product.isNew && (
          <span className="absolute top-2 left-2 bg-green-500 text-white px-2 py-1 text-xs rounded">
            Yeni
          </span>
        )}
        {product.isBackInStock && (
          <span className="absolute top-2 left-2 bg-blue-500 text-white px-2 py-1 text-xs rounded">
            Tekrar Stokta!
          </span>
        )}
      </div>
      <div className="p-4">
        <h3 className="font-semibold text-lg mb-2 line-clamp-2 group-hover:text-primary-600 transition-colors">
          {product.name}
        </h3>
        <div className="flex items-center space-x-1 text-amber-500 text-xs mb-1" aria-label="Puanlama placeholder">
          {'★★★★★'}
          <span className="text-gray-400 text-[11px] ml-1">(yakında)</span>
        </div>
        <div className="flex items-center space-x-2">
          <span className="text-xl font-bold text-primary-600">
            ₺{displayPrice.toFixed(2)}
          </span>
          {originalPrice && (
            <span className="text-sm text-gray-500 line-through">
              ₺{originalPrice.toFixed(2)}
            </span>
          )}
        </div>
        {product.collection && (
          <span className="text-xs text-gray-500 mt-1 block">
            {product.collection}
          </span>
        )}
      </div>
      <div className="absolute right-3 bottom-3">
        <button
          onClick={(e) => {
            e.preventDefault()
            e.stopPropagation()
            dispatch(addToCart({ productId: product.id, quantity: 1 }))
          }}
          className="btn btn-primary px-3 py-2 text-sm shadow"
          aria-label={`Add ${product.name} to cart`}
        >
          Sepete Ekle
        </button>
      </div>
    </Link>
  )
}
