import { useEffect, useMemo, useState } from 'react'
import api from '../../services/api'
import { Search } from 'lucide-react'

export default function AdminUsers(){
  const [users, setUsers] = useState<any[]>([])
  const [query, setQuery] = useState('')

  useEffect(()=>{ fetchUsers() }, [])

  const fetchUsers = async ()=>{
    const res = await api.get('/admin/users')
    setUsers(res.data || [])
  }

  const updateRole = async (id:number, role:string)=>{
    await api.put(`/admin/users/${id}/role`, role)
    fetchUsers()
  }

  const filtered = useMemo(() => {
    if (!query.trim()) return users
    const q = query.toLowerCase()
    return users.filter((u) => u.email?.toLowerCase().includes(q) || `${u.firstName} ${u.lastName}`.toLowerCase().includes(q))
  }, [users, query])

  return (
    <div className="container mx-auto p-6 space-y-4">
      <h1 className="text-2xl font-bold mb-2">Kullanıcılar</h1>
      <div className="flex items-center gap-3">
        <div className="relative">
          <Search className="w-4 h-4 text-gray-400 absolute left-3 top-1/2 -translate-y-1/2" />
          <input className="input pl-8" placeholder="E-posta veya ad" value={query} onChange={(e) => setQuery(e.target.value)} />
        </div>
      </div>
      <div className="space-y-3">
        {filtered.map(u=> (
          <div key={u.id} className="card p-4 flex items-center justify-between">
            <div>
              <div className="font-semibold">{u.email}</div>
              <div className="text-sm text-gray-500">{u.firstName} {u.lastName}</div>
            </div>
            <div className="flex items-center space-x-2">
              <select className="input" defaultValue={u.role} onChange={e=> updateRole(u.id, e.target.value)}>
                <option>Customer</option>
                <option>Admin</option>
              </select>
            </div>
          </div>
        ))}
        {filtered.length === 0 && <div className="text-sm text-gray-500">Kayıt bulunamadı.</div>}
      </div>
    </div>
  )
}
