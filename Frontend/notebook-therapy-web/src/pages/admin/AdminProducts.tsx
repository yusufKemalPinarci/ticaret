import { useEffect, useMemo, useState } from 'react'
import { productApi, fileApi, categoryApi } from '../../services/api'
import { Search, Edit2, X, Filter } from 'lucide-react'

export default function AdminProducts() {
  const [products, setProducts] = useState<any[]>([])
  const [categories, setCategories] = useState<any[]>([])
  const [form, setForm] = useState({ id: 0, name: '', price: 0, stock: 0, categoryId: 0, imageUrl: '', description: '' })
  const [uploading, setUploading] = useState(false)
  const [imageFileName, setImageFileName] = useState('')
  const [formError, setFormError] = useState<string | null>(null)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [query, setQuery] = useState('')
  const [categoryFilter, setCategoryFilter] = useState<number | null>(null)
  const [sort, setSort] = useState('newest')
  const [page, setPage] = useState(1)
  const pageSize = 8

  useEffect(() => {
    fetch()
    categoryApi.getAll().then(setCategories).catch(() => setCategories([]))
  }, [])

  const fetch = async () => {
    const data = await productApi.getAll()
    setProducts(data)
  }

  const handleCreate = async () => {
    setFormError(null)
    const name = form.name.trim()
    if (!name) return setFormError('Ürün adı zorunlu.')
    if (form.price <= 0) return setFormError('Fiyat 0 dan büyük olmalı.')
    if (form.categoryId <= 0) return setFormError('Kategori ID zorunlu.')
    if (form.stock < 0) return setFormError('Stok negatif olamaz.')
    if (!form.imageUrl) return setFormError('Görsel ekleyin veya URL girin.')

    const payload = {
      name,
      price: parseFloat(String(form.price)),
      stock: form.stock,
      categoryId: form.categoryId,
      imageUrl: form.imageUrl,
      description: form.description?.trim() || '',
      discountPrice: undefined,
    }

    if (editingId) {
      await productApi.update(editingId, payload)
    } else {
      await productApi.create(payload)
    }

    setForm({ id: 0, name: '', price: 0, stock: 0, categoryId: 0, imageUrl: '', description: '' })
    setImageFileName('')
    setEditingId(null)
    fetch()
  }

  const handleUpload = async (file?: File) => {
    if (!file) return
    setUploading(true)
    try {
      const res = await fileApi.upload(file)
      setForm({ ...form, imageUrl: res.url })
      setImageFileName(file.name)
    } catch (err) {
      alert('Dosya yüklenemedi')
    } finally {
      setUploading(false)
    }
  }

  const handleDelete = async (id: number) => {
    if (!confirm('Ürünü silmek istediğinize emin misiniz?')) return
    await productApi.delete(id)
    fetch()
  }

  const startEdit = (p: any) => {
    setEditingId(p.id)
    setForm({
      id: p.id,
      name: p.name || '',
      price: p.price || 0,
      stock: p.stock || 0,
      categoryId: p.categoryId || 0,
      imageUrl: p.imageUrl || '',
      description: p.description || '',
    })
    setImageFileName('')
  }

  const clearEdit = () => {
    setEditingId(null)
    setForm({ id: 0, name: '', price: 0, stock: 0, categoryId: 0, imageUrl: '', description: '' })
    setImageFileName('')
    setFormError(null)
  }

  const filtered = useMemo(() => {
    let list = [...products]
    if (query.trim()) {
      const q = query.toLowerCase()
      list = list.filter((p) => p.name?.toLowerCase().includes(q))
    }
    if (categoryFilter) {
      list = list.filter((p) => p.categoryId === categoryFilter)
    }
    switch (sort) {
      case 'price-asc':
        list.sort((a, b) => (a.discountPrice || a.price) - (b.discountPrice || b.price))
        break
      case 'price-desc':
        list.sort((a, b) => (b.discountPrice || b.price) - (a.discountPrice || a.price))
        break
      case 'stock-asc':
        list.sort((a, b) => (a.stock || 0) - (b.stock || 0))
        break
      case 'stock-desc':
        list.sort((a, b) => (b.stock || 0) - (a.stock || 0))
        break
      default:
        list.sort((a, b) => (b.id || 0) - (a.id || 0))
    }
    return list
  }, [products, query, categoryFilter, sort])

  const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize))
  const currentPage = Math.min(page, totalPages)
  const paged = filtered.slice((currentPage - 1) * pageSize, currentPage * pageSize)

  return (
    <div className="container mx-auto p-6 space-y-6">
      <h1 className="text-2xl font-bold mb-2">Ürünler</h1>

      <div className="flex flex-wrap items-center gap-3">
        <div className="relative">
          <Search className="w-4 h-4 text-gray-400 absolute left-3 top-1/2 -translate-y-1/2" />
          <input
            className="input pl-8"
            placeholder="Ürün ara"
            value={query}
            onChange={(e) => { setQuery(e.target.value); setPage(1) }}
          />
        </div>
        <div className="flex items-center gap-2">
          <Filter className="w-4 h-4 text-gray-500" />
          <select
            className="input"
            value={categoryFilter ?? ''}
            onChange={(e) => { setCategoryFilter(e.target.value ? Number(e.target.value) : null); setPage(1) }}
          >
            <option value="">Tüm kategoriler</option>
            {categories.map((c) => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
        </div>
        <select className="input" value={sort} onChange={(e) => setSort(e.target.value)}>
          <option value="newest">En yeni</option>
          <option value="price-asc">Fiyat (Artan)</option>
          <option value="price-desc">Fiyat (Azalan)</option>
          <option value="stock-asc">Stok (Artan)</option>
          <option value="stock-desc">Stok (Azalan)</option>
        </select>
      </div>

      <div className="card p-4 mb-6 space-y-3">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
          <div className="space-y-2">
            <label className="text-sm font-medium">Ürün Adı</label>
            <input className="input" placeholder="Örn: Lavanta Kokulu Mum" value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} />
          </div>
          <div className="space-y-2">
            <label className="text-sm font-medium">Kategori</label>
            <select
              className="input"
              value={form.categoryId || ''}
              onChange={(e) => setForm({ ...form, categoryId: Number(e.target.value) })}
            >
              <option value="">Kategori seçin</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
          <div className="space-y-2">
            <label className="text-sm font-medium">Fiyat (₺)</label>
            <input className="input" type="number" min={0} step="0.01" value={form.price} onChange={e => setForm({ ...form, price: Number(e.target.value) })} />
          </div>
          <div className="space-y-2">
            <label className="text-sm font-medium">Stok (adet)</label>
            <input className="input" type="number" min={0} value={form.stock} onChange={e => setForm({ ...form, stock: Number(e.target.value) })} />
          </div>
          <div className="space-y-2">
            <label className="text-sm font-medium">Resim</label>
            <div className="flex items-center space-x-2">
              <input
                type="file"
                accept="image/*"
                onChange={e => handleUpload(e.target.files?.[0] || undefined)}
              />
              {uploading && <span className="text-sm text-gray-500">Yükleniyor...</span>}
            </div>
            <input
              className="input mt-2"
              placeholder="Veya doğrudan URL girin"
              value={form.imageUrl}
              onChange={e => setForm({ ...form, imageUrl: e.target.value })}
            />
            {imageFileName && <div className="text-xs text-gray-500">Yüklendi: {imageFileName}</div>}
          </div>
        </div>

        <div className="space-y-2">
          <label className="text-sm font-medium">Açıklama</label>
          <textarea className="input" rows={2} value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
        </div>

        {formError && <div className="text-sm text-red-600">{formError}</div>}

        <div>
          <button className="btn btn-primary" onClick={handleCreate} disabled={uploading}>
            {editingId ? 'Kaydet' : 'Yeni Ürün Ekle'}
          </button>
          {editingId && (
            <button className="btn btn-outline ml-2" type="button" onClick={clearEdit}>
              <X className="w-4 h-4" />
            </button>
          )}
        </div>
      </div>

      <div className="space-y-3">
        {paged.map(p => (
          <div key={p.id} className="card p-4 flex items-center justify-between">
            <div className="flex items-center space-x-4">
              <img src={p.imageUrl} alt={p.name} className="w-20 h-20 object-cover rounded" loading="lazy" />
              <div>
                <div className="font-semibold">{p.name}</div>
                <div className="text-sm text-gray-500">₺{p.price} · Stok: {p.stock ?? 0}</div>
                {p.categoryId && <div className="text-xs text-gray-400">Kategori #{p.categoryId}</div>}
              </div>
            </div>
            <div>
              <button className="btn btn-outline mr-2 flex items-center gap-1" onClick={() => startEdit(p)}>
                <Edit2 className="w-4 h-4" /> Düzenle
              </button>
              <button className="btn btn-secondary" onClick={() => handleDelete(p.id)}>Sil</button>
            </div>
          </div>
        ))}
        {paged.length === 0 && <div className="text-sm text-gray-500">Kayıt bulunamadı.</div>}
      </div>

      {totalPages > 1 && (
        <div className="flex items-center gap-3">
          <button className="btn btn-outline" disabled={currentPage === 1} onClick={() => setPage((p) => Math.max(1, p - 1))}>Önceki</button>
          <span className="text-sm text-gray-600">Sayfa {currentPage} / {totalPages}</span>
          <button className="btn btn-outline" disabled={currentPage === totalPages} onClick={() => setPage((p) => Math.min(totalPages, p + 1))}>Sonraki</button>
        </div>
      )}
    </div>
  )
}
