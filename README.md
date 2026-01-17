# HediyeJoy E-Ticaret Platformu

Modern e-ticaret platformu - .NET Core Backend + React Frontend

## 📋 Mimari

Bu proje **Clean Architecture** prensiplerine göre tasarlanmıştır:

- **NotebookTherapy.API**: ASP.NET Core Web API (Presentation Layer)
- **NotebookTherapy.Application**: Business Logic ve Use Cases
- **NotebookTherapy.Core**: Domain Entities ve Interfaces
- **NotebookTherapy.Infrastructure**: Data Access, External Services
- **notebook-therapy-web**: React + TypeScript Frontend

## 🛠 Teknolojiler

### Backend
- .NET 8.0
- Entity Framework Core
- SQL Server
- JWT Authentication
- AutoMapper
- BCrypt (Password Hashing)

### Frontend
- React 18
- TypeScript
- Redux Toolkit
- React Router v6
- Tailwind CSS
- Vite
- Axios
- Lucide React (Icons)

## 🚀 Kurulum

### Gereksinimler
- .NET 8.0 SDK
- Node.js 18+ ve npm
- SQL Server (LocalDB veya SQL Server Express)

### Backend Kurulumu

1. Proje dizinine gidin:
```bash
cd Backend/NotebookTherapy.API
```

2. Paketleri yükleyin:
```bash
dotnet restore
```

3. Veritabanını oluşturun:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. Uygulamayı çalıştırın:
```bash
dotnet run
```

API `http://localhost:5000` adresinde çalışacaktır. Swagger UI: `http://localhost:5000/swagger`

### Frontend Kurulumu

1. Proje dizinine gidin:
```bash
cd Frontend/notebook-therapy-web
```

2. Paketleri yükleyin:
```bash
npm install
```

3. Geliştirme sunucusunu başlatın:
```bash
npm run dev
```

Frontend `http://localhost:3000` adresinde çalışacaktır.

## ✨ Özellikler

- ✅ Ürün katalogu ve kategoriler
- ✅ Ürün arama ve filtreleme
- ✅ Sepet yönetimi (Guest ve Authenticated)
- ✅ Kullanıcı kayıt ve giriş (JWT)
- ✅ Öne çıkan ürünler
- ✅ Yeni ürünler
- ✅ Tekrar stokta ürünler
- ✅ Koleksiyon bazlı filtreleme (Tsuki, Hinoki)
- ✅ Responsive tasarım
- ✅ Modern UI/UX

## 📁 Proje Yapısı

```
Backend/
├── NotebookTherapy.API/          # Web API Controllers
├── NotebookTherapy.Application/  # Business Logic, DTOs, Services
├── NotebookTherapy.Core/          # Domain Entities, Interfaces
└── NotebookTherapy.Infrastructure/# Data Access, Repositories, JWT

Frontend/
└── notebook-therapy-web/
    ├── src/
    │   ├── components/            # React Components
    │   ├── pages/                # Page Components
    │   ├── store/                # Redux Store & Slices
    │   └── services/             # API Services
```

## 🔐 Güvenlik

- JWT Token Authentication
- BCrypt Password Hashing
- CORS Configuration
- Input Validation

## 📝 API Endpoints

### Products
- `GET /api/products` - Tüm ürünler
- `GET /api/products/{id}` - Ürün detayı
- `GET /api/products/featured` - Öne çıkan ürünler
- `GET /api/products/new` - Yeni ürünler
- `GET /api/products/back-in-stock` - Tekrar stokta
- `GET /api/products/search?q={query}` - Ürün arama

### Categories
- `GET /api/categories` - Tüm kategoriler
- `GET /api/categories/{id}` - Kategori detayı

### Cart
- `GET /api/cart` - Sepeti getir
- `POST /api/cart/items` - Sepete ürün ekle
- `DELETE /api/cart/items/{id}` - Sepetten ürün çıkar

### Auth
- `POST /api/auth/register` - Kayıt ol
- `POST /api/auth/login` - Giriş yap

## 🎨 Tasarım

Modern, temiz ve kullanıcı dostu bir arayüz. Tailwind CSS ile responsive tasarım.

## 📄 Lisans

Bu proje eğitim amaçlıdır.
# ticaret
