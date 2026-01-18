import PageMeta from '../components/PageMeta'

export default function KvkkPage() {
  return (
    <div className="container mx-auto px-4 py-12 max-w-4xl">
      <PageMeta title="KVKK Aydınlatma Metni | HediyeJoy" />
      <h1 className="text-3xl font-bold mb-8">KVKK Aydınlatma Metni</h1>
      <div className="card p-8 prose prose-rose max-w-none">
        <p>HediyeJoy olarak kişisel verilerinizin güvenliği hususuna azami hassasiyet göstermekteyiz.</p>

        <h2 className="text-xl font-bold mt-6 mb-2">1. Veri Sorumlusu</h2>
        <p>6698 sayılı Kişisel Verilerin Korunması Kanunu uyarınca, kişisel verileriniz veri sorumlusu olarak HediyeJoy tarafından aşağıda açıklanan kapsamda işlenebilecektir.</p>

        <h2 className="text-xl font-bold mt-6 mb-2">2. Kişisel Verilerin İşlenme Amacı</h2>
        <p>Toplanan kişisel verileriniz, siparişlerinizin yönetimi, faturalandırma süreçleri ve yasal yükümlülüklerimizin yerine getirilmesi amacıyla işlenmektedir.</p>

        <h2 className="text-xl font-bold mt-6 mb-2">3. İşlenen Kişisel Veriler</h2>
        <p>Ad, soyad, e-posta, telefon, adres, TC Kimlik No (isteğe bağlı/fatura için), Vergi No ve Vergi Dairesi (kurumsal fatura için).</p>

        <h2 className="text-xl font-bold mt-6 mb-2">4. İlgili Kişinin Hakları</h2>
        <p>Kanun'un 11. maddesi uyarınca veri sahibi olarak; verilerinizin işlenip işlenmediğini öğrenme, işlenmişse bilgi talep etme, düzeltilmesini veya silinmesini isteme haklarına sahipsiniz.</p>

        <div className="mt-8 p-4 bg-gray-50 rounded border italic text-sm">
            Bu metin örnek bir aydınlatma metnidir. Gerçek kullanımda hukuk danışmanınızdan onay almanız önerilir.
        </div>
      </div>
    </div>
  )
}
