# Sistema de Gestión de Clientes - Prueba Técnica

Este proyecto consiste en una solución integral (API + Desktop) para la gestión de clientes, desarrollada bajo estándares de código limpio, arquitectura en capas y principios SOLID.

## 🚀 Tecnologías Utilizadas

* **Backend:** ASP.NET Core API 10.0
* **Frontend:** Windows Forms (WinForms) .NET 8.0
* **Testing:** xUnit & Moq (Enfoque TDD)
* **Persistencia:** JSON Asíncrono

## 🏗️ Arquitectura y Patrones

Se ha implementado una **Arquitectura en 3 Capas** para garantizar la separación de intereses y la mantenibilidad del sistema:

1.  **Capa de Presentación (Controllers):** Gestión de peticiones HTTP y respuestas.
2.  **Capa de Negocio (Managers):** Validación de reglas (DNI único, integridad de datos).
3.  **Capa de Acceso a Datos (Services):** Persistencia en archivos JSON utilizando I/O asíncrono.

### Mejoras Implementadas
* **Inyección de Dependencias (DI):** Uso de interfaces para desacoplar componentes y facilitar el testing.
* **Programación Asíncrona:** Implementación de `Task` y `async/await` en toda la cadena de llamadas para optimizar el rendimiento.
* **Middleware Global:** Captura centralizada de excepciones para respuestas coherentes y seguras.
* **Resiliencia:** Manejo de casos de borde (listas vacías, archivos corruptos).
* **Validación Case-Insensitive:** Control de duplicados inteligente independientemente del uso de mayúsculas/minúsculas.

## 🧪 Pruebas Unitarias (TDD)

El proyecto incluye una suite de tests automáticos que cubren:
* Validación de unicidad de DNI.
* Manejo de excepciones de infraestructura (ej. errores de disco).
* Consistencia en la recuperación de datos (evitar NullReferenceExceptions).

Para ejecutar los tests:
```bash
dotnet test
```

## 🖥️ Aplicación Desktop
La aplicación WinForms se ha diseñado con un enfoque dinámico:

- UI Data-Driven: El DataGridView genera columnas automáticamente por reflexión del modelo.

U- X Mejorada: Interfaz estructurada con paneles (Header, Grid, Footer) que soportan el redimensionado dinámico.

- Manejo de Assets: Carga y escalado dinámico de logotipos e iconos.

## 🛠️ Instalación y Ejecución
Clonar el repositorio.

Restaurar dependencias:
```bash
dotnet restore
```

Ejecutar API:
```bash
cd ClienteApi
dotnet run
```

Ejecutar Desktop
```bash
cd ClienteDesktop
dotnet run
```

## Desarrollado por: Mario Ventura
