import { FormEvent, useEffect, useMemo, useState } from 'react'
import { Elements, CardElement, useElements, useStripe } from '@stripe/react-stripe-js'
import { loadStripe } from '@stripe/stripe-js'
import { useAppDispatch, useAppSelector } from '../store/hooks'
import { clearCart, fetchCart } from '../store/slices/cartSlice'
import { ensureCartSessionId, generateIdempotencyKey, ordersApi, cartApi } from '../services/api'
import { publishNotice } from '../services/notifications'
import axios from 'axios'
import { AlertTriangle, RefreshCw, ShieldCheck } from 'lucide-react'

const stripePublicKey = import.meta.env.VITE_STRIPE_PK
const stripePromise = stripePublicKey ? loadStripe(stripePublicKey) : null

function CheckoutForm() {
  const dispatch = useAppDispatch()
  const { items, totalAmount, isLoading } = useAppSelector((state) => state.cart)
  const stripe = useStripe()
  const elements = useElements()

  const [email, setEmail] = useState('')
  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [phone, setPhone] = useState('')
  const [shippingAddress, setShippingAddress] = useState('')
  const [billingAddress, setBillingAddress] = useState('')
  const [notes, setNotes] = useState('')
  const [couponCode, setCouponCode] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [cardReady, setCardReady] = useState(false)
  const [statusMessage, setStatusMessage] = useState<string | null>(null)
  const [couponStatus, setCouponStatus] = useState<string | null>(null)
  const [discountAmount, setDiscountAmount] = useState(0)
  const [shippingCost, setShippingCost] = useState(0)
  const [taxAmount, setTaxAmount] = useState(0)
  const [requiresAction, setRequiresAction] = useState(false)

  const sessionId = useMemo(() => ensureCartSessionId(), [])
  const [idempotencyKey, setIdempotencyKey] = useState(generateIdempotencyKey())

  useEffect(() => {
    dispatch(fetchCart(sessionId))
  }, [dispatch, sessionId])

  useEffect(() => {
    const subtotal = items.reduce((sum, item) => sum + item.totalPrice, 0)
    const shipping = subtotal >= 1000 ? 0 : Math.min(99, Math.max(0, subtotal * 0.05))
    const tax = Math.max(0, subtotal * 0.18)
    setShippingCost(Number(shipping.toFixed(2)))
    setTaxAmount(Number(tax.toFixed(2)))
  }, [items])

  const validateCoupon = async () => {
    if (!couponCode.trim()) {
      setCouponStatus('Kupon kodu girilmedi.')
      setDiscountAmount(0)
      return
    }
    try {
      const orderAmount = items.reduce((sum, item) => sum + item.totalPrice, 0) + shippingCost + taxAmount
      const res = await axios.get(`/api/coupons/${encodeURIComponent(couponCode.trim())}/validate`, {
        params: { orderAmount },
      })
      const data = res.data
      if (data?.isValid) {
        setDiscountAmount(Number(data.discountAmount || 0))
        setCouponStatus(data.message || 'Kupon uygulandı.')
        publishNotice({ kind: 'success', message: 'Kupon onaylandı.' })
      } else {
        setDiscountAmount(0)
        setCouponStatus(data?.message || 'Kupon geçersiz.')
      }
    } catch (err: any) {
      setDiscountAmount(0)
      const msg = err?.response?.data?.message || err?.response?.data?.Message || 'Kupon doğrulanamadı.'
      setCouponStatus(msg)
      publishNotice({ kind: 'warning', message: msg })
    }
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    if (!stripe || !elements) return
    setSubmitting(true)
    setStatusMessage(null)
    setRequiresAction(false)

    try {
      // refresh cart to ensure stock and totals are current
      const refreshed = await cartApi.getCart(sessionId)
      const refreshedTotal = refreshed?.items?.reduce((sum: number, item: any) => sum + item.totalPrice, 0) || 0
      if (Math.abs(refreshedTotal - totalAmount) > 0.01 || (refreshed?.items?.length || 0) !== items.length) {
        dispatch(fetchCart(sessionId))
        setStatusMessage('Sepet güncellendi. Lütfen kontrol edip tekrar deneyin.')
        setSubmitting(false)
        return
      }

      const payload = {
        idempotencyKey,
        couponCode: couponCode?.trim() || undefined,
        email: email.trim(),
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        phone: phone.trim(),
        sessionId,
        shippingAddress: shippingAddress.trim(),
        billingAddress: (billingAddress || shippingAddress).trim(),
        notes: notes?.trim() || undefined,
        shippingCost,
        tax: taxAmount,
        shippingRegion: 'TR',
        totalWeight: 0,
      }

      const order = await ordersApi.checkout(payload)
      if (!order?.id) {
        setStatusMessage('Sipariş oluşturulamadı.')
        setSubmitting(false)
        return
      }

      const intent = await ordersApi.createPaymentIntent(order.id, idempotencyKey, sessionId)
      if (!intent?.clientSecret) {
        setStatusMessage('Ödeme başlatılamadı.')
        setSubmitting(false)
        return
      }

      const cardElement = elements.getElement(CardElement)
      if (!cardElement) {
        setStatusMessage('Kart alanı yüklenmedi.')
        setSubmitting(false)
        return
      }

      const result = await stripe.confirmCardPayment(intent.clientSecret, {
        payment_method: {
          card: cardElement,
          billing_details: {
            email: email.trim(),
            name: `${firstName} ${lastName}`.trim(),
            phone: phone.trim() || undefined,
          },
        },
      })

      if (result.error) {
        setStatusMessage(result.error.message || 'Ödeme tamamlanamadı.')
      } else if (result.paymentIntent?.status === 'succeeded') {
        setStatusMessage('Ödeme başarılı! Siparişiniz oluşturuldu.')
        dispatch(clearCart(sessionId))
        setIdempotencyKey(generateIdempotencyKey())
        setCouponStatus(null)
        setDiscountAmount(0)
      } else if (result.paymentIntent?.status === 'requires_action') {
        setStatusMessage('3D Secure doğrulaması gerekiyor, lütfen doğrulamayı tamamlayın.')
        setRequiresAction(true)
      } else {
        setStatusMessage('Ödeme sonucu alınamadı, lütfen kontrol edin.')
      }
    } catch (err: any) {
      const message = err?.response?.data?.message || err?.message || 'Beklenmeyen hata oluştu.'
      setStatusMessage(message)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-6">Ödeme</h1>
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <form onSubmit={handleSubmit} className="lg:col-span-2 space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">Ad</label>
              <input className="input" value={firstName} onChange={(e) => setFirstName(e.target.value)} required />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Soyad</label>
              <input className="input" value={lastName} onChange={(e) => setLastName(e.target.value)} required />
            </div>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">E-posta</label>
              <input type="email" className="input" value={email} onChange={(e) => setEmail(e.target.value)} required />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Telefon</label>
              <input className="input" value={phone} onChange={(e) => setPhone(e.target.value)} />
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Teslimat Adresi (TR)</label>
            <textarea className="input" value={shippingAddress} onChange={(e) => setShippingAddress(e.target.value)} required />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Fatura Adresi (boş bırakılırsa teslimat)</label>
            <textarea className="input" value={billingAddress} onChange={(e) => setBillingAddress(e.target.value)} />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Not</label>
            <textarea className="input" value={notes} onChange={(e) => setNotes(e.target.value)} />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Kupon</label>
            <div className="flex gap-2">
              <input className="input flex-1" value={couponCode} onChange={(e) => setCouponCode(e.target.value)} placeholder="(opsiyonel)" />
              <button type="button" className="btn btn-outline" onClick={validateCoupon} disabled={!couponCode.trim() || submitting}>Kontrol et</button>
            </div>
            {couponStatus && <div className="text-sm mt-1 text-amber-700">{couponStatus}</div>}
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Kart Bilgisi</label>
            <div className="border rounded p-3 bg-white">
              <CardElement
                options={{ style: { base: { fontSize: '16px' } } }}
                onReady={() => setCardReady(true)}
                onChange={(event) => {
                  if (event.error) setStatusMessage(event.error.message || 'Kart bilgisi geçersiz.')
                }}
              />
            </div>
            <div className="flex items-center gap-2 text-xs text-gray-500 mt-2">
              <ShieldCheck className="w-4 h-4" />
              Kart bilgileriniz Stripe ile güvenli şekilde şifrelenir.
            </div>
          </div>
          {statusMessage && <div className="p-3 rounded bg-amber-50 text-amber-800 border border-amber-200">{statusMessage}</div>}
          {requiresAction && (
            <div className="p-3 rounded bg-blue-50 text-blue-800 border border-blue-200 flex items-center gap-2 text-sm">
              <AlertTriangle className="w-4 h-4" />
              Bankanızın 3D Secure ekranını tamamlayın, ardından gerekirse yeniden dene.
            </div>
          )}
          <button
            type="submit"
            className="btn btn-primary w-full md:w-auto"
            disabled={submitting || isLoading || !stripe || !cardReady}
          >
            {submitting ? 'İşleniyor...' : !cardReady ? 'Kart yükleniyor...' : 'Ödemeyi Tamamla'}
          </button>
          <div className="flex items-center gap-2 text-sm text-gray-500">
            <RefreshCw className="w-4 h-4" />
            Ödeme başarısız olursa yeniden denemeden önce kart ve bakiye bilgilerini kontrol edin.
          </div>
          <div className="p-3 border rounded text-sm text-gray-600 bg-gray-50">
            <div className="font-semibold mb-1">Alternatif ödeme yakında</div>
            Apple Pay, Google Pay ve kapıda ödeme seçenekleri üzerinde çalışıyoruz.
          </div>
          <p className="text-sm text-gray-500">Ödemeler şu an yalnızca Türkiye (TR) adresleri için açıktır.</p>
          <div className="text-sm text-gray-500">Kargo ve vergi hesaplaması ödeme sırasında doğrulanacaktır.</div>
        </form>

        <div className="lg:col-span-1">
          <div className="card p-6 sticky top-20 space-y-4">
            <h2 className="text-2xl font-bold">Sipariş Özeti</h2>
            <div className="divide-y">
              {items.map((item) => (
                <div key={item.id} className="py-2 flex justify-between">
                  <div>
                    <div className="font-medium">{item.productName}</div>
                    <div className="text-sm text-gray-500">{item.quantity} adet</div>
                  </div>
                  <div className="font-semibold">${item.totalPrice.toFixed(2)}</div>
                </div>
              ))}
            </div>
            <div className="border-t pt-3 space-y-1 text-sm text-gray-700">
              <div className="flex justify-between">
                <span>Ara toplam</span>
                <span>${totalAmount.toFixed(2)}</span>
              </div>
              <div className="flex justify-between">
                <span>Kargo</span>
                <span>${shippingCost.toFixed(2)}</span>
              </div>
              <div className="flex justify-between">
                <span>Vergi (tahmini)</span>
                <span>${taxAmount.toFixed(2)}</span>
              </div>
              {discountAmount > 0 && (
                <div className="flex justify-between text-emerald-700 font-medium">
                  <span>Kupon indirimi</span>
                  <span>- ${discountAmount.toFixed(2)}</span>
                </div>
              )}
              <div className="flex justify-between text-lg font-semibold pt-2">
                <span>Genel Toplam</span>
                <span>${(totalAmount + shippingCost + taxAmount - discountAmount).toFixed(2)}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

export default function CheckoutPage() {
  if (!stripePublicKey) {
    return <div className="container mx-auto px-4 py-12">Stripe public key (VITE_STRIPE_PK) tanımlı değil.</div>
  }

  return (
    <Elements stripe={stripePromise}>
      <CheckoutForm />
    </Elements>
  )
}
