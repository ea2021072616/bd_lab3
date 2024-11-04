IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BD_CLIENTES')
BEGIN
    CREATE DATABASE BD_CLIENTES;
END
GO

USE BD_CLIENTES;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'TiposDocumentos' AND type = 'U')
BEGIN
    CREATE TABLE TiposDocumentos (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Descripcion NVARCHAR(100) NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT * FROM TiposDocumentos)
BEGIN
    INSERT INTO TiposDocumentos (Descripcion)
    VALUES 
        ('Cédula de Ciudadanía'),
        ('Pasaporte Diplomático'),
        ('Carnet de Extranjero'),
        ('Licencia de Conducir Nacional'),
        ('Tarjeta de Residencia'),
        ('Número de Identificación Tributaria'),
        ('Acta de Nacimiento'),
        ('Visa de Estudiante'),
        ('Permiso de Conducir Europeo'); 
END
GO
