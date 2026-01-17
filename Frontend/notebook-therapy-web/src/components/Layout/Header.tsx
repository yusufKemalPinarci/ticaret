import { Link } from 'react-router-dom'
import { ShoppingCart, User, Menu, Shield, X, Heart, Package, UserCircle } from 'lucide-react'
import { useSelector, useDispatch } from 'react-redux'
import { RootState } from '../../store/store'
import { logout } from '../../store/slices/authSlice'
import { clearTokens, authApi } from '../../services/api'
import { useState } from 'react'

export default function Header() {
  const { user, accessToken, refreshToken } = useSelector((state: RootState) => state.auth)
  const { itemCount } = useSelector((state: RootState) => state.cart)
  const dispatch = useDispatch()
  const [open, setOpen] = useState(false)

  const handleLogout = async () => {
    try {
      if (user && accessToken) {
        await authApi.logout(refreshToken ?? undefined)
      }
    } catch (err) {
      // ignore logout errors
    }
    dispatch(logout())
    clearTokens()
  }

  return (
    <header className="bg-white shadow-md sticky top-0 z-50">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          <Link to="/" className="text-2xl font-bold text-primary-600">
            HediyeJoy
          </Link>

          <nav className="hidden md:flex items-center space-x-6">
            <Link to="/" className="hover:text-primary-600 transition-colors">Ana Sayfa</Link>
            <Link to="/products" className="hover:text-primary-600 transition-colors">Tüm Hediyeler</Link>
            <Link to="/products?collection=Gunluk Hediye" className="hover:text-primary-600 transition-colors">Günlük Hediyeler</Link>
            <Link to="/products?collection=Kisiye Ozel" className="hover:text-primary-600 transition-colors">Kişiye Özel</Link>
            <Link to="/products?collection=Ozel Gun" className="hover:text-primary-600 transition-colors">Özel Gün</Link>
          </nav>

          <div className="flex items-center space-x-4">
            <Link to="/wishlist" className="hover:text-primary-600 transition-colors p-1" title="İstek Listem">
              <Heart className="w-6 h-6" />
            </Link>
            <Link to="/cart" className="relative hover:text-primary-600 transition-colors p-1" title="Sepetim">
              <ShoppingCart className="w-6 h-6" />
              {itemCount > 0 && (
                <span className="absolute -top-2 -right-2 bg-primary-600 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                  {itemCount}
                </span>
              )}
            </Link>

            {accessToken ? (
              <div className="flex items-center space-x-3">
                {user?.role === 'Admin' && (
                  <Link to="/admin" className="btn btn-outline hidden md:inline-flex items-center space-x-2">
                    <Shield className="w-4 h-4" />
                    <span>Admin</span>
                  </Link>
                )}

                <div className="relative group">
                  <button className="flex items-center space-x-1 hover:text-primary-600 py-2">
                    <User className="w-6 h-6" />
                    <span className="hidden lg:inline">{user?.firstName}</span>
                  </button>
                  <div className="absolute right-0 top-full hidden group-hover:block bg-white shadow-lg rounded-lg border py-2 min-w-[160px] z-50">
                    <Link to="/profile" className="flex items-center gap-2 px-4 py-2 hover:bg-gray-50 transition-colors">
                      <UserCircle className="w-4 h-4" />
                      <span>Profilim</span>
                    </Link>
                    <Link to="/orders" className="flex items-center gap-2 px-4 py-2 hover:bg-gray-50 transition-colors">
                      <Package className="w-4 h-4" />
                      <span>Siparişlerim</span>
                    </Link>
                    <div className="border-t my-1"></div>
                    <button
                      onClick={handleLogout}
                      className="w-full text-left flex items-center gap-2 px-4 py-2 text-red-600 hover:bg-red-50 transition-colors"
                    >
                      <span>Çıkış Yap</span>
                    </button>
                  </div>
                </div>
              </div>
            ) : (
              <Link to="/login" className="btn btn-outline">
                Giriş
              </Link>
            )}

            <button className="md:hidden" onClick={() => setOpen(!open)} aria-label="Menüyü aç/kapat">
              {open ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
            </button>
          </div>
        </div>
      </div>

      {open && (
        <div className="md:hidden border-t bg-white shadow-sm">
          <div className="flex flex-col space-y-2 p-4">
            <Link to="/" onClick={() => setOpen(false)} className="py-2">Ana Sayfa</Link>
            <Link to="/products" onClick={() => setOpen(false)} className="py-2">Tüm Hediyeler</Link>
            <Link to="/products?collection=Gunluk Hediye" onClick={() => setOpen(false)} className="py-2">Günlük Hediyeler</Link>
            <Link to="/products?collection=Kisiye Ozel" onClick={() => setOpen(false)} className="py-2">Kişiye Özel</Link>
            <Link to="/products?collection=Ozel Gun" onClick={() => setOpen(false)} className="py-2">Özel Gün</Link>
            <Link to="/wishlist" onClick={() => setOpen(false)} className="py-2">İstek Listem</Link>
            {accessToken ? (
              <>
                <Link to="/profile" onClick={() => setOpen(false)} className="py-2 border-t pt-4">Profilim</Link>
                <Link to="/orders" onClick={() => setOpen(false)} className="py-2">Siparişlerim</Link>
                {user?.role === 'Admin' && <Link to="/admin" onClick={() => setOpen(false)} className="py-2">Admin</Link>}
                <button onClick={() => { setOpen(false); handleLogout() }} className="text-left py-2 text-red-600 font-semibold mt-2">Çıkış Yap</button>
              </>
            ) : (
              <Link to="/login" onClick={() => setOpen(false)} className="py-2">Giriş</Link>
            )}
          </div>
        </div>
      )}
    </header>
  )
}
