using System;
using System.Data.SQLite;
using System.IO;

namespace FurniTrack.Data
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string dbPath)
        {
            if (string.IsNullOrWhiteSpace(dbPath))
                throw new ArgumentException("Database path cannot be empty.");

            _connectionString = $"Data Source={dbPath};Version=3;";
            Initialize(dbPath);
        }

        public SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            // Important pragmas
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "PRAGMA foreign_keys = ON; PRAGMA journal_mode = WAL;";
                cmd.ExecuteNonQuery();
            }
            return conn;
        }

        private void Initialize(string dbPath)
        {
            string dir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                // Note: The schema application is supposed to happen here if version is 0. 
                // Since this is a test implementation, we assume the DB is already created by Python/SQLite3.
            }
        }
    }
}
