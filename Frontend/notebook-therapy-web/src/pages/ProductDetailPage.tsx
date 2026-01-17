import { useEffect, useMemo, useState } from 'react'
import { useParams, useNavigate, Link } from 'react-router-dom'
import { useDispatch, useSelector } from 'react-redux'
import { AppDispatch, RootState } from '../store/store'
import { fetchProductById, fetchProducts } from '../store/slices/productSlice'
import { addToCart } from '../store/slices/cartSlice'
import { ShoppingCart, Star, MessageSquare, Heart } from 'lucide-react'
import PageMeta from '../components/PageMeta'
import Skeleton from '../components/Skeleton'
import { reviewApi } from '../services/api'
import { publishNotice } from '../services/notifications'
import { addToWishlist, removeFromWishlist } from '../store/slices/wishlistSlice'

export default function ProductDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const dispatch = useDispatch<AppDispatch>()
  const { currentProduct, isLoading, products } = useSelector((state: RootState) => state.products)
  const { user } = useSelector((state: RootState) => state.auth)
  const isWishlisted = useSelector((state: RootState) =>
    state.wishlist.items.some(item => item.productId === (currentProduct?.id || 0))
  )
  const [quantity, setQuantity] = useState(1)
  const [reviews, setReviews] = useState<any[]>([])
  const [isReviewsLoading, setIsReviewsLoading] = useState(true)
  const [newReviewRating, setNewReviewRating] = useState(5)
  const [newReviewComment, setNewReviewComment] = useState('')
  const [isSubmittingReview, setIsSubmittingReview] = useState(false)

  useEffect(() => {
    if (id) {
      dispatch(fetchProductById(parseInt(id)))
      loadReviews(parseInt(id))
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

  const loadReviews = async (productId: number) => {
    setIsReviewsLoading(true)
    try {
      const data = await reviewApi.getByProduct(productId)
      setReviews(data)
    } catch (err) {
      // ignore
    } finally {
      setIsReviewsLoading(false)
    }
  }

  const handleAddToCart = () => {
    if (currentProduct) {
      dispatch(addToCart({ productId: currentProduct.id, quantity }))
      navigate('/cart')
    }
  }

  const handleWishlist = async () => {
    if (!user || !currentProduct) {
      publishNotice({ kind: 'warning', message: 'İstek listesi için giriş yapmalısınız.' })
      return
    }
    try {
      if (isWishlisted) {
        await dispatch(removeFromWishlist(currentProduct.id)).unwrap()
        publishNotice({ kind: 'success', message: 'İstek listesinden çıkarıldı.' })
      } else {
        await dispatch(addToWishlist(currentProduct.id)).unwrap()
        publishNotice({ kind: 'success', message: 'İstek listesine eklendi.' })
      }
    } catch (err) {
      publishNotice({ kind: 'warning', message: 'İşlem başarısız oldu.' })
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

          <div className="flex gap-4">
            <button
              onClick={handleAddToCart}
              disabled={currentProduct.stock === 0}
              className="btn btn-primary flex-1 flex items-center justify-center space-x-2 text-lg py-3 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <ShoppingCart className="w-5 h-5" />
              <span>Sepete Ekle</span>
            </button>
            <button
              onClick={handleWishlist}
              className={`btn btn-outline p-3 rounded-lg border-2 transition-colors ${
                isWishlisted ? 'border-red-500 text-red-500' : 'border-gray-200 text-gray-400'
              }`}
              title="İstek listesine ekle"
            >
              <Heart className={`w-6 h-6 ${isWishlisted ? 'fill-current' : ''}`} />
            </button>
          </div>

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

      {/* Reviews Section */}
      <div className="mt-16 border-t pt-12">
        <h2 className="text-3xl font-bold mb-8 flex items-center gap-2">
          <MessageSquare className="w-8 h-8 text-primary-600" />
          Müşteri Değerlendirmeleri
        </h2>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-12">
          <div className="lg:col-span-1">
            {user ? (
              <div className="card p-6 sticky top-24">
                <h3 className="text-xl font-bold mb-4">Ürünü Değerlendir</h3>
                <form
                  onSubmit={async (e) => {
                    e.preventDefault()
                    if (!currentProduct) return
                    setIsSubmittingReview(true)
                    try {
                      const review = await reviewApi.create({
                        productId: currentProduct.id,
                        rating: newReviewRating,
                        comment: newReviewComment,
                      })
                      setReviews([review, ...reviews])
                      setNewReviewComment('')
                      setNewReviewRating(5)
                      publishNotice({ kind: 'success', message: 'Yorumunuz için teşekkürler!' })
                    } catch (err) {
                      publishNotice({ kind: 'warning', message: 'Yorum gönderilemedi.' })
                    } finally {
                      setIsSubmittingReview(false)
                    }
                  }}
                  className="space-y-4"
                >
                  <div>
                    <label className="block text-sm font-medium mb-1">Puanınız</label>
                    <div className="flex gap-1">
                      {[1, 2, 3, 4, 5].map((star) => (
                        <button
                          key={star}
                          type="button"
                          onClick={() => setNewReviewRating(star)}
                          className="focus:outline-none"
                        >
                          <Star
                            className={`w-8 h-8 ${
                              star <= newReviewRating ? 'text-yellow-400 fill-current' : 'text-gray-300'
                            }`}
                          />
                        </button>
                      ))}
                    </div>
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-1">Yorumunuz</label>
                    <textarea
                      className="input min-h-[100px]"
                      value={newReviewComment}
                      onChange={(e) => setNewReviewComment(e.target.value)}
                      placeholder="Ürün hakkındaki düşüncelerinizi paylaşın..."
                      required
                    />
                  </div>
                  <button type="submit" className="btn btn-primary w-full" disabled={isSubmittingReview}>
                    {isSubmittingReview ? 'Gönderiliyor...' : 'Yorumu Gönder'}
                  </button>
                </form>
              </div>
            ) : (
              <div className="card p-6 bg-gray-50 text-center sticky top-24">
                <p className="text-gray-600 mb-4">Yorum yapabilmek için giriş yapmalısınız.</p>
                <Link to="/login" className="btn btn-outline w-full">Giriş Yap</Link>
              </div>
            )}
          </div>

          <div className="lg:col-span-2">
            {isReviewsLoading ? (
              <div className="space-y-4">
                <Skeleton className="h-32 w-full" />
                <Skeleton className="h-32 w-full" />
              </div>
            ) : reviews.length === 0 ? (
              <div className="text-center py-12 bg-gray-50 rounded-lg">
                <p className="text-gray-500">Henüz yorum yapılmamış. İlk yorumu siz yapın!</p>
              </div>
            ) : (
              <div className="space-y-6">
                {reviews.map((review) => (
                  <div key={review.id} className="border-b pb-6 last:border-0">
                    <div className="flex justify-between items-start mb-2">
                      <div>
                        <div className="font-bold text-lg">{review.userFullName}</div>
                        <div className="flex gap-0.5 text-yellow-400 my-1">
                          {Array.from({ length: 5 }).map((_, i) => (
                            <Star
                              key={i}
                              className={`w-4 h-4 ${i < review.rating ? 'fill-current' : 'text-gray-200'}`}
                            />
                          ))}
                        </div>
                      </div>
                      <span className="text-sm text-gray-500">
                        {new Date(review.createdAt).toLocaleDateString('tr-TR')}
                      </span>
                    </div>
                    <p className="text-gray-700 leading-relaxed">{review.comment}</p>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
