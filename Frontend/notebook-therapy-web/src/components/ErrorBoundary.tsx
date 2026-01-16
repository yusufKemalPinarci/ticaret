import { Component, ErrorInfo, ReactNode } from 'react'

type Props = { children: ReactNode }
type State = { hasError: boolean }

export default class ErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props)
    this.state = { hasError: false }
  }

  static getDerivedStateFromError() {
    return { hasError: true }
  }

  componentDidCatch(error: Error, info: ErrorInfo) {
    console.error('UI error boundary caught:', error, info)
  }

  handleReload = () => {
    this.setState({ hasError: false })
    window.location.href = '/'
  }

  render() {
    if (this.state.hasError) {
      return (
        <div className="container mx-auto px-4 py-12 text-center">
          <h1 className="text-3xl font-bold mb-3">Beklenmedik bir hata oluştu</h1>
          <p className="text-gray-600 mb-6">Sayfayı yenileyip tekrar deneyin.</p>
          <button className="btn btn-primary" onClick={this.handleReload}>Ana sayfaya dön</button>
        </div>
      )
    }
    return this.props.children
  }
}
