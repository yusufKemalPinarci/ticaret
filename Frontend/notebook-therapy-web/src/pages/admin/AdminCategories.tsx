import { useEffect, useMemo, useState } from 'react'
import { categoryApi } from '../../services/api'
import { Edit2, Search } from 'lucide-react'

export default function AdminCategories() {
  const [categories, setCategories] = useState<any[]>([])
  const [form, setForm] = useState({ id: 0, name: '', slug: '', displayOrder: 0 })
  const [editingId, setEditingId] = useState<number | null>(null)
  const [query, setQuery] = useState('')

  useEffect(() => { fetch() }, [])

  const fetch = async () => {
    const data = await categoryApi.getAll()
    setCategories(data)
  }

  const handleCreate = async () => {
    if (!form.name.trim()) return
    if (!form.slug.trim()) return
    if (editingId) {
      await categoryApi.update(editingId, { name: form.name, slug: form.slug, displayOrder: form.displayOrder })
    } else {
      await categoryApi.create({ name: form.name, slug: form.slug, displayOrder: form.displayOrder })
    }
    setForm({ id: 0, name: '', slug: '', displayOrder: 0 })
    setEditingId(null)
    fetch()
  }

  const handleDelete = async (id: number) => {
    if (!confirm('Kategoriyi silmek istediğinize emin misiniz?')) return
    await categoryApi.delete(id)
    fetch()
  }

  const startEdit = (c: any) => {
    setEditingId(c.id)
    setForm({ id: c.id, name: c.name, slug: c.slug, displayOrder: c.displayOrder || 0 })
  }

  const filtered = useMemo(() => {
    if (!query.trim()) return categories
    const q = query.toLowerCase()
    return categories.filter((c) => c.name?.toLowerCase().includes(q) || c.slug?.toLowerCase().includes(q))
  }, [categories, query])

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-2xl font-bold mb-4">Kategoriler</h1>
      <div className="flex items-center gap-3 mb-3">
        <div className="relative">
          <Search className="w-4 h-4 text-gray-400 absolute left-3 top-1/2 -translate-y-1/2" />
          <input className="input pl-8" placeholder="Ara" value={query} onChange={(e) => setQuery(e.target.value)} />
        </div>
      </div>

      <div className="card p-4 mb-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-2">
          <input className="input" placeholder="Ad" value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} />
          <input className="input" placeholder="Slug" value={form.slug} onChange={e => setForm({ ...form, slug: e.target.value })} />
          <input className="input" placeholder="Sıra" type="number" value={form.displayOrder} onChange={e => setForm({ ...form, displayOrder: Number(e.target.value) })} />
        </div>
        <div className="mt-3 flex items-center gap-2">
          <button className="btn btn-primary" onClick={handleCreate}>{editingId ? 'Kaydet' : 'Yeni Kategori'}</button>
          {editingId && <button className="btn btn-outline" onClick={() => { setEditingId(null); setForm({ id: 0, name: '', slug: '', displayOrder: 0 }) }}>Vazgeç</button>}
        </div>
      </div>

      <div className="space-y-3">
        {filtered.map(c => (
          <div key={c.id} className="card p-4 flex items-center justify-between">
            <div>
              <div className="font-semibold">{c.name}</div>
              <div className="text-sm text-gray-500">{c.slug}</div>
            </div>
            <div>
              <button className="btn btn-outline mr-2 flex items-center gap-1" onClick={() => startEdit(c)}><Edit2 className="w-4 h-4" /> Düzenle</button>
              <button className="btn btn-secondary" onClick={() => handleDelete(c.id)}>Sil</button>
            </div>
          </div>
        ))}
        {filtered.length === 0 && <div className="text-sm text-gray-500">Kayıt bulunamadı.</div>}
      </div>
    </div>
  )
}
