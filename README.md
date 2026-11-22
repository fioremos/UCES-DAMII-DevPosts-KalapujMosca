# DevPosts Reader (Lector de Publicaciones Técnicas) 

**Carrera:** Tecnicatura en Programación  

**Materia:** Desarrollo de Aplicaciones Móviles II  (UCES)

**Docente:** Nicolás Ferreira  

**Estudiantes:** 
* Sol Ailen Kalapuj - 154106
* Fiorella Mosca - 154108  




##  Descripción del Proyecto

**DevPosts Reader** es una aplicación móvil multiplataforma desarrollada en **.NET 10 MAUI** orientada a la consulta, creación y lectura de artículos y publicaciones técnicas con soporte **Offline-First**. Consume la API REST pública de **JSONPlaceholder** (`[https://jsonplaceholder.typicode.com/posts](https://jsonplaceholder.typicode.com/posts)`) y sincroniza los registros localmente en **SQLite**.

La solución fue estructurada siguiendo las mejores prácticas:
* **Arquitectura MVVM con CommunityToolkit.Mvvm:** Implementación reactiva basada en `ObservableObject`, eliminando código repetitivo mediante generadores de código en tiempo de compilación (*Source Generators*) a través de los atributos `[ObservableProperty]` y `[RelayCommand]`.
* **Modelos de Dominio y Validación (`Post` & `ValidadorPost`):** Entidades mapeadas para serialización con `System.Text.Json` y persistencia local con `sqlite-net-pcl`, respaldadas por reglas de validación puras en la capa de dominio.
* **Consumo Asíncrono de API REST y Estrategia Offline-First:** Implementación de `HttpClient` encapsulado en `PostService` (`IPostService`) junto con el patrón Repositorio (`IPostRepository` / `PostRepositorySQLite`), sincronizando datos remotos en almacenamiento local y garantizando recuperación automática ante pérdida de conectividad.
* **Integración de Hardware y Sensores (`IHardwareService`):** Abstracción desacoplada sobre `Microsoft.Maui.Essentials` (`MediaPicker`, `Geolocation`, `Vibration`) para captura/selección de fotografías en `AppDataDirectory`, geolocalización con gestión de permisos en tiempo de ejecución y retroalimentación háptica con trazabilidad vía `ILogger`.
* **Manejo Granular y Contextual de Errores:** Clasificación de respuestas mediante el contenedor `ResultadoOperacion<T>`, distinguiendo fallas de red/timeout (`HttpRequestException`, `TaskCanceledException`), errores HTTP (4xx / 5xx) y errores de persistencia, reflejados contextualmente en la interfaz de usuario.
* **Navegación Shell con Objetos Complejos:** Transferencia de la entidad completa (incluyendo imagen local y coordenadas GPS) a través de `ShellNavigationQueryParameters` y recepción segura en destino con `IQueryAttributable`, evitando la fragmentación o pérdida de datos por URI.
* **Inyección de Dependencias (DI):** Ensamblaje centralizado en `MauiProgram.cs` aplicando ciclo de vida `Singleton` para infraestructura (`HttpClient`, repositorios y servicios) y `Transient` para la capa de presentación (ViewModels y Páginas).
* **Suite de Pruebas Automatizadas (`DevPostsApp.Tests`):** Proyecto de testing con **xUnit** cubriendo pruebas unitarias de validación de dominio, pruebas de integración contra bases SQLite aisladas y pruebas del flujo *Offline-First* con Fakes y simulación HTTP.

---

##  Documentación del Proyecto

Toda la documentación técnica y explicativa se encuentra organizada en el directorio `docs/`:

* **[Explicación de la Arquitectura paso a paso](docs/arquitectura_explicada.md):** En este documento detalla los fundamentos teóricos, patrones de diseño y decisiones de arquitectura implementadas en la solución móvil **DevPosts Reader**, desarrollada sobre la plataforma **.NET MAUI**.
- **[Guía de Visualización de PlantUML](docs/plantUML.md):** Instrucciones detalladas para abrir, previsualizar y compilar los diagramas `.puml`.
- **Diagramas de Soporte:**
  - [Diagrama de Clases y Arquitectura (`docs/arquitectura.png`)](docs/arquitectura.png)
  - [Diagrama de Secuencia y Navegación (`docs/navegacion_secuencia.png`)](docs/navegacion_secuencia.png)

---

## Compilación y Ejecución

### Requisitos Previos
- .NET 10.0 SDK (o superior) con la carga de trabajo de MAUI instalada.
- Visual Studio 2022 con el componente de desarrollo móvil .NET MAUI y emulador Android (API 34+) o Windows App SDK.

### Comandos de Compilación y Pruebas
Para compilar y verificar el proyecto y su suite de tests desde la terminal:

```bash
# Ejecutar suite de pruebas unitarias y de integración (xUnit)
dotnet test DevPostsApp.Tests/DevPostsApp.Tests.csproj --verbosity normal

# Restauración y compilación en Windows
dotnet build DevPostsApp.csproj -f net10.0-windows10.0.19041.0

# Restauración y compilación en Android
dotnet build DevPostsApp.csproj -f net10.0-android
```