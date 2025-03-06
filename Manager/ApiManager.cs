using FoxFact.Common;
using FoxFact.DAO;
using FoxFact.DTO;
using Hangfire.PostgreSql;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxFact.Manager
{
    class ApiManager
    {
        public async Task<List<EnergiaActivaDTO>> GetEnergiaActiva(int year, int mes)
        {
            List<EnergiaActivaDTO> energiaActivaDTOs = new List<EnergiaActivaDTO>();

            using (NpgsqlConnection npgsqlConnection = await ConectPostgreSQLGet.ConnAsync())
            {
                using (NpgsqlCommand cmd = npgsqlConnection.CreateCommand())
                {
                    ApiDAO apiDAO = new ApiDAO();
                    energiaActivaDTOs = await apiDAO.GetEnergiaActiva(cmd, mes, year);
                }
            }

            return energiaActivaDTOs;
        }


    }
}
