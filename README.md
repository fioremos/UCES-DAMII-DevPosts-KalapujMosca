# DevPosts Reader (Lector de Publicaciones Técnicas) 

**Carrera:** Tecnicatura en Programación  

**Materia:** Desarrollo de Aplicaciones Móviles II  (UCES)

**Docente:** Nicolás Ferreira  

**Estudiantes:** 
* Sol Ailen Kalapuj - 15406
* Fiorella Mosca - 154108  




##  Descripción del Proyecto

**DevPosts Reader** es una aplicación móvil desarrollada en **.NET MAUI** orientada a la consulta y lectura de artículos y publicaciones técnicas. Consume la API REST pública de **JSONPlaceholder** (`https://jsonplaceholder.typicode.com/posts`).

La solución fue estructurada siguiendo las mejores práctica:
* **Patrón MVVM estricto:** Separación total entre la interfaz de usuario en XAML y la lógica de presentación.
* **Inyección de Dependencias (DI):** Registro centralizado y resolución automática de dependencias en `MauiProgram.cs`.
* **Manejo Granular de Estados y Errores:** Clasificación en tiempo real de errores de red (conectividad/timeout) vs. errores de servidor HTTP (4xx / 5xx), reflejados contextualmente con colores y mensajes claros en la UI.
* **Navegación Shell con Objetos Complejos:** Transferencia del modelo de datos completo mediante `ShellNavigationQueryParameters`.



##  Documentación del Proyecto

Toda la documentación técnica y explicativa se encuentra organizada en la carpeta `docs/`:

*  **[Explicación de la Arquitectura paso a paso](docs/arquitectura_explicada.md):** En este documento detalla los fundamentos teóricos, patrones de diseño y decisiones de arquitectura implementadas en la solución móvil **DevPosts Reader**, desarrollada sobre la plataforma **.NET MAUI**.

*  **[Guía de Visualización de PlantUML](docs/plantUML.md):** Instrucciones detalladas para abrir, previsualizar y compilar los diagramas `.puml`.
