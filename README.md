# 🗂️ GestorStock

**GestorStock** — Aplicación de escritorio para gestión de inventarios y control de stock.  
Proyecto diseñado con **arquitectura por capas** y la metodología **Code-Behind** (separación UI / lógica).

---

## 📌 Índice
1. [Descripción](#descripción)  
2. [Arquitectura](#arquitectura)  
3. [Flujo de la aplicación](#flujo-de-la-aplicación)  
4. [Requisitos](#requisitos)  
5. [Cómo generar el .exe (build y publish)](#cómo-generar-el-exe-build-y-publish)  
6. [Rutas típicas donde aparece el .exe](#rutas-típicas-donde-aparece-el-exe)  
7. [Opciones de despliegue en LAN (paso a paso)](#opciones-de-despliegue-en-lan-paso-a-paso)  
   - [Opción A — Ejecutar desde carpeta compartida (rápido)](#opción-a---ejecutar-desde-carpeta-compartida-rápido)  
   - [Opción B — Instalar en cada puesto con instalador (recomendado)](#opción-b---instalar-en-cada-puesto-con-instalador-recomendado)  
   - [Opción C — ClickOnce / Publicación con actualizaciones automáticas](#opción-c---clickonce--publicación-con-actualizaciones-automáticas)  
8. [Base de datos centralizada y connection string](#base-de-datos-centralizada-y-connection-string)  
9. [Administración y actualizaciones](#administración-y-actualizaciones)  
10. [Problemas comunes y soluciones rápidas](#problemas-comunes-y-soluciones-rápidas)  
11. [Contribuir / Contacto / Licencia](#contribuir--contacto--licencia)

---

## Descripción
GestorStock es una aplicación de escritorio pensada para pequeñas/medianas oficinas que necesitan:
- Registrar productos.  
- Controlar entradas y salidas de stock.  
- Generar listados e informes básicos.

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

---

## Requisitos
- **Visual Studio** (recomendado) o `dotnet SDK` si es .NET Core/.NET.  
- **.NET Framework** o **.NET 5/6/7/9** (depende de tu proyecto).  
- **Base de datos**: SQL Server / MySQL (depende de tu configuración).  
- Red LAN para la base de datos y/o carpeta compartida.

---

## Cómo generar el .exe (build y publish)

### Desde Visual Studio (rápido)
1. Abre la solución `.sln` en Visual Studio.  
2. En **Solution Explorer** haz clic derecho en el proyecto UI → **Publish**.  
3. Elige **Folder** (carpeta) como destino y apunta a una carpeta temporal, por ejemplo `C:\Publish\GestorStock`.  
4. Opciones recomendadas:
   - **Configuration**: Release  
   - **Target runtime**: (por ejemplo) `win-x64` si quieres self-contained.  
   - Si quieres un único archivo EXE: marca `Produce single file` (si está disponible para tu framework).  
5. Pulsa **Publish**. La carpeta contendrá el `GestorStock.exe` y todas las dependencias.

### Desde línea de comandos (para .NET Core / .NET 5+)
Ejemplo para generar un **self-contained single file** (tamaño mayor, no necesita runtime en cliente):
```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o ./publish
