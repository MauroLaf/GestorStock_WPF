using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorStock.Services.Implementations
{
    public class RepuestoCatalogoService : IRepuestoCatalogoService
    {
        private readonly StockDbContext _db;

        public RepuestoCatalogoService(StockDbContext db)
        {
            _db = db;
        }

        public async Task<List<RepuestoCatalogo>> GetAllAsync()
        {
            return await _db.RepuestoCatalogos
                .AsNoTracking()
                .Include(rc => rc.Familia)
                .Include(rc => rc.UbicacionProducto)
                .Include(rc => rc.TipoSoporte)
                .OrderBy(rc => rc.Nombre)
                .ToListAsync();
        }

        public async Task<RepuestoCatalogo?> GetByIdAsync(int id)
        {
            return await _db.RepuestoCatalogos
                .AsNoTracking()
                .Include(rc => rc.Familia)
                .Include(rc => rc.UbicacionProducto)
                .Include(rc => rc.TipoSoporte)
                .FirstOrDefaultAsync(rc => rc.Id == id);
        }

        public async Task<List<RepuestoCatalogo>> GetByFamiliaAsync(int familiaId)
        {
            return await _db.RepuestoCatalogos
                .AsNoTracking()
                .Where(rc => rc.FamiliaId == familiaId)
                .Include(rc => rc.Familia)
                .Include(rc => rc.UbicacionProducto)
                .Include(rc => rc.TipoSoporte)
                .OrderBy(rc => rc.Nombre)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string nombre, int? familiaId = null)
        {
            nombre = (nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nombre)) return false;

            var q = _db.RepuestoCatalogos.AsNoTracking()
                    .Where(rc => rc.Nombre.ToLower() == nombre.ToLower());

            if (familiaId.HasValue)
                q = q.Where(rc => rc.FamiliaId == familiaId);

            return await q.AnyAsync();
        }

        public async Task<RepuestoCatalogo> CreateAsync(RepuestoCatalogo entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            entity.Nombre = (entity.Nombre ?? string.Empty).Trim();

            // Evita duplicados por nombre+familia
            var dup = await _db.RepuestoCatalogos
                .FirstOrDefaultAsync(rc =>
                    rc.Nombre.ToLower() == entity.Nombre.ToLower() &&
                    rc.FamiliaId == entity.FamiliaId);

            if (dup != null) return dup;

            _db.RepuestoCatalogos.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(RepuestoCatalogo entity)
        {
            if (entity is null) return false;

            // Protección contra duplicados
            var nombre = (entity.Nombre ?? string.Empty).Trim();
            var exists = await _db.RepuestoCatalogos.AnyAsync(rc =>
                rc.Id != entity.Id &&
                rc.Nombre.ToLower() == nombre.ToLower() &&
                rc.FamiliaId == entity.FamiliaId);

            if (exists) return false;

            var current = await _db.RepuestoCatalogos.FirstOrDefaultAsync(rc => rc.Id == entity.Id);
            if (current == null) return false;

            current.Nombre = nombre;
            current.FamiliaId = entity.FamiliaId;
            current.UbicacionProductoId = entity.UbicacionProductoId;
            current.TipoSoporteId = entity.TipoSoporteId;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var current = await _db.RepuestoCatalogos.FirstOrDefaultAsync(rc => rc.Id == id);
            if (current == null) return false;

            _db.RepuestoCatalogos.Remove(current);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<RepuestoCatalogo> EnsureAsync(
            string nombre,
            int? familiaId = null,
            int? ubicacionProductoId = null,
            int? tipoSoporteId = null)
        {
            nombre = (nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

            var existing = await _db.RepuestoCatalogos.FirstOrDefaultAsync(rc =>
                rc.Nombre.ToLower() == nombre.ToLower() &&
                rc.FamiliaId == familiaId);

            if (existing != null) return existing;

            var nuevo = new RepuestoCatalogo
            {
                Nombre = nombre,
                FamiliaId = familiaId,
                UbicacionProductoId = ubicacionProductoId,
                TipoSoporteId = tipoSoporteId
            };

            _db.RepuestoCatalogos.Add(nuevo);
            await _db.SaveChangesAsync();
            return nuevo;
        }
    }
}
