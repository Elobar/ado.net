# GestionVideojuegos — ADO.NET con SQLite en C#

Programa de consola en C# que demuestra el acceso a datos con **ADO.NET** usando **SQLite** como base de datos.
Hace parte de la actividad de investigación _"Introducción a ADO.NET y su aplicación en C#"_ (Programación en C# con .NET, semana 12).

## ¿Qué hace el programa?

1. Crea (si no existe) la base de datos SQLite `videojuegos.db`.
2. Abre una conexión con `SqliteConnection`.
3. Crea la tabla `Videojuegos` con 6 columnas (`Id`, `Titulo`, `Genero`, `Plataforma`, `Anio`, `Calificacion`).
4. Inserta 5 registros con `SqliteCommand` y parámetros (`@titulo`, `@genero`, ...).
5. Consulta y muestra los registros con `SqliteDataReader` (una consulta general y otra con filtro parametrizado).
6. Usa `using` para liberar la conexión, los comandos y los lectores.

## Requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) (el proyecto apunta a `net10.0`).
- Conexión a internet la primera vez, para restaurar el paquete NuGet `Microsoft.Data.Sqlite`.

> Si tienes instalada otra versión del SDK (por ejemplo 8 o 9), cambia `<TargetFramework>` en
> `GestionVideojuegos.csproj` (por ejemplo a `net8.0`) y ejecuta `dotnet add package Microsoft.Data.Sqlite`.
> Puedes ver tu versión con `dotnet --version`.

## Cómo ejecutarlo

```bash
git clone <https://github.com/Elobar/ado.net>
cd GestionVideojuegos
dotnet restore
dotnet run
```

También puedes abrir la carpeta en Visual Studio o Rider y ejecutar el proyecto (F5).

El archivo `videojuegos.db` se crea en el directorio de trabajo; el programa imprime su ruta completa al iniciar.
La tabla se reinicia en cada ejecución, así que el resultado siempre es el mismo.

## Estructura

```
GestionVideojuegos/
├── GestionVideojuegos.csproj   # Configuración del proyecto y paquete NuGet
├── Program.cs                  # Código fuente (conexión, comandos y lector)
├── README.md
└── .gitignore
```

## Tecnologías

- C# / .NET
- ADO.NET (`Connection`, `Command`, `DataReader`, `Parameters`)
- SQLite mediante el proveedor `Microsoft.Data.Sqlite`
