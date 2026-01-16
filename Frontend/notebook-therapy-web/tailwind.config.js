/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#fbf1f1',
          100: '#f8e6e6',
          200: '#f2cfcf',
          300: '#e6b2b2',
          400: '#d98e8e',
          500: '#c86f6f',
          600: '#b45353',
          700: '#943f3f',
          800: '#7a3333',
          900: '#612828',
        },
        surface: {
          DEFAULT: '#ffffff',
          muted: '#f8fafb'
        },
        muted: {
          50: '#f9fafb',
          100: '#f3f4f6',
          200: '#e5e7eb',
          300: '#d1d5db',
        }
      },
      boxShadow: {
        soft: '0 6px 18px rgba(15, 23, 42, 0.06)',
        inset: 'inset 0 1px 0 rgba(255,255,255,0.6)'
      },
      borderRadius: {
        xl: '1rem',
        '2xl': '1.5rem'
      },
    },
  },
  plugins: [],
}
