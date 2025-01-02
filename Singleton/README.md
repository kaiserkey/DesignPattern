### Patrón de Diseño Singleton en C#  

El patrón Singleton garantiza que una clase tenga una única instancia en toda la aplicación y proporciona un punto global de acceso a esa instancia. Este patrón es muy útil en situaciones donde se necesita controlar estrictamente el acceso a recursos compartidos, como conexiones a bases de datos, manejadores de configuración o registros de logs.

**Un ejemplo practico: Logger**

Para diagramar un escenario de la vida real podemos imaginar que desarrollamos una aplicación que registra eventos en un archivo. No deseas que múltiples instancias de un logger creen varios archivos de log. En su lugar, deseas un único objeto que registre todos los eventos.

### Implementación de Singleton en C#
**Clase Logger como Singleton:**

```csharp
using System;
using System.IO;

public sealed class Logger
{
    private static Logger instance = null;
    private static readonly object lockObject = new object();

    private string logFilePath = "app.log";

    private Logger() { }

    public static Logger GetInstance()
    {
        if (instance == null)
        {
            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = new Logger();
                }
            }
        }
        return instance;
    }

    public void Log(string message)
    {
        File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
    }
}
```

**Uso del Singleton en la Aplicación:**

```csharp
class Program
{
    static void Main(string[] args)
    {
        Logger logger = Logger.GetInstance();
        logger.Log("Application started.");

        // Simulación de eventos en la aplicación.
        logger.Log("User logged in.");
        logger.Log("User performed an action.");

        Console.WriteLine("Logs registrados en app.log.");
    }
}
```

### Pros del Patron Singleton:
- Controla el acceso a recursos compartidos.
- Facilita el mantenimiento de estado global.

### Contras del Patron Singleton:
- Dificultad para realizar pruebas unitarias debido al acoplamiento.
- Riesgo de ser mal usado y violar principios de diseño como SRP (Single Responsibility Principle).

### Cómo usar correctamente el Singleton en C#:
El patrón Singleton no limita el número de veces que puedes usar la clase, sino que garantiza que solo haya una única instancia de esa clase en toda la aplicación. Esto significa que puedes llamar a la clase desde cualquier lugar, pero todas las llamadas usarán la misma instancia compartida.

Buenas Prácticas al Implementar un Singleton:
1. Usar un Singleton donde tenga sentido:
- Registro de logs (Logger).
- Cachés (almacenamiento temporal de datos).
- Configuraciones globales (configuración de una aplicación).
- Conexiones a bases de datos (solo si no necesitas múltiples conexiones).
2. Garantizar seguridad en multihilos  
  Si tu aplicación es multihilo, el Singleton debe protegerse adecuadamente para evitar que se creen múltiples instancias al mismo tiempo. Usa enfoques como:
  - Double-Checked Locking.
  - Inicialización temprana.
  - Lazy<T> instantiation.
3. Evitar que el Singleton sea clonado, reflejado o subclasificado

- **Clonado**: Sobrescribe el método **MemberwiseClone()** para lanzar una excepción.
- **Reflejado**: Usa un chequeo en el constructor privado para evitar instancias adicionales.
- **Subclasificado**: Sella la clase con sealed.
4. Exponer solo lo necesario
Evita sobrecargar el Singleton con lógica de negocio. Solo debe encapsular funcionalidad compartida.

### ¿Cómo implementar un Singleton correctamente?

```csharp
using System;
using System.IO;

public sealed class Logger
{
    private static readonly Lazy<Logger> instance = new Lazy<Logger>(() => new Logger());
    private string logFilePath;

    private Logger()
    {
        logFilePath = "log.txt";
    }

    public static Logger Instance => instance.Value;

    public void LogMessage(string message)
    {
        lock (instance)
        {
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}\n");
        }
    }
}
```

### Uso del Singleton:
Puedes llamar a Logger.Instance desde cualquier lugar de tu código, pero siempre usarás la misma instancia.

```csharp
class Program
{
    static void Main(string[] args)
    {
        Logger.Instance.LogMessage("Iniciando la aplicación...");
        Logger.Instance.LogMessage("Realizando una operación...");
        Logger.Instance.LogMessage("Finalizando la aplicación...");
    }
}
```

### ¿El Singleton solo puede usarse en un lugar?
¡No! Puedes usarlo en varios lugares de tu aplicación, pero siempre se compartirá la misma instancia.

Ejemplo Práctico: Cache Compartido


```csharp
public sealed class Cache
{
    private static readonly Lazy<Cache> instance = new Lazy<Cache>(() => new Cache());
    private Dictionary<string, string> cacheData;

    private Cache()
    {
        cacheData = new Dictionary<string, string>();
    }

    public static Cache Instance => instance.Value;

    public void Add(string key, string value)
    {
        lock (cacheData)
        {
            cacheData[key] = value;
        }
    }

    public string Get(string key)
    {
        lock (cacheData)
        {
            return cacheData.ContainsKey(key) ? cacheData[key] : "Key not found";
        }
    }
}
```

### Uso del Cache Compartido:
Puedes acceder al Singleton desde diferentes partes del código.

```csharp
class Program
{
    static void Main(string[] args)
    {
        Cache.Instance.Add("username", "JohnDoe");
        Cache.Instance.Add("email", "john.doe@example.com");

        Console.WriteLine(Cache.Instance.Get("username")); // Output: JohnDoe
        Console.WriteLine(Cache.Instance.Get("email"));    // Output: john.doe@example.com
    }
}
```

### Errores Comunes al Usar Singleton:
1. **Abusar del Singleton**
Si haces que muchas clases dependan de un Singleton, creas acoplamiento fuerte, lo que dificulta las pruebas unitarias.

    **Solución**: Usa patrones como Inversión de Dependencia (Dependency Injection) junto con el Singleton.

2. **No protegerlo en entornos multihilos**
Si tu Singleton no está protegido contra accesos simultáneos, puedes terminar con múltiples instancias.

    **Solución**: Usa Lazy<T> o Double-Checked Locking.

3. **Convertir el Singleton en un "Dios"**
Si metes demasiada lógica en el Singleton, se convierte en una clase difícil de mantener y probar.

    **Solución**: Mantén al Singleton simple y enfocado.


### ¿Por qué el patrón singleton se considera un antipatrón?
- Los singletons no son fáciles de manejar con pruebas unitarias. No se puede controlar su instanciación y pueden conservar el estado entre invocaciones.
- La memoria asignada a un singleton no se puede liberar.
- En un entorno multiproceso, es posible que se deba proteger el acceso al objeto singleton (por ejemplo, mediante sincronización).
- Los singletons promueven un acoplamiento estrecho entre clases, por lo que es difícil realizar pruebas.

Diferencia entre clase estática y patrón singleton:
- En C#, una clase estática no puede implementar una interfaz. Cuando una clase de instancia única necesita implementar una interfaz por alguna razón comercial o propósitos de IoC, puede usar el patrón Singleton sin una clase estática.
- Puede clonar el objeto de Singleton, pero no puede clonar el objeto de clase estática .
- El objeto Singleton se almacena en Heap, pero el objeto estático se almacena en stack.
- Un singleton se puede inicializar de forma diferida o asincrónica, mientras que una clase estática generalmente se inicializa cuando se carga por primera vez.