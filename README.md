# FoxFact

Este proyecto es una aplicación que calcula y gestiona conceptos de facturación de energía eléctrica en PostgreSQL. Incluye consultas SQL complejas, un manager para manejar la lógica del negocio y una API documentada con Swagger para interactuar con los datos.

---

## Características Principales

1. **Conexión con PostgreSQL**: Implementación de una clase reutilizable para gestionar conexiones con la base de datos PostgreSQL.
2. **Cálculo de Facturación Energética**:
   - **Energía Activa (EA)**: Cálculo del consumo de energía activa basado en registros de consumo.
   - **Comercialización de Excedentes de Energía (EC)**: Cálculo de inyecciones de energía a la red.
   - **Excedentes de Energía tipo 1 (EE1)**: Cálculo de excedentes limitados al consumo total.
   - **Excedentes de Energía tipo 2 (EE2)**: Excedente de inyección no cubierto por el consumo.
3. **API RESTful**:
   - Implementada con Azure Functions.
   - Soporte para recibir parámetros a través del cuerpo de la solicitud (`year`, `mes`, `idService`).
   - Documentación con Swagger.
4. **Manejo de Errores**:
   - Validaciones exhaustivas para entradas.
   - Captura de excepciones específicas y generales.
5. **Separación de Lógica**:
   - Uso de DAO (Data Access Object) para las consultas SQL.
   - Manager para manejar la lógica del negocio y centralizar la obtención de datos.

---

## Arquitectura

### Controladores
- **ReciboUsuarioController**: Expone el endpoint `CalculateInvoice` para calcular la factura de un usuario en un mes específico.

### DTOs (Data Transfer Objects)
- `CalculateInvoiceRequestDTO`: Contiene los datos de entrada (`year`, `mes`, `idService`).
- `EnergiaActivaDTO`: Representa el consumo de energía activa.
- `ComercializacionExcedentesEnergiaDTO`: Representa la comercialización de excedentes de energía.
- `ExcedentesEnergiaTipoUnoDTO`: Representa los excedentes tipo 1.
- `ExcedentesEnergiaTipoDosDTO`: Representa los excedentes tipo 2.
- `ReciboUsuarioDTO`: Contiene el resumen de la factura y el total a pagar.

### Capa de Acceso a Datos (DAO)
- **ApiDAO**: Implementa consultas SQL para obtener los datos energéticos de PostgreSQL.

### Capa de Lógica del Negocio (Managers)
- **ApiManager**: Gestiona la obtención de datos de energía activa y excedentes.
- **ReciboUsuarioManager**: Orquesta las consultas y calcula el total de la factura.

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

---

## Uso

Para calcular la factura de un usuario, realiza una solicitud `POST` al endpoint `CalculateInvoice` con el siguiente JSON en el cuerpo:

```json
{
    "year": 2024,
    "mes": 6,
    "idService": 12345
}
```

La respuesta incluirá los detalles de la factura y el total a pagar.

---

## Contacto
Si tienes preguntas o sugerencias, contáctanos a través del repositorio del proyecto.


