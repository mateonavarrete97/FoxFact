# FoxFact

Este proyecto es una aplicación que calcula y gestiona conceptos de facturación de energía eléctrica en PostgreSQL. Incluye consultas SQL complejas, un manager para manejar la lógica del negocio, y una API documentada con Swagger para interactuar con los datos.

---

## Características Principales

1. **Conexión con PostgreSQL**: Implementación de una clase reutilizable para gestionar conexiones con la base de datos PostgreSQL.
2. **Cálculo de Facturación Energética**:
   - Energía Activa (EA).
   - Comercialización de Excedentes de Energía (EC).
   - Excedentes de Energía tipo 1 (EE1).
   - Excedentes de Energía tipo 2 (EE2).
3. **API RESTful**:
   - Métodos implementados con Azure Functions.
   - Soporte para recibir parámetros a través de headers HTTP (`year` y `mes`).
   - Documentación con Swagger.
4. **Manejo de Errores**:
   - Validaciones exhaustivas para entradas.
   - Captura de excepciones específicas y generales.
5. **Separación de Lógica**:
   - Uso de DAO (Data Access Object) para las consultas SQL.
   - Manager para manejar la lógica del negocio.

---

## Instalación

### Prerrequisitos

1. **Base de Datos PostgreSQL**:
   - Configura una base de datos PostgreSQL con las tablas necesarias (`consumption`, `injection`, `tariffs`, etc.).
   - Asegúrate de que tu archivo `pg_hba.conf` permita las conexiones y esté configurado con un método de autenticación adecuado (por ejemplo, `scram-sha-256`).

2. **Dependencias de .NET**:
   - Asegúrate de tener instalado .NET 6 o superior.
   - Instala la biblioteca `Npgsql` para conexiones con PostgreSQL:
     ```bash
     dotnet add package Npgsql
     ```

