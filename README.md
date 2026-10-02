# DevPosts Reader (Lector de Publicaciones Técnicas) 

**Carrera:** Tecnicatura en Programación  

**Materia:** Desarrollo de Aplicaciones Móviles II  (UCES)

**Docente:** Nicolás Ferreira  

**Estudiantes:** 
* Sol Ailen Kalapuj - 154106
* Fiorella Mosca - 154108  




##  Descripción del Proyecto

**DevPosts Reader** es una aplicación móvil desarrollada en **.NET MAUI** orientada a la consulta y lectura de artículos y publicaciones técnicas. Consume la API REST pública de **JSONPlaceholder** (`https://jsonplaceholder.typicode.com/posts`).

La solución fue estructurada siguiendo las mejores prácticas:
* **Arquitectura MVVM Estricta:** Separación formal entre Vistas declarativas (XAML), ViewModels reactivos (`BaseViewModel` con `INotifyPropertyChanged`) y Modelos de dominio puros (`Post`).
* **Consumo Asíncrono de API REST:** Implementación de `HttpClient` en un servicio desacoplado (`PostService`) bajo el contrato `IPostService`.
* **Manejo Granular y Contextual de Errores:** Clasificación de fallos mediante el contenedor `ResultadoOperacion<T>`, distinguiendo errores de red/timeout (`HttpRequestException`, `TaskCanceledException`) de fallos HTTP (4xx / 5xx), reflejados con código de colores en la interfaz.
* **Navegación Shell con Objetos Complejos:** Transferencia de entidades completas mediante `ShellNavigationQueryParameters` y consumo en destino con `IQueryAttributable`, eliminando la pérdida de datos o fragmentación por URI.
* **Inyección de Dependencias (DI):** Registro centralizado en `MauiProgram.cs` aplicando `Singleton` para servicios y `Transient` para presentación.


##  Documentación del Proyecto

Toda la documentación técnica y explicativa se encuentra organizada en la carpeta `docs/`:

*  **[Explicación de la Arquitectura paso a paso](docs/arquitectura_explicada.md):** En este documento detalla los fundamentos teóricos, patrones de diseño y decisiones de arquitectura implementadas en la solución móvil **DevPosts Reader**, desarrollada sobre la plataforma **.NET MAUI**.

*  **[Guía de Visualización de PlantUML](docs/plantUML.md):** Instrucciones detalladas para abrir, previsualizar y compilar los diagramas `.puml`.


---

## 🚀 Compilación y Ejecución

### Requisitos Previos
* .NET 10.0 SDK (o superior) con la carga de trabajo de MAUI instalada.
* Visual Studio 2022 con el componente de desarrollo móvil .NET MAUI.

### Comandos de Compilación
Para compilar y verificar el proyecto desde terminal:

```bash
# Restauración y compilación en Windows
dotnet build -f net10.0-windows10.0.19041.0
```