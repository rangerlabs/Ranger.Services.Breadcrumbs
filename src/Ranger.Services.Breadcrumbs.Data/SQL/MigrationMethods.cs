using System;
using System.IO;
using System.Reflection;

namespace Ranger.Services.Breadcrumbs.Data
{
    public static class MigrationMethods
    {

        public static string UpsertGeofenceStates()
        {
            return GetManifestResourceSql("upsert_geofence_states");
        }

        private static string GetManifestResourceSql(string name)
        {
            string sql;
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (var manifestStream = assembly.GetManifestResourceStream($"Ranger.Services.Breadcrumbs.Data.SQL.{name}.sql"))
            {
                if (manifestStream is null)
                {
                    throw new Exception($@"Failed to file ""Ranger.Services.Breadcrumbs.Data.SQL.{name}.sql"" in manifest files {String.Join(";", assembly.GetManifestResourceNames())} ");
                }
                using (var reader = new StreamReader(manifestStream))
                {
                    sql = reader.ReadToEnd();
                }
                if (String.IsNullOrWhiteSpace(sql))
                {
                    throw new Exception($"The file '{name}.sql' was empty");
                }
            }
            return sql;
        }
    }
}