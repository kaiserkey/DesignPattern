### Patrón de Diseño Singleton en C#  

El patrón Singleton se utiliza en situaciones donde necesitamos asegurarnos de que una clase tenga una única instancia en toda la aplicación, y ese objeto se debe poder acceder globalmente. Este patrón es especialmente útil cuando se gestionan recursos compartidos, como conexiones a bases de datos, manejadores de configuración, cache compartida, o el registro de logs.

### Ejemplo práctico: Logger

Imaginemos que estamos desarrollando una aplicación que necesita registrar eventos en un archivo. No queremos que se creen múltiples instancias de un Logger para escribir en diferentes archivos, sino que debemos asegurarnos de que solo haya una instancia de la clase que maneje el archivo de log.

**Implementación de Singleton en C# - Logger**

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

En este ejemplo, el Logger asegura que solo exista una instancia de la clase, incluso en entornos multihilo, mediante la sincronización (lock).

**Uso del Singleton en la Aplicación**

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


### Pros y Contras del Patrón Singleton
**Ventajas**: 

- **Control de recursos compartidos**: Se garantiza que todas las partes del sistema usen la misma instancia para acceder a los recursos.

- **Mantenimiento de estado global**: Facilita la gestión de configuraciones globales o registros centralizados.

**Desventajas**: 

- **Dificultad para pruebas unitarias**: La creación de un Singleton puede dificultar las pruebas, ya que la instancia es única y puede ser difícil de controlar durante las pruebas.

- **Acoplamiento**: Si no se usa adecuadamente, puede causar acoplamiento entre las clases que dependen del Singleton, violando el principio de responsabilidad única (SRP).

### Buenas Prácticas para Implementar un Singleton

**Usar donde tenga sentido**:

- **Registro de logs**: Útil para aplicaciones que necesitan registrar eventos globalmente.

- **Caché de datos**: Para almacenar datos accesibles globalmente.

- **Configuraciones globales**: Para gestionar la configuración de una aplicación.

- **Conexiones a bases de datos**: Para asegurar que solo haya una conexión activa.

### Seguridad en aplicaciones multihilo:
Usar técnicas como Double-Checked Locking o Lazy<T> para garantizar que no se creen instancias adicionales en entornos multihilo.

### Evitar que el Singleton sea subclasificado, clonado o reflejado:

- **Subclases**: Usa la palabra clave sealed para evitar que se herede la clase.

- **Clonación**: Implementa MemberwiseClone() para lanzar una excepción si se intenta clonar la instancia.

- **Reflexión**: Implementa una comprobación en el constructor para evitar instancias adicionales.

**Exponer solo lo necesario**: El Singleton debe centrarse solo en la funcionalidad compartida y no abarcar responsabilidades adicionales.

**Implementación Mejorada de un Singleton**: Logger con Lazy<T>
Usar Lazy<T> garantiza que la instancia solo se crea cuando se accede por primera vez, y es seguro para hilos.


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

### Uso del Logger Singleton


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


### Cache Compartido como Ejemplo
Otro ejemplo práctico es un sistema de caché compartido. Este Singleton almacena datos de manera eficiente y accesible globalmente.

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

### Uso del Cache Singleton

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

### Errores Comunes al Usar Singleton

**Abusar del Singleton**: Si muchas clases dependen de un Singleton, se crea un acoplamiento fuerte que dificulta las pruebas unitarias.  

- **Solución**: Usar patrones como Inyección de Dependencias (Dependency Injection) junto con el Singleton.

**No protegerlo en entornos multihilo**: Si el Singleton no está protegido adecuadamente, varios hilos podrían crear instancias simultáneamente.  

- **Solución**: Implementar mecanismos de protección como Double-Checked Locking o Lazy<T>.

**Sobrecargar el Singleton**: Si el Singleton contiene demasiada lógica de negocio, se convierte en una clase difícil de mantener y probar.

- **Solución**: Mantener el Singleton centrado solo en la funcionalidad compartida, sin agregarle lógica adicional.


### ¿Por qué el patrón singleton se considera un antipatrón?
- Los singletons no son fáciles de manejar con pruebas unitarias. No se puede controlar su instanciación y pueden conservar el estado entre invocaciones.
- La memoria asignada a un singleton no se puede liberar.
- En un entorno multiproceso, es posible que se deba proteger el acceso al objeto singleton (por ejemplo, mediante sincronización).
- Los singletons promueven un acoplamiento estrecho entre clases, por lo que es difícil realizar pruebas.

**Diferencia entre clase estática y patrón singleton**:
- En C#, una clase estática no puede implementar una interfaz. Cuando una clase de instancia única necesita implementar una interfaz por alguna razón comercial o propósitos de IoC, puede usar el patrón Singleton sin una clase estática.
- Puede clonar el objeto de Singleton, pero no puede clonar el objeto de clase estática .
- El objeto Singleton se almacena en Heap, pero el objeto estático se almacena en stack.
- Un singleton se puede inicializar de forma diferida o asincrónica, mientras que una clase estática generalmente se inicializa cuando se carga por primera vez.