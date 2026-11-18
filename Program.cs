using System.Text;
using Microsoft.Data.Sqlite;

namespace GestionVideojuegos;

/// <summary>
/// Demostración de ADO.NET con SQLite: crea una base de datos, una tabla,
/// inserta registros con SqliteCommand y los consulta con SqliteDataReader.
/// </summary>
internal class Program
{
    // La cadena de conexión indica el archivo de la base de datos.
    // Si el archivo no existe, SQLite lo crea al abrir la conexión.
    private const string NombreArchivo = "videojuegos.db";
    private const string CadenaConexion = $"Data Source={NombreArchivo}";

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== Registro de videojuegos con ADO.NET y SQLite ===");
        Console.WriteLine();

        try
        {
            // 'using' garantiza que la conexión se cierre y libere sus recursos,
            // incluso si ocurre una excepción dentro del bloque.
            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            Console.WriteLine("[1] Conexión abierta correctamente.");
            Console.WriteLine($"    Base de datos: {Path.GetFullPath(NombreArchivo)}");
            Console.WriteLine();

            CrearTabla(conexion);
            InsertarDatosDeEjemplo(conexion);
            MostrarTodos(conexion);
            MostrarMejorCalificados(conexion, 9.0);
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Error de base de datos: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Programa finalizado. Presiona una tecla para salir...");
        Console.ReadKey();
    }

    /// <summary>
    /// Crea la tabla Videojuegos (se reinicia en cada ejecución para que
    /// la demostración siempre muestre el mismo resultado).
    /// </summary>
    private static void CrearTabla(SqliteConnection conexion)
    {
        using var comando = conexion.CreateCommand();
        comando.CommandText = """
            DROP TABLE IF EXISTS Videojuegos;

            CREATE TABLE Videojuegos (
                Id           INTEGER PRIMARY KEY AUTOINCREMENT,
                Titulo       TEXT    NOT NULL,
                Genero       TEXT    NOT NULL,
                Plataforma   TEXT    NOT NULL,
                Anio         INTEGER NOT NULL,
                Calificacion REAL    NOT NULL
            );
            """;

        // ExecuteNonQuery se usa para instrucciones que no devuelven filas
        // (CREATE, INSERT, UPDATE, DELETE).
        comando.ExecuteNonQuery();

        Console.WriteLine("[2] Tabla 'Videojuegos' creada (6 columnas).");
        Console.WriteLine();
    }

    private static void InsertarDatosDeEjemplo(SqliteConnection conexion)
    {
        Console.WriteLine("[3] Insertando registros...");

        InsertarVideojuego(conexion, "The Legend of Zelda: Breath of the Wild", "Aventura", "Nintendo Switch", 2017, 9.7);
        InsertarVideojuego(conexion, "Hollow Knight", "Metroidvania", "PC", 2017, 9.3);
        InsertarVideojuego(conexion, "Minecraft", "Sandbox", "Multiplataforma", 2011, 9.0);
        InsertarVideojuego(conexion, "Celeste", "Plataformas", "PC", 2018, 8.8);
        InsertarVideojuego(conexion, "Stardew Valley", "Simulación", "Multiplataforma", 2016, 8.9);

        Console.WriteLine();
    }

    /// <summary>
    /// Inserta un videojuego usando una consulta parametrizada.
    /// Los parámetros (@titulo, @genero, ...) evitan la inyección SQL y
    /// se encargan de convertir correctamente los tipos de datos.
    /// </summary>
    private static void InsertarVideojuego(
        SqliteConnection conexion,
        string titulo,
        string genero,
        string plataforma,
        int anio,
        double calificacion)
    {
        using var comando = conexion.CreateCommand();
        comando.CommandText = """
            INSERT INTO Videojuegos (Titulo, Genero, Plataforma, Anio, Calificacion)
            VALUES (@titulo, @genero, @plataforma, @anio, @calificacion);
            """;

        comando.Parameters.AddWithValue("@titulo", titulo);
        comando.Parameters.AddWithValue("@genero", genero);
        comando.Parameters.AddWithValue("@plataforma", plataforma);
        comando.Parameters.AddWithValue("@anio", anio);
        comando.Parameters.AddWithValue("@calificacion", calificacion);

        // ExecuteNonQuery devuelve el número de filas afectadas.
        int filasAfectadas = comando.ExecuteNonQuery();

        Console.WriteLine($"    + {titulo} (filas insertadas: {filasAfectadas})");
    }

    /// <summary>
    /// Consulta todos los registros y los muestra en pantalla con SqliteDataReader.
    /// </summary>
    private static void MostrarTodos(SqliteConnection conexion)
    {
        Console.WriteLine("[4] Consulta: todos los videojuegos registrados");

        using var comando = conexion.CreateCommand();
        comando.CommandText = """
            SELECT Id, Titulo, Genero, Plataforma, Anio, Calificacion
            FROM Videojuegos
            ORDER BY Id;
            """;

        using var lector = comando.ExecuteReader();
        ImprimirTabla(lector);
        Console.WriteLine();
    }

    /// <summary>
    /// Consulta con un parámetro en el WHERE: solo los juegos con una
    /// calificación mayor o igual a la indicada.
    /// </summary>
    private static void MostrarMejorCalificados(SqliteConnection conexion, double calificacionMinima)
    {
        Console.WriteLine($"[5] Consulta: videojuegos con calificación >= {calificacionMinima:0.0}");

        using var comando = conexion.CreateCommand();
        comando.CommandText = """
            SELECT Id, Titulo, Genero, Plataforma, Anio, Calificacion
            FROM Videojuegos
            WHERE Calificacion >= @minima
            ORDER BY Calificacion DESC;
            """;
        comando.Parameters.AddWithValue("@minima", calificacionMinima);

        using var lector = comando.ExecuteReader();
        ImprimirTabla(lector);
    }

    /// <summary>
    /// Recorre el SqliteDataReader fila por fila. Read() avanza al siguiente
    /// registro y devuelve false cuando ya no quedan más filas.
    /// </summary>
    private static void ImprimirTabla(SqliteDataReader lector)
    {
        Console.WriteLine($"    {"Id",-3} {"Título",-42} {"Género",-14} {"Plataforma",-17} {"Año",-5} {"Nota",4}");
        Console.WriteLine($"    {new string('-', 91)}");

        int total = 0;

        while (lector.Read())
        {
            int id = lector.GetInt32(lector.GetOrdinal("Id"));
            string titulo = lector.GetString(lector.GetOrdinal("Titulo"));
            string genero = lector.GetString(lector.GetOrdinal("Genero"));
            string plataforma = lector.GetString(lector.GetOrdinal("Plataforma"));
            int anio = lector.GetInt32(lector.GetOrdinal("Anio"));
            double calificacion = lector.GetDouble(lector.GetOrdinal("Calificacion"));

            Console.WriteLine($"    {id,-3} {titulo,-42} {genero,-14} {plataforma,-17} {anio,-5} {calificacion,4:0.0}");
            total++;
        }

        Console.WriteLine($"    Total de registros: {total}");
    }
}
