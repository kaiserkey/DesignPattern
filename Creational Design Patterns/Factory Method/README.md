### Patrón de Diseño  en C#

El **Factory Method** es un patrón de diseño creacional que proporciona una interfaz para crear objetos en una superclase, mientras permite a las subclases alterar el tipo de objetos que se crearán. Este patrón es comúnmente utilizado cuando se necesita crear objetos de clases que comparten una interfaz común pero tienen comportamientos diferentes, lo que permite una mayor flexibilidad y extensibilidad en el diseño del software.

## Propósito

- **Flexibilidad**: Permite que un sistema permanezca independiente de cómo se crean sus objetos.
- **Extensibilidad**: Facilita la adición de nuevos tipos de objetos sin modificar el código existente.
- **Encapsulación**: Oculta la lógica de creación de objetos del código cliente, proporcionando una interfaz común.

## Componentes Principales

1. **Product**: La interfaz común para los objetos que crea el Factory Method.
2. **Concrete Product**: Implementaciones específicas del Product.
3. **Creator**: La clase abstracta que declara el Factory Method.
4. **Concrete Creator**: Las subclases que implementan el Factory Method.

## Implementación Práctica: Sistema de Notificaciones

En este ejemplo, construiremos un sistema de notificaciones que soporta distintos tipos de notificación: correo electrónico (Email) y mensajes SMS. Utilizaremos el patrón Factory para crear notificaciones de distintos tipos sin modificar el código cliente cada vez que agreguemos un nuevo tipo de notificación.

### Definición de la Interfaz Product (INotification)

```csharp
public interface INotification
{
    void Send(string message);
    bool ValidateMessage(string message);  // Nuevo método
    Dictionary<string, string> GetMetadata();  // Nuevo método
}
```

### Implementación de Concrete Products

```csharp
public class EmailNotification : INotification
{
    private readonly string _serverConfig;
    private readonly int _port;

    public EmailNotification(string serverConfig = "smtp.default.com", int port = 587)
    {
        _serverConfig = serverConfig;
        _port = port;
    }

    public void Send(string message)
    {
        if (!ValidateMessage(message))
        {
            throw new ArgumentException("Mensaje de email inválido");
        }
        
        // Simulación de envío de email
        Console.WriteLine($"Configurando servidor SMTP: {_serverConfig}:{_port}");
        Console.WriteLine($"Enviando Email: {message}");
    }

    public bool ValidateMessage(string message)
    {
        return !string.IsNullOrEmpty(message) && message.Length <= 1000;
    }

    public Dictionary<string, string> GetMetadata()
    {
        return new Dictionary<string, string>
        {
            { "type", "email" },
            { "server", _serverConfig },
            { "port", _port.ToString() }
        };
    }
}

public class SmsNotification : INotification
{
    private readonly string _providerApi;
    private readonly string _apiKey;

    public SmsNotification(string providerApi = "default-sms-api", string apiKey = "default-key")
    {
        _providerApi = providerApi;
        _apiKey = apiKey;
    }

    public void Send(string message)
    {
        if (!ValidateMessage(message))
        {
            throw new ArgumentException("Mensaje SMS demasiado largo");
        }

        Console.WriteLine($"Conectando a API de SMS: {_providerApi}");
        Console.WriteLine($"Enviando SMS: {message}");
    }

    public bool ValidateMessage(string message)
    {
        return !string.IsNullOrEmpty(message) && message.Length <= 160;
    }

    public Dictionary<string, string> GetMetadata()
    {
        return new Dictionary<string, string>
        {
            { "type", "sms" },
            { "provider", _providerApi }
        };
    }
}
```

### Definición del Creator

```csharp
public abstract class NotificationFactory
{
    // Factory Method
    public abstract INotification CreateNotification();

    // Template Method
    public void SendNotification(string message)
    {
        var notification = CreateNotification();
        
        try
        {
            // Logging pre-envío
            Console.WriteLine($"Iniciando envío de notificación tipo: {notification.GetMetadata()["type"]}");
            
            notification.Send(message);
            
            // Logging post-envío
            Console.WriteLine("Notificación enviada exitosamente");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar notificación: {ex.Message}");
            throw;
        }
    }
}
```

### Implementación de Concrete Creators

```csharp
public class EmailNotificationFactory : NotificationFactory
{
    private readonly string _serverConfig;
    private readonly int _port;

    public EmailNotificationFactory(string serverConfig = "smtp.default.com", int port = 587)
    {
        _serverConfig = serverConfig;
        _port = port;
    }

    public override INotification CreateNotification()
    {
        return new EmailNotification(_serverConfig, _port);
    }
}

public class SmsNotificationFactory : NotificationFactory
{
    private readonly string _providerApi;
    private readonly string _apiKey;

    public SmsNotificationFactory(string providerApi = "default-sms-api", string apiKey = "default-key")
    {
        _providerApi = providerApi;
        _apiKey = apiKey;
    }

    public override INotification CreateNotification()
    {
        return new SmsNotification(_providerApi, _apiKey);
    }
}
```

### Ejemplo de Uso con Manejo de Errores

```csharp
public class NotificationService
{
    private readonly Dictionary<string, NotificationFactory> _factories;

    public NotificationService()
    {
        _factories = new Dictionary<string, NotificationFactory>
        {
            { "email", new EmailNotificationFactory() },
            { "sms", new SmsNotificationFactory() }
        };
    }

    public void SendNotification(string type, string message)
    {
        try
        {
            if (!_factories.ContainsKey(type))
            {
                throw new ArgumentException($"Tipo de notificación no soportado: {type}");
            }

            var factory = _factories[type];
            factory.SendNotification(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en el servicio de notificaciones: {ex.Message}");
            throw;
        }
    }
}

// Uso del servicio
class Program
{
    static void Main(string[] args)
    {
        var notificationService = new NotificationService();

        try
        {
            // Enviar email
            notificationService.SendNotification("email", "¡Bienvenido a nuestro servicio!");

            // Enviar SMS
            notificationService.SendNotification("sms", "Su código de verificación es: 123456");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en la aplicación: {ex.Message}");
        }
    }
}
```

## Mejores Prácticas y Consideraciones

### Cuándo Usar Factory Method

- Cuando no conoces de antemano los tipos exactos de objetos con los que tu código debe interactuar.
- Cuando quieres proporcionar una forma de extender las partes internas de tu biblioteca o framework sin alterar el código cliente.
- Cuando necesitas gestionar la creación de objetos complejos o con parámetros de configuración variables.

### Ventajas

- **Principio de Responsabilidad Única**: Mueve la responsabilidad de creación de los objetos a una clase específica.
- **Principio Abierto/Cerrado**: Puedes introducir nuevos tipos de productos sin modificar el código existente.
- **Mantenibilidad**: La centralización de la lógica de creación facilita el mantenimiento y la evolución del sistema.
- **Testabilidad**: Permite el uso de objetos mock en pruebas unitarias.

### Desventajas

- Puede resultar en una mayor cantidad de clases si se necesitan muchos tipos de productos.
- Requiere subclases para cada tipo de producto, lo que podría aumentar la complejidad si no se necesita tanta flexibilidad.
- Puede aumentar la complejidad del sistema si no se aplica adecuadamente.
