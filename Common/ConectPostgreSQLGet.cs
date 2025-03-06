using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace FoxFact.Common
{
    class ConectPostgreSQLGet
    {

        public static async Task<NpgsqlConnection> ConnAsync()
        {
            try
            {
                string conStr = System.Environment.GetEnvironmentVariable("ConnectionStrings:PostgreSqlConnection", EnvironmentVariableTarget.Process);

                if (string.IsNullOrEmpty(conStr))
                {
                    conStr = "Host=localhost; Port=5432; Database=foxtrack_db; Username=postgres; Password=Mateo123*; SSL Mode=Prefer;";

                }

                NpgsqlConnection connection = new NpgsqlConnection(conStr);
                await connection.OpenAsync();

                return connection;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
