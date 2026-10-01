# ¿Cómo Funciona la Arquitectura de Nuestra App?


## Justificación del Desacoplamiento Arquitectónico

En aplicaciones móviles basadas en frameworks modernos, la concentración de responsabilidades dentro del archivo de interfaz o su código subyacente (*code-behind*, `.xaml.cs`) conduce a antipatrones de diseño, comúnmente denominados *código espagueti*. Dicha práctica genera un alto acoplamiento entre la interfaz visual, las reglas de negocio y el acceso a datos.

Para garantizar los atributos de calidad de software requeridos (mantenibilidad, escalabilidad y testabilidad), el sistema adopta el principio de **Separación de Responsabilidades (SoC)** mediante la implementación estricta de:
* El patrón de presentación **MVVM (Model - View - ViewModel)**.
* El principio de **Inversión de Dependencias (DIP)** articulado a través de un contenedor nativo de **Inyección de Dependencias (DI)**.
* Servicios dedicados para la integración con la capa de red (APIs REST).


## Implementación del Patrón MVVM

El patrón MVVM estructura la presentación en tres componentes fundamentales con responsabilidades acotadas:

```
[ View (XAML) ] <--- Data Binding / ICommand ---> [ ViewModel ] <--- Servicios / DI ---> [ Model ]
```

### 1. Modelo (`Model`)
Representa la estructura de las entidades de dominio y datos crudos del sistema. Se mantiene completamente agnóstico a la tecnología visual y a la infraestructura de red.
* **Componente:** `Post.cs`
* **Responsabilidad:** Encapsula las propiedades de la entidad recibida desde la API externa (`Id`, `UserId`, `Title`, `Body`) mediante tipos de datos primitivos de C#.

### 2. Vista (`View`)
Define la estructura visual y distribución de los elementos de la interfaz de usuario en lenguaje declarativo XAML.
* **Componentes:** `MainPage.xaml` y `DetallePage.xaml`.
* **Criterio de diseño:** Carece por completo de lógica algorítmica y de negocio en su *code-behind*. Su comportamiento está supeditado a los datos provistos por su contexto de enlace (`BindingContext`), resolviendo la interacción del usuario mediante comandos.

### 3. ViewModel (`ViewModel`)
Actúa como intermediario de presentación entre la Vista y los servicios de la aplicación.
* **Componentes:** `MainViewModel.cs` y `DetalleViewModel.cs`.
* **Responsabilidades:**
  * **Notificación de Cambios:** Implementa la interfaz `INotifyPropertyChanged` para advertir a la Vista sobre modificaciones en el estado interno (por ejemplo, `EstaOcupado`, `MensajeEstado`, `ColorEstado`).
  * **Exposición de Comandos:** Publica instancias de `ICommand` (`CargarPostsCommand`, `SeleccionarPostCommand`) que abstraen eventos de usuario sin acoplarse a controles específicos de UI.
  * **Orquestación:** Delega la obtención de información a la capa de servicios y formatea el estado contextual antes de entregarlo a la Vista.


## 3. Capa de Servicios y Consumo de APIs REST

Para evitar la duplicación de código y aislar las operaciones de entrada/salida (I/O) de red, la interacción con la API de JSONPlaceholder se encapsula en una capa de servicio autónoma.

### 1. Contrato de Abstracción (`IPostService`)
Define el contrato funcional que requiere la aplicación:
```csharp
public interface IPostService
{
    Task<ResultadoOperacion<List<Post>>> ObtenerPostsAsync();
}
```
El uso de una interfaz permite desacoplar los consumidores (ViewModels) de la implementación tecnológica concreta, facilitando la futura sustitución del proveedor de datos o la incorporación de dobles de prueba (*mocks/fakes*) en entornos de testeo unitario.

### 2. Implementación (`PostService`)
* Utiliza una instancia compartida de `HttpClient` para evitar el agotamiento de sockets en el sistema operativo.
* Implementa una estrategia granular de manejo de excepciones asíncronas, diferenciando:
  * Errores de conectividad de bajo nivel y tiempo de espera (`HttpRequestException`, `TaskCanceledException`).
  * Respuestas erróneas del protocolo HTTP (`4xx` para errores de cliente, `5xx` para errores de servidor).
* Deserializa la carga útil en formato JSON hacia colecciones de objetos C# mediante `System.Text.Json`.



##  Inversión de Control e Inyección de Dependencias (DI)

La solución elimina la instanciación directa (`new`) de dependencias de infraestructura dentro de las clases de presentación. En su lugar, el ensamblaje de la aplicación se centraliza en `MauiProgram.cs` mediante el contenedor integrado de .NET MAUI.

### Política de Ciclos de Vida
* **`Singleton`:** Aplicado a servicios de red (`IPostService`, `PostService`). Se genera una única instancia durante todo el ciclo de vida de la aplicación, optimizando el consumo de recursos y reutilizando la infraestructura de red.
* **`Transient`:** Aplicado a los ViewModels (`MainViewModel`, `DetalleViewModel`) y las Páginas (`MainPage`, `DetallePage`). Se garantiza una instancia limpia e independiente en cada solicitud de navegación, evitando que el estado de una sesión o pantalla previa persista de forma indebida.



## Mecanismo de Navegación y Paso de Parámetros Complejos

La navegación entre la vista de listado y la vista de detalle se gestiona mediante el sistema declarativo basado en URI de **.NET MAUI Shell**.

Para resolver la transferencia de datos entre pantallas:
1. **Evitación de parámetros planos por URI:** Se desestima la descomposición de la entidad en cadenas primitivas individuales (`?id=1&title=texto`) debido a riesgos de truncamiento, incompatibilidad de codificación de caracteres o sobrecosto de re-consulta innecesaria.
2. **Transferencia por `ShellNavigationQueryParameters`:** Al invocarse la navegación con `Shell.Current.GoToAsync`, se encapsula el objeto completo `Post` dentro de un diccionario transitorio.
3. **Consumo vía `IQueryAttributable`:** El `DetalleViewModel` implementa el método `ApplyQueryAttributes`, recuperando la referencia directa al objeto deserializado. Este enfoque proporciona tipado seguro, bajo consumo de memoria y eliminación automática de la referencia tras la navegación.

---

## 6. Referencias a Diagramas de Arquitectura

Para un análisis esquemático del sistema, consultar los diagramas modelados en PlantUML en este mismo directorio:
* **Estructura y Relación de Capas:** [`arquitectura.puml`](./arquitectura.puml)
    ![Arquitectura](arquitectura.png)
* **Secuencia de Navegación y Parámetros:** [`navegacion_secuencia.puml`](./navegacion_secuencia.puml)
    ![Arquitectura](navegacion_secuencia.png)

* **Instrucciones de compilación y visualización:** [`plantUML.md`](./plantUML.md)
