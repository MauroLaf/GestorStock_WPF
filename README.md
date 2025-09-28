# 🗂️ GestorStock

**GestorStock** — Aplicación de escritorio para gestión de inventarios y control de stock.  
Proyecto diseñado con **arquitectura por capas** y la metodología **Code-Behind** (separación UI / lógica).  
Incluye exportación de datos a **Excel** mediante la librería **EPPlus**.

---

## 📌 Índice
1. [Descripción](#descripción)  
2. [Arquitectura](#arquitectura)  
3. [Flujo de la aplicación](#flujo-de-la-aplicación)  
4. [Requisitos](#requisitos)  
5. [Dependencias y librerías](#dependencias-y-librerías)  
6. [Cómo generar el .exe (build y publish)](#cómo-generar-el-exe-build-y-publish)  
7. [Rutas típicas donde aparece el .exe](#rutas-típicas-donde-aparece-el-exe)  
8. [Opciones de despliegue en LAN (paso a paso)](#opciones-de-despliegue-en-lan-paso-a-paso)  
9. [Base de datos centralizada y connection string](#base-de-datos-centralizada-y-connection-string)  
10. [Administración y actualizaciones](#administración-y-actualizaciones)  
11. [Problemas comunes y soluciones rápidas](#problemas-comunes-y-soluciones-rápidas)  
12. [Contribuir / Contacto / Licencia](#contribuir--contacto--licencia)

---

## Descripción
GestorStock es una aplicación de escritorio pensada para pequeñas/medianas oficinas que necesitan:
- Registrar productos.  
- Controlar entradas y salidas de stock.  
- Generar listados e informes básicos.  
- Exportar información a **Excel** de forma rápida y práctica.  

La aplicación separa la UI (ventanas/controles) de la lógica de negocio y de la lógica de acceso a datos para facilitar mantenimiento.

---

## Arquitectura
Estructura por capas (cada capa en su propio proyecto dentro de la solución):

- **UI (GestorStock.UI)** — Formularios / XAML + Code-Behind (event handlers).
- **Business (GestorStock.Business)** — Servicios y lógica de negocio (validaciones, reglas).
- **Data (GestorStock.Data)** — Repositorios / acceso a base de datos.
- **Entities (GestorStock.Entities)** — Clases `Producto`, `Movimiento`, `Usuario`, etc.

La UI llama a la capa Business; Business llama a Data; Data opera sobre la BD. Esto facilita pruebas y mantenimiento.

---

## Flujo de la aplicación
1. Usuario en la UI pulsa un botón (ej. `Agregar Producto`).  
2. El **Code-Behind** captura el evento y llama a `ProductoService.Add(producto)`.  
3. `ProductoService` valida reglas de negocio (stock mínimo, unicidad, etc.).  
4. Si OK, `ProductoRepository.Insert(producto)` escribe en la BD.  
5. Resultado (éxito/error) vuelve a la UI y se muestra al usuario.  
6. Si el usuario elige **Exportar a Excel**, la capa Business prepara los datos y con **EPPlus** se genera el archivo `.xlsx` directamente desde la aplicación.

---

## Requisitos
- **Visual Studio** (recomendado) o `dotnet SDK` si es .NET Core/.NET.  
- **.NET Framework** o **.NET 5/6/7/9** (depende de tu proyecto).  
- **Base de datos**: SQL Server / MySQL (según configuración).  
- Red LAN para la base de datos y/o carpeta compartida.  
- **Excel** instalado en los clientes (opcional: el archivo se genera en formato `.xlsx`, que puede abrirse también con LibreOffice, WPS Office, etc.).

---

## Dependencias y librerías
- **EPPlus** (https://github.com/EPPlusSoftware/EPPlus)  
  Usada para generar archivos `.xlsx` sin necesidad de tener Microsoft Office instalado en el servidor.  

  Instalación vía NuGet:
  ```bash
  Install-Package EPPlus
