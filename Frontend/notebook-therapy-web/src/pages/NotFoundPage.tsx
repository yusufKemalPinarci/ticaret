import { Link } from 'react-router-dom'

export default function NotFoundPage() {
  return (
    <div className="container mx-auto px-4 py-16 text-center">
      <h1 className="text-5xl font-bold mb-4">404</h1>
      <p className="text-gray-600 mb-6">Aradığınız sayfa bulunamadı.</p>
      <Link to="/" className="btn btn-primary">Ana sayfaya dön</Link>
    </div>
  )
}
