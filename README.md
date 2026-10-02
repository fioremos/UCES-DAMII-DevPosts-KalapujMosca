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
* **Arquitectura MVVM con CommunityToolkit.Mvvm:** Implementación reactiva basada en `ObservableObject`, eliminando código repetitivo mediante generadores de código en tiempo de compilación (*Source Generators*) a través de los atributos `[ObservableProperty]` y `[RelayCommand]`.
* **Modelos de Dominio Puros:** Entidades desacopladas (`Post`) serializadas de forma estricta mediante `System.Text.Json`.
* **Consumo Asíncrono de API REST:** Implementación de `HttpClient` encapsulado en un servicio autónomo (`PostService`) bajo el contrato de interfaz `IPostService`.
* **Manejo Granular y Contextual de Errores:** Clasificación de respuestas mediante el contenedor `ResultadoOperacion<T>`, distinguiendo fallas de red/timeout (`HttpRequestException`, `TaskCanceledException`) de errores de respuesta HTTP (4xx / 5xx), reflejados contextualmente mediante código de colores en la interfaz de usuario.
* **Navegación Shell con Objetos Complejos:** Transferencia de la entidad completa a través de `ShellNavigationQueryParameters` y recepción segura en destino con `IQueryAttributable`, evitando la fragmentación o pérdida de datos por URI.
* **Inyección de Dependencias (DI):** Ensamblaje centralizado en `MauiProgram.cs` aplicando ciclo de vida `Singleton` para clientes de red/servicios y `Transient` para la capa de presentación (ViewModels y Páginas).


##  Documentación del Proyecto

Toda la documentación técnica y explicativa se encuentra organizada en el directorio `docs/`:

*  **[Explicación de la Arquitectura paso a paso](docs/arquitectura_explicada.md):** En este documento detalla los fundamentos teóricos, patrones de diseño y decisiones de arquitectura implementadas en la solución móvil **DevPosts Reader**, desarrollada sobre la plataforma **.NET MAUI**.

*  **[Guía de Visualización de PlantUML](docs/plantUML.md):** Instrucciones detalladas para abrir, previsualizar y compilar los diagramas `.puml`.
* **Diagramas de Soporte:**
  * [Diagrama de Clases y Arquitectura (`docs/arquitectura.png`)](docs/arquitectura.png)
  * [Diagrama de Secuencia y Navegación (`docs/navegacion_secuencia.png`)](docs/navegacion_secuencia.png)

---

## Compilación y Ejecución

### Requisitos Previos
* .NET 10.0 SDK (o superior) con la carga de trabajo de MAUI instalada.
* Visual Studio 2022 con el componente de desarrollo móvil .NET MAUI.

### Comandos de Compilación
Para compilar y verificar el proyecto desde terminal:

```bash
# Restauración y compilación en Windows
dotnet build -f net10.0-windows10.0.19041.0
```