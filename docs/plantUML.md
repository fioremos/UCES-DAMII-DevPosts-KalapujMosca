## 👁️ ¿Cómo visualizar los archivos `.puml`?

Para editar los diagramas PlantUML en Visual Studio Code, sigue estos pasos:

1. **Instala las extensiones necesarias:**
   - PlantUML

2. **Requisitos para renderizar:**

   **Opción local:**  
   Debes instalar previamente:
   - Java Runtime Environment (JRE)
   - GraphViz (para renderizar los diagramas)

   **Opción con servidor:**  
   - Presiona `Ctrl+P` para abrir el buscador de comandos.
   - Busca y selecciona: `>Preferences: Open User Settings (JSON)`.
   - Agrega al final del archivo JSON los siguientes atributos:

     ```json
     "plantuml.render": "PlantUMLServer",
     "plantuml.server": "http://www.plantuml.com/plantuml"
     ```

3. **Para editar un diagrama:**
   - Abre el archivo `.puml` correspondiente.
   - La vista previa se actualizará automáticamente mientras editas.
   - Usa `Alt+D` para abrir la vista previa en una ventana separada.

4. **Para exportar un diagrama:**
   - Haz clic derecho en el editor del archivo `.puml`.
   - Selecciona “Export Current File Diagrams”.
   - Elige el formato de salida (PNG, SVG, etc.).

5. **Comandos útiles:**
   - `Alt+D`: Abrir vista previa.
   - `Ctrl+Shift+P`: Abrir paleta de comandos y buscar "PlantUML: Export Current File Diagrams".

6. **Recurso Web:**
   - [Tutorial en YouTube](https://www.youtube.com/watch?v=OEB3Kw40AzU)
