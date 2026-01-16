import { useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { AppDispatch, RootState } from '../store/store'
import { fetchFeaturedProducts, fetchNewProducts, fetchBackInStockProducts } from '../store/slices/productSlice'
import ProductCard from '../components/ProductCard'
import { Link } from 'react-router-dom'
import PageMeta from '../components/PageMeta'
import Skeleton from '../components/Skeleton'

export default function HomePage() {
  const dispatch = useDispatch<AppDispatch>()
  const { featuredProducts, newProducts, backInStockProducts, isLoading } = useSelector(
    (state: RootState) => state.products
  )

  useEffect(() => {
    dispatch(fetchFeaturedProducts())
    dispatch(fetchNewProducts())
    dispatch(fetchBackInStockProducts())
  }, [dispatch])

  return (
    <div>
      <PageMeta title="HediyeJoy | Hediyelik, kişiye özel ve kutu hediyeler" description="Kupa & termos, mum & dekor, kişiye özel ve özel gün hediyeleri. Hızlı kargo, kolay iade." />
      {/* Hero Section */}
      <section className="bg-gradient-to-r from-primary-50 via-white to-primary-100 py-20">
        <div className="container mx-auto px-4 text-center">
          <h1 className="text-5xl font-bold mb-4">
            Her güne uygun hediye fikirleri
          </h1>
          <p className="text-xl text-gray-600 mb-8">
            Kişiye özel kupalar, mumlar, ofis setleri ve özel gün kutuları
          </p>
          <Link to="/products" className="btn btn-primary text-lg px-8 py-3">
            Hediyeleri keşfet
          </Link>
        </div>
      </section>

      {/* Featured Products */}
      <section className="py-16">
        <div className="container mx-auto px-4">
          <h2 className="text-3xl font-bold mb-8">Öne Çıkan Hediyeler</h2>
          {isLoading ? (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6" aria-busy="true">
              {Array.from({ length: 4 }).map((_, i) => <Skeleton key={i} className="h-72" />)}
            </div>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
              {featuredProducts.slice(0, 8).map((product) => (
                <ProductCard key={product.id} product={product} />
              ))}
            </div>
          )}
        </div>
      </section>

      {/* Back in Stock */}
      <section className="py-16 bg-gray-50">
        <div className="container mx-auto px-4">
          <h2 className="text-3xl font-bold mb-8">Tekrar Stokta</h2>
          {isLoading ? (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6" aria-busy="true">
              {Array.from({ length: 4 }).map((_, i) => <Skeleton key={i} className="h-72" />)}
            </div>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
              {backInStockProducts.slice(0, 8).map((product) => (
                <ProductCard key={product.id} product={product} />
              ))}
            </div>
          )}
        </div>
      </section>

      {/* New Products */}
      <section className="py-16">
        <div className="container mx-auto px-4">
          <h2 className="text-3xl font-bold mb-8">Yeni Gelenler</h2>
          {isLoading ? (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6" aria-busy="true">
              {Array.from({ length: 4 }).map((_, i) => <Skeleton key={i} className="h-72" />)}
            </div>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
              {newProducts.slice(0, 8).map((product) => (
                <ProductCard key={product.id} product={product} />
              ))}
            </div>
          )}
        </div>
      </section>

      {/* Features */}
      <section className="py-16 bg-primary-600 text-white">
        <div className="container mx-auto px-4">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8 text-center">
            <div>
              <h3 className="text-xl font-semibold mb-2">Hızlı & güvenli kargo</h3>
              <p className="text-primary-100">Özenli paketleme, takipli gönderim</p>
            </div>
            <div>
              <h3 className="text-xl font-semibold mb-2">30 gün kolay iade</h3>
              <p className="text-primary-100">Hediyeniz uygun değilse risksiz iade</p>
            </div>
            <div>
              <h3 className="text-xl font-semibold mb-2">Mutlu eden seçimler</h3>
              <p className="text-primary-100">Binlerce gönderide yüksek memnuniyet</p>
            </div>
          </div>
        </div>
      </section>
    </div>
  )
}
