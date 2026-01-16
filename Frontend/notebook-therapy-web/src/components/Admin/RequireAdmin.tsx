import { ReactNode, useEffect, useState } from 'react'
import { useSelector, useDispatch } from 'react-redux'
import { RootState } from '../../store/store'
import { Navigate } from 'react-router-dom'
import api from '../../services/api'
import { AppDispatch } from '../../store/store'
import { setUser, parseToken, hydrateFromToken } from '../../store/slices/authSlice'

export default function RequireAdmin({ children }: { children: ReactNode }) {
  const auth = useSelector((state: RootState) => state.auth)
  const dispatch = useDispatch<AppDispatch>()
  const [loading, setLoading] = useState(false)

  const token = auth.accessToken
  const role = auth.user?.role || parseToken(token)?.role || null

  useEffect(() => {
    let mounted = true
    const fetchMe = async () => {
      if (!token) return
      if (auth.user) return
      setLoading(true)
      try {
        const response = await api.get('/users/me')
        if (!mounted) return
        const user = response.data?.role
          ? {
              email: response.data.email,
              firstName: response.data.firstName,
              lastName: response.data.lastName,
              role: response.data.role,
            }
          : parseToken(token)
        if (user) {
          dispatch(setUser(user))
          dispatch(hydrateFromToken(token))
        }
      } catch (err) {
        // ignore fetch errors; fallback to token parsing
      } finally {
        if (mounted) setLoading(false)
      }
    }
    fetchMe()
    return () => { mounted = false }
  }, [token, auth.user, dispatch])

  if (loading) return <div className="container mx-auto p-8 text-center">Doğrulanıyor...</div>

  if (role !== 'Admin') {
    return <Navigate to="/login" replace />
  }

  return <>{children}</>
}
