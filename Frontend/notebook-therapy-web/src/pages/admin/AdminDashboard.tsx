import { Link } from 'react-router-dom'

const links = [
  { title: 'Ürünler', description: 'Ekle, düzenle, sil', href: '/admin/products' },
  { title: 'Kategoriler', description: 'Kategori yönetimi', href: '/admin/categories' },
  { title: 'Siparişler', description: 'Durum ve takip', href: '/admin/orders' },
  { title: 'Kullanıcılar', description: 'Roller ve oturumlar', href: '/admin/users' },
]

export default function AdminDashboard() {
  return (
    <div className="container mx-auto p-6">
      <h1 className="text-3xl font-bold mb-6">Admin Dashboard</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-4">
        {links.map((link) => (
          <Link key={link.href} to={link.href} className="card p-4 hover:shadow-lg transition-shadow">
            <div className="text-lg font-semibold">{link.title}</div>
            <div className="text-sm text-gray-600 mt-1">{link.description}</div>
            <div className="mt-3 text-primary-600 font-medium">Yönetim sayfasına git →</div>
          </Link>
        ))}
      </div>
    </div>
  )
}
