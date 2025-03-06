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
        public async Task<List<ComercializacionExcedentesEnergiaDTO>> GetComercializacionExcedentesEnergia(int mes, int year)
        {
            List<ComercializacionExcedentesEnergiaDTO> comercializacionExcedentesEnergiaDTOs = new List<ComercializacionExcedentesEnergiaDTO>();
            
            using (NpgsqlConnection npgsqlConnection = await ConectPostgreSQLGet.ConnAsync())
            {
                using (NpgsqlCommand cmd = npgsqlConnection.CreateCommand())
                {
                    ApiDAO apiDAO = new ApiDAO();
                    comercializacionExcedentesEnergiaDTOs = await apiDAO.GetComercializacionExcedentesEnergia(cmd, mes, year);
                }
            }
            return comercializacionExcedentesEnergiaDTOs;
        }
        public async Task<List<ExcedentesEnergiaTipoUnoDTO>> ExcedentesEnergiaTipoUnoDAO(int mes, int year, int idservice)
        {
            List<ExcedentesEnergiaTipoUnoDTO> excedentesEnergiaTipoUnoDTOs = new List<ExcedentesEnergiaTipoUnoDTO>();

            using (NpgsqlConnection npgsqlConnection = await ConectPostgreSQLGet.ConnAsync())
            {
                using (NpgsqlCommand cmd = npgsqlConnection.CreateCommand())
                {
                    ApiDAO apiDAO = new ApiDAO();
                    excedentesEnergiaTipoUnoDTOs = await apiDAO.ExcedentesEnergiaTipoUnoDAO(cmd, mes, year, idservice);
                }
            }
            return excedentesEnergiaTipoUnoDTOs;
        }
        public async Task<List<ExcedentesEnergiaTipoDosDTO>> ExcedentesEnergiaTipoDosDAO(int mes, int year, int idService)
        {
            List<ExcedentesEnergiaTipoDosDTO> excedentesEnergiaTipoDosDTOs = new List<ExcedentesEnergiaTipoDosDTO>();

            using (NpgsqlConnection npgsqlConnection = await ConectPostgreSQLGet.ConnAsync())
            {
                using (NpgsqlCommand cmd = npgsqlConnection.CreateCommand())
                {
                    ApiDAO apiDAO = new ApiDAO();
                    excedentesEnergiaTipoDosDTOs = await apiDAO.ExcedentesEnergiaTipoDosDAO(cmd, mes, year, idService);
                }
            }
            return excedentesEnergiaTipoDosDTOs;
        }
    }
}
