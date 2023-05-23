using SQLite;

namespace AppGestCulture;

public class Constants
{
    public const string DatabaseFilename = "database.db3";

    public static string DatabasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

    public static string TechnicienMatricule = "AAAA1111";

    public static string TechnicienMdp = "caribou";

    public static string Salt = "frgzoiahjf";


    public const SQLiteOpenFlags Flags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;   
}