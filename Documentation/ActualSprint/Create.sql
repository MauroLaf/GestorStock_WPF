-- Crear base de datos (si no existe)
CREATE DATABASE IF NOT EXISTS StockDB;
USE StockDB;

-- Eliminar tablas si ya existen (orden inverso de dependencias)
DROP TABLE IF EXISTS Movimiento;
DROP TABLE IF EXISTS Stock;
DROP TABLE IF EXISTS Item;
DROP TABLE IF EXISTS Explotacion;

-- Crear tabla de Explotaciones
CREATE TABLE Explotacion (
    ExplotacionId INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(150) NOT NULL
);

-- Crear tabla de Items
CREATE TABLE Item (
    ItemId INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(150) NOT NULL,
    Categoria VARCHAR(100) NULL
);

-- Crear tabla de Stock
CREATE TABLE Stock (
    StockId INT AUTO_INCREMENT PRIMARY KEY,
    ExplotacionId INT NOT NULL,
    ItemId INT NOT NULL,
    UnidadesIniciales INT DEFAULT 0,
    FOREIGN KEY (ExplotacionId) REFERENCES Explotacion(ExplotacionId),
    FOREIGN KEY (ItemId) REFERENCES Item(ItemId)
);

-- Crear tabla de Movimientos
CREATE TABLE Movimiento (
    MovimientoId INT AUTO_INCREMENT PRIMARY KEY,
    StockId INT NOT NULL,
    Tipo ENUM('INCIDENCIA','PEDIDO') NOT NULL,
    Fecha DATE NOT NULL,
    Cantidad INT NOT NULL,
    Documento VARCHAR(50),
    FOREIGN KEY (StockId) REFERENCES Stock(StockId)
);
