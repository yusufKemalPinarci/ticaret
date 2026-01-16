import { useEffect, useState } from 'react'
import { X } from 'lucide-react'
import { GlobalNotice, subscribeToNotices } from '../services/notifications'

const palette: Record<GlobalNotice['kind'], string> = {
  'rate-limit': 'bg-amber-100 text-amber-900 border-amber-300',
  network: 'bg-red-50 text-red-900 border-red-200',
  'auth-lock': 'bg-orange-100 text-orange-900 border-orange-200',
  info: 'bg-blue-50 text-blue-900 border-blue-200',
  warning: 'bg-yellow-50 text-yellow-900 border-yellow-300',
  success: 'bg-emerald-50 text-emerald-900 border-emerald-200',
  error: 'bg-red-50 text-red-900 border-red-200',
}

export default function GlobalBanner() {
  const [notice, setNotice] = useState<GlobalNotice | null>(null)

  useEffect(() => {
    let timeout: ReturnType<typeof setTimeout> | undefined
    const unsubscribe = subscribeToNotices((incoming) => {
      setNotice(incoming)
      if (timeout) clearTimeout(timeout)
      timeout = setTimeout(() => setNotice(null), incoming.kind === 'rate-limit' ? 8000 : 5000)
    })
    return () => {
      if (timeout) clearTimeout(timeout)
      unsubscribe()
    }
  }, [])

  if (!notice) return null

  const styling = palette[notice.kind] || palette.info
  const retry = notice.retryAfterSeconds
  return (
    <div className={`border-b ${styling}`}>
      <div className="container mx-auto px-4 py-3 flex items-center justify-between gap-4">
        <div className="text-sm font-medium">
          {notice.message}
          {retry ? <span className="ml-2 text-xs opacity-80">(Tahmini bekleme: ~{retry}s)</span> : null}
        </div>
        <button
          aria-label="Kapat"
          className="p-1 rounded hover:bg-white/40 transition"
          onClick={() => setNotice(null)}
        >
          <X className="w-4 h-4" />
        </button>
      </div>
    </div>
  )
}
