import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { adminApi } from '../../services/api'
import { TrendingUp, Package, Clock, DollarSign, ChevronRight } from 'lucide-react'

export default function AdminDashboard() {
  const [stats, setStats] = useState<any>(null)

  useEffect(() => {
    adminApi.getStats()
      .then(setStats)
      .catch(console.error)
  }, [])

  const cards = [
    { title: 'Ürünler', desc: 'Ekle, düzenle, sil', href: '/admin/products', color: 'bg-blue-500' },
    { title: 'Kategoriler', desc: 'Kategori yönetimi', href: '/admin/categories', color: 'bg-purple-500' },
    { title: 'Siparişler', desc: 'Durum ve takip', href: '/admin/orders', color: 'bg-orange-500' },
    { title: 'Kuponlar', desc: 'İndirim yönetimi', href: '/admin/coupons', color: 'bg-rose-500' },
  ]

  return (
    <div className="container mx-auto p-6 space-y-8">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold text-gray-800">Admin Paneli</h1>
        <div className="text-sm text-gray-500">Hoş geldiniz, Admin</div>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div className="card p-6 border-l-4 border-emerald-500 flex items-center space-x-4">
          <div className="p-3 bg-emerald-100 rounded-full text-emerald-600">
            <DollarSign className="w-6 h-6" />
          </div>
          <div>
            <p className="text-sm text-gray-500 font-medium uppercase tracking-wider">Toplam Kazanç</p>
            <p className="text-2xl font-bold">₺{stats?.totalRevenue?.toFixed(2) || '0.00'}</p>
          </div>
        </div>

        <div className="card p-6 border-l-4 border-blue-500 flex items-center space-x-4">
          <div className="p-3 bg-blue-100 rounded-full text-blue-600">
            <Package className="w-6 h-6" />
          </div>
          <div>
            <p className="text-sm text-gray-500 font-medium uppercase tracking-wider">Toplam Sipariş</p>
            <p className="text-2xl font-bold">{stats?.totalOrders || 0}</p>
          </div>
        </div>

        <div className="card p-6 border-l-4 border-amber-500 flex items-center space-x-4">
          <div className="p-3 bg-amber-100 rounded-full text-amber-600">
            <Clock className="w-6 h-6" />
          </div>
          <div>
            <p className="text-sm text-gray-500 font-medium uppercase tracking-wider">Bekleyenler</p>
            <p className="text-2xl font-bold">{stats?.pendingOrders || 0}</p>
          </div>
        </div>

        <div className="card p-6 border-l-4 border-rose-500 flex items-center space-x-4">
          <div className="p-3 bg-rose-100 rounded-full text-rose-600">
            <TrendingUp className="w-6 h-6" />
          </div>
          <div>
            <p className="text-sm text-gray-500 font-medium uppercase tracking-wider">En Çok Satan</p>
            <p className="text-lg font-bold truncate max-w-[120px]">
              {stats?.topProducts?.[0]?.productName || '-'}
            </p>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Quick Links */}
        <div className="lg:col-span-2 grid grid-cols-1 md:grid-cols-2 gap-4">
          {cards.map((card) => (
            <Link key={card.href} to={card.href} className="card p-5 group hover:shadow-xl transition-all duration-300 relative overflow-hidden">
               <div className={`absolute top-0 right-0 w-1 h-full ${card.color}`}></div>
               <div className="flex justify-between items-start">
                  <div>
                    <h3 className="text-lg font-bold text-gray-800 group-hover:text-primary-600 transition-colors">{card.title}</h3>
                    <p className="text-sm text-gray-500 mt-1">{card.desc}</p>
                  </div>
                  <ChevronRight className="w-5 h-5 text-gray-300 group-hover:text-primary-500 transition-all transform group-hover:translate-x-1" />
               </div>
            </Link>
          ))}
        </div>

        {/* Top Products Mini Table */}
        <div className="card p-6">
          <h3 className="font-bold text-gray-800 mb-4 flex items-center gap-2">
            <TrendingUp className="w-5 h-5 text-primary-500" />
            En Çok Satanlar
          </h3>
          <div className="space-y-4">
            {stats?.topProducts?.map((p: any, i: number) => (
              <div key={p.productId} className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <span className="text-xs font-bold text-gray-400">#{i+1}</span>
                  <p className="text-sm font-medium text-gray-700 truncate max-w-[150px]">{p.productName}</p>
                </div>
                <div className="text-sm font-bold text-primary-600">{p.totalSold} Adet</div>
              </div>
            ))}
            {(!stats?.topProducts || stats.topProducts.length === 0) && (
              <div className="text-sm text-gray-400 italic">Henüz veri yok</div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
