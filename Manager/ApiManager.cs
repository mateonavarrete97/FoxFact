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
        public async Task<EnergiaActivaDTO> GetEnergiaActiva(int year, int mes, int idService)
        {
            EnergiaActivaDTO energiaActivaDTO = new EnergiaActivaDTO();

            using (NpgsqlConnection npgsqlConnection = await ConectPostgreSQLGet.ConnAsync())
            {
                using (NpgsqlCommand cmd = npgsqlConnection.CreateCommand())
                {
                    ApiDAO apiDAO = new ApiDAO();
                    energiaActivaDTO = await apiDAO.GetEnergiaActiva(cmd, mes, year, idService);
                }
            }
            if (energiaActivaDTO != null)
            {
                energiaActivaDTO.Monto = energiaActivaDTO.CantidadEA * energiaActivaDTO.TarifaEA;
            }

            return energiaActivaDTO;
        }
        public async Task<ComercializacionExcedentesEnergiaDTO> GetComercializacionExcedentesEnergia(int mes, int year, int idService)
        {
            using (NpgsqlConnection npgsqlConnection = await ConectPostgreSQLGet.ConnAsync())
            {
                using (NpgsqlCommand cmd = npgsqlConnection.CreateCommand())
                {
                    ApiDAO apiDAO = new ApiDAO();
                    return await apiDAO.GetComercializacionExcedentesEnergia(cmd, mes, year, idService);
                }
            }
        }

        public async Task<ExcedentesEnergiaTipoUnoDTO> ExcedentesEnergiaTipoUnoDAO(int mes, int year, int idservice)
        {
            using (NpgsqlConnection npgsqlConnection = await ConectPostgreSQLGet.ConnAsync())
            {
                using (NpgsqlCommand cmd = npgsqlConnection.CreateCommand())
                {
                    ApiDAO apiDAO = new ApiDAO();
                    return await apiDAO.ExcedentesEnergiaTipoUnoDAO(cmd, mes, year, idservice);
                }
            }
        }

        public async Task<ExcedentesEnergiaTipoDosDTO> ExcedentesEnergiaTipoDosDAO(int mes, int year, int idService)
        {
            using (NpgsqlConnection npgsqlConnection = await ConectPostgreSQLGet.ConnAsync())
            {
                using (NpgsqlCommand cmd = npgsqlConnection.CreateCommand())
                {
                    ApiDAO apiDAO = new ApiDAO();
                    return await apiDAO.ExcedentesEnergiaTipoDosDAO(cmd, mes, year, idService);
                }
            }
        }

    }
}
