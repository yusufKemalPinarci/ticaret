import { useEffect } from 'react'

interface PageMetaProps {
  title: string
  description?: string
}

export default function PageMeta({ title, description }: PageMetaProps) {
  useEffect(() => {
    const previousTitle = document.title
    document.title = title

    const meta = document.querySelector('meta[name="description"]') as HTMLMetaElement | null
    const previousDescription = meta?.content
    if (description) {
      if (meta) meta.content = description
      else {
        const el = document.createElement('meta')
        el.name = 'description'
        el.content = description
        document.head.appendChild(el)
      }
    }

    return () => {
      document.title = previousTitle
      if (meta && previousDescription) meta.content = previousDescription
    }
  }, [title, description])

  return null
}
