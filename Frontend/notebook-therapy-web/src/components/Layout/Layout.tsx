import { ReactNode } from 'react'
import Header from './Header'
import Footer from './Footer'
import GlobalBanner from '../GlobalBanner'

interface LayoutProps {
  children: ReactNode
}

export default function Layout({ children }: LayoutProps) {
  return (
    <div className="min-h-screen flex flex-col">
      <a href="#main-content" className="sr-only focus:not-sr-only focus:absolute focus:top-2 focus:left-2 bg-white text-primary-700 px-3 py-2 rounded shadow">İçeriğe atla</a>
      <Header />
      <GlobalBanner />
      <main id="main-content" className="flex-grow">
        {children}
      </main>
      <Footer />
    </div>
  )
}
