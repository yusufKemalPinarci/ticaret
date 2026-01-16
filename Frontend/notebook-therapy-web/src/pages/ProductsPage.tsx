import { useEffect, useMemo, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { useSearchParams } from 'react-router-dom'
import { AppDispatch, RootState } from '../store/store'
import { fetchProducts, searchProducts } from '../store/slices/productSlice'
import ProductCard from '../components/ProductCard'
import { Search } from 'lucide-react'
import { categoryApi } from '../services/api'
import PageMeta from '../components/PageMeta'
import Skeleton from '../components/Skeleton'

export default function ProductsPage() {
  const dispatch = useDispatch<AppDispatch>()
  const [searchParams, setSearchParams] = useSearchParams()
  const [searchQuery, setSearchQuery] = useState('')
  const [sort, setSort] = useState('popularity')
  const [selectedCategory, setSelectedCategory] = useState<number | null>(null)
  const [page, setPage] = useState(1)
  const [pageSize] = useState(12)
  const [minPrice, setMinPrice] = useState<number | null>(null)
  const [maxPrice, setMaxPrice] = useState<number | null>(null)
  const [inStockOnly, setInStockOnly] = useState(false)
  const [categories, setCategories] = useState<{ id: number; name: string }[]>([])
  
  const { products, isLoading } = useSelector((state: RootState) => state.products)
  const collection = searchParams.get('collection')
  const categoryId = searchParams.get('categoryId')

  useEffect(() => {
    dispatch(fetchProducts())
    categoryApi.getAll().then((data) => setCategories(data || [])).catch(() => setCategories([]))
  }, [dispatch])

  useEffect(() => {
    if (categoryId) setSelectedCategory(Number(categoryId))
    if (searchParams.get('q')) setSearchQuery(searchParams.get('q') || '')
    if (searchParams.get('sort')) setSort(searchParams.get('sort') || 'popularity')
    if (searchParams.get('page')) setPage(Number(searchParams.get('page')) || 1)
    if (searchParams.get('minPrice')) setMinPrice(Number(searchParams.get('minPrice')))
    if (searchParams.get('maxPrice')) setMaxPrice(Number(searchParams.get('maxPrice')))
    if (searchParams.get('inStock') === '1') setInStockOnly(true)
  }, [categoryId, searchParams])

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    const params = new URLSearchParams(searchParams)
    if (searchQuery.trim()) {
      dispatch(searchProducts(searchQuery))
      params.set('q', searchQuery.trim())
    } else {
      dispatch(fetchProducts())
      params.delete('q')
    }
    params.set('page', '1')
    setPage(1)
    setSearchParams(params)
  }

  const filtered = useMemo(() => {
    let list = [...products]
    if (collection) {
      list = list.filter((p) => p.collection?.toLowerCase() === collection.toLowerCase())
    }
    if (selectedCategory) {
      list = list.filter((p) => p.categoryId === selectedCategory)
    }
    if (minPrice !== null) {
      list = list.filter((p) => (p.discountPrice || p.price) >= minPrice)
    }
    if (maxPrice !== null) {
      list = list.filter((p) => (p.discountPrice || p.price) <= maxPrice)
    }
    if (inStockOnly) {
      list = list.filter((p) => (p.stock ?? 0) > 0)
    }

    switch (sort) {
      case 'price-asc':
        list.sort((a, b) => (a.discountPrice || a.price) - (b.discountPrice || b.price))
        break
      case 'price-desc':
        list.sort((a, b) => (b.discountPrice || b.price) - (a.discountPrice || a.price))
        break
      case 'newest':
        list = list.sort((a, b) => (b.id || 0) - (a.id || 0))
        break
      default:
        break
    }
    return list
  }, [products, collection, selectedCategory, sort])

  const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize))
  const safePage = Math.min(page, totalPages)
  const paged = filtered.slice((safePage - 1) * pageSize, safePage * pageSize)

  return (
    <div className="container mx-auto px-4 py-8">
      <PageMeta title="Ürünler | HediyeJoy" description="HediyeJoy ürün kataloğu: kupa & termos, mum & dekor, kişiye özel ve özel gün hediyeleri." />
      <div className="mb-8">
        <h1 className="text-4xl font-bold mb-4">Ürünler</h1>
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-4">
          <form onSubmit={handleSearch} className="flex gap-2 lg:col-span-2">
            <div className="flex-1 relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="Ürün ara..."
                className="input pl-10"
              />
            </div>
            <button type="submit" className="btn btn-primary">
              Ara
            </button>
          </form>
          <select
            className="input"
            value={selectedCategory ?? ''}
            onChange={(e) => {
              const val = e.target.value ? Number(e.target.value) : null
              setSelectedCategory(val)
              setPage(1)
              const params = new URLSearchParams(searchParams)
              if (val) params.set('categoryId', String(val))
              else params.delete('categoryId')
              params.set('page', '1')
              setSearchParams(params)
            }}
          >
            <option value="">Tüm Kategoriler</option>
            {categories.map((c) => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
          <select
            className="input"
            value={sort}
            onChange={(e) => {
              const val = e.target.value
              setSort(val)
              const params = new URLSearchParams(searchParams)
              params.set('sort', val)
              setSearchParams(params)
            }}
          >
            <option value="popularity">Öne çıkan</option>
            <option value="price-asc">Fiyat (artan)</option>
            <option value="price-desc">Fiyat (azalan)</option>
            <option value="newest">En yeni</option>
          </select>
        </div>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-3 mt-4">
          <div>
            <label className="block text-xs text-gray-600 mb-1">Min ₺</label>
            <input
              type="number"
              className="input"
              value={minPrice ?? ''}
              onChange={(e) => {
                const val = e.target.value ? Number(e.target.value) : null
                setMinPrice(val)
                const params = new URLSearchParams(searchParams)
                if (val !== null) params.set('minPrice', String(val))
                else params.delete('minPrice')
                params.set('page', '1')
                setPage(1)
                setSearchParams(params)
              }}
              placeholder="0"
            />
          </div>
          <div>
            <label className="block text-xs text-gray-600 mb-1">Max ₺</label>
            <input
              type="number"
              className="input"
              value={maxPrice ?? ''}
              onChange={(e) => {
                const val = e.target.value ? Number(e.target.value) : null
                setMaxPrice(val)
                const params = new URLSearchParams(searchParams)
                if (val !== null) params.set('maxPrice', String(val))
                else params.delete('maxPrice')
                params.set('page', '1')
                setPage(1)
                setSearchParams(params)
              }}
              placeholder="1000"
            />
          </div>
          <label className="flex items-center gap-2 text-sm col-span-2 md:col-span-1">
            <input
              type="checkbox"
              checked={inStockOnly}
              onChange={(e) => {
                const checked = e.target.checked
                setInStockOnly(checked)
                const params = new URLSearchParams(searchParams)
                if (checked) params.set('inStock', '1')
                else params.delete('inStock')
                params.set('page', '1')
                setPage(1)
                setSearchParams(params)
              }}
            />
            Stokta olanlar
          </label>
        </div>
      </div>

      {isLoading ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6" aria-busy="true">
          {Array.from({ length: 8 }).map((_, i) => <Skeleton key={i} className="h-72" />)}
        </div>
      ) : filtered.length === 0 ? (
        <div className="text-center py-12">
          <p className="text-gray-500 text-lg">Ürün bulunamadı.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          {paged.map((product) => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      )}

      {filtered.length > pageSize && (
        <div className="flex justify-center items-center space-x-3 mt-8">
          <button
            className="btn btn-outline"
            disabled={safePage === 1}
            onClick={() => {
              const next = Math.max(1, safePage - 1)
              setPage(next)
              const params = new URLSearchParams(searchParams)
              params.set('page', String(next))
              setSearchParams(params)
            }}
          >
            Önceki
          </button>
          <div className="text-sm text-gray-600">Sayfa {safePage} / {totalPages}</div>
          <button
            className="btn btn-outline"
            disabled={safePage === totalPages}
            onClick={() => {
              const next = Math.min(totalPages, safePage + 1)
              setPage(next)
              const params = new URLSearchParams(searchParams)
              params.set('page', String(next))
              setSearchParams(params)
            }}
          >
            Sonraki
          </button>
        </div>
      )}
    </div>
  )
}
