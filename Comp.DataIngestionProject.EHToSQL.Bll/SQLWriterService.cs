using Comp.DataIngestionProject.DTO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;

namespace Comp.DataIngestionProject.EHToSQL.Bll
{
    public class SQLWriterService
    {
        public (string, string) GetSQLCredentials()
        {
            var configuration = new ConfigurationBuilder().SetBasePath("C:\\Progetti\\Comp.DataIngestionProject").AddJsonFile("appsettings.json");
            var config = configuration.Build();
            string sqlTableName = config.GetSection("SQLTableName").Value;
            string sqlConnectionString = config.GetSection("SQLConnectionString").Value;

            return (sqlTableName, sqlConnectionString);
        }

        public static JObject FlattenObject(object obj)
        {
            // Test
            var json = JsonConvert.SerializeObject(obj);
            var jsonObject = JObject.Parse(json);
            var flatObject = new JObject();
            Flatten(jsonObject, flatObject, null);
            return flatObject;
        }

        public static void Flatten(JToken token, JObject flatObject, string prefix)
        {
            // Test
            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (var property in token.Children<JProperty>())
                    {
                        Flatten(property.Value, flatObject, prefix != null ? $"{prefix}.{property.Name}" : property.Name);
                    }
                    break;
                case JTokenType.Array:
                    int index = 0;
                    foreach (var value in token.Children())
                    {
                        Flatten(value, flatObject, $"{prefix}[{index++}]");
                    }
                    break;
                default:
                    flatObject.Add(new JProperty(prefix, ((JValue)token).Value));
                    break;
            }
        }

        // ...
        public static string GenerateCreateTableSql(string tableName, JObject flatObject, SqlConnection connection)
        {
            // Check if the table already exists
            string checkTableExistsQuery = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
            using (SqlCommand checkTableExistsCommand = new SqlCommand(checkTableExistsQuery, connection))
            {
                connection.Open();
                int tableExists = (int)checkTableExistsCommand.ExecuteScalar();
                connection.Close();

                // If the table exists, return null or an empty string
                if (tableExists > 0)
                {
                    return null;
                }
            }

            // If the table doesn't exist, generate the CREATE TABLE SQL
            var columns = new List<string>();
            foreach (var property in flatObject.Properties())
            {
                //Test

                string sqlType;
                switch (property.Value.Type)
                {
                    case JTokenType.Guid:
                        sqlType = "GUID";
                        break;
                    case JTokenType.String:
                        sqlType = "NVARCHAR(MAX)";
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported type: {property.Value.Type}");
                }
                columns.Add($"[{property.Name}] {sqlType}");
            }

            return $"CREATE TABLE [{tableName}] ({string.Join(", ", columns)});";
        }

        public static string GenerateInsertSql(string tableName, JObject flatObject)
        {
            //Test
            var columnNames = flatObject.Properties().Select(p => $"[{p.Name}]").ToArray();
            var paramNames = flatObject.Properties().Select(p => $"@{p.Name}").ToArray();
            return $"INSERT INTO [{tableName}] ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", paramNames)});";
        }

        public void SQLWriter(LoadDataForServices loadDataForServices)
        {
            loadDataForServices.Src = "SQL Writer";
            (string sqlTableName, string sqlConnectionString) = GetSQLCredentials();
            JObject flatObject = FlattenObject(loadDataForServices);

            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            {
                // Generate and execute CREATE TABLE SQL if necessary
                string createTableSql = GenerateCreateTableSql(sqlTableName, flatObject, connection);
                if (!string.IsNullOrEmpty(createTableSql))
                {
                    using (SqlCommand createTableCommand = new SqlCommand(createTableSql, connection))
                    {
                        connection.Open();
                        createTableCommand.ExecuteNonQuery();
                        connection.Close();
                    }
                }

                // Generate and execute INSERT SQL
                string insertSql = GenerateInsertSql(sqlTableName, flatObject);
                using (SqlCommand insertCommand = new SqlCommand(insertSql, connection))
                {
                    foreach (var property in flatObject.Properties())
                    {
                        insertCommand.Parameters.AddWithValue($"@{property.Name}", property.Value);
                    }
                    connection.Open();
                    insertCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
    }
}