-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BD_CLIENTES')
BEGIN
    CREATE DATABASE BD_CLIENTES;
END
GO

-- Utilizar la base de datos
USE BD_CLIENTES;
GO

-- Crear la tabla TiposDocumentos si no existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'TiposDocumentos' AND type = 'U')
BEGIN
    CREATE TABLE TiposDocumentos (
        Id INT PRIMARY KEY IDENTITY(1,1), -- Identificador único para cada tipo de documento
        Descripcion NVARCHAR(100) NOT NULL -- Descripción del tipo de documento
    );
END
GO

-- Insertar algunos registros de prueba solo si la tabla no tiene datos
IF NOT EXISTS (SELECT * FROM TiposDocumentos)
BEGIN
    INSERT INTO TiposDocumentos (Descripcion)
    VALUES 
        ('DNI'),
        ('Pasaporte'),
        ('Carnet de Extranjería'),
        ('Licencia de Conducir');
END
GO

-- Crear la tabla CambioMonedas si no existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'CambioMonedas' AND type = 'U')
BEGIN
    CREATE TABLE CambioMonedas (
        id INT PRIMARY KEY IDENTITY(1,1), -- Identificador único para cada registro
        moneda_base NVARCHAR(10) NOT NULL, -- Moneda base de cambio
        moneda_destino NVARCHAR(10) NOT NULL, -- Moneda a la cual se cambia
        tasa_cambio DECIMAL(10,4) NOT NULL, -- Tasa de cambio
        fecha DATETIME NOT NULL -- Fecha y hora de la tasa de cambio
    );
END
GO

-- Insertar algunos registros de prueba solo si la tabla no tiene datos
IF NOT EXISTS (SELECT * FROM CambioMonedas)
BEGIN
    INSERT INTO CambioMonedas (moneda_base, moneda_destino, tasa_cambio, fecha)
    VALUES 
        ('USD', 'EUR', 0.9275, '2024-11-01 08:00:00'),
        ('USD', 'GBP', 0.7543, '2024-11-01 09:00:00'),
        ('EUR', 'JPY', 150.42, '2024-11-01 10:00:00'),
        ('USD', 'JPY', 111.35, '2024-11-01 11:00:00');
END
GO
