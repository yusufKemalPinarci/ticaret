import { Link } from 'react-router-dom'

export default function Footer() {
  return (
    <footer className="bg-gray-900 text-white mt-auto">
      <div className="container mx-auto px-4 py-12">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          <div>
            <h3 className="text-xl font-bold mb-4">HediyeJoy</h3>
            <p className="text-gray-400">
              Hediyelik, kişiye özel ve özel gün kutuları. Mutlu eden paketler.
            </p>
          </div>

          <div>
            <h4 className="font-semibold mb-4">Mağaza</h4>
            <ul className="space-y-2 text-gray-400">
              <li><Link to="/products" className="hover:text-white">Tüm Hediyeler</Link></li>
              <li><Link to="/products?collection=Kisiye Ozel" className="hover:text-white">Kişiye Özel</Link></li>
              <li><Link to="/products?collection=Ozel Gun" className="hover:text-white">Özel Gün Koleksiyonları</Link></li>
            </ul>
          </div>

          <div>
            <h4 className="font-semibold mb-4">Müşteri Destek</h4>
            <ul className="space-y-2 text-gray-400">
              <li><a href="#" className="hover:text-white">Hakkımızda</a></li>
              <li><a href="#" className="hover:text-white">Kargo & İade</a></li>
              <li><a href="#" className="hover:text-white">İletişim</a></li>
            </ul>
          </div>

          <div>
            <h4 className="font-semibold mb-4">Follow Us</h4>
            <p className="text-gray-400 mb-2">
              İlham veren hediye fikirleri için bizi takip edin
            </p>
            <div className="flex space-x-4">
              <a href="#" className="hover:text-white">Instagram</a>
              <a href="#" className="hover:text-white">TikTok</a>
            </div>
          </div>
        </div>

        <div className="border-t border-gray-800 mt-8 pt-8 text-center text-gray-400">
          <p>&copy; 2025 HediyeJoy. Tüm hakları saklıdır.</p>
        </div>
      </div>
    </footer>
  )
}
