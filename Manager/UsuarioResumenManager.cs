using FoxFact.Common;
using FoxFact.DAO;
using FoxFact.DTO;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoxFact.Manager
{
    public class ReciboUsuarioManager
    {
        public async Task<ReciboUsuarioDTO> ObtenerReciboUsuario(FoxFact.Controller.CalculateInvoiceRequestDTO request)
        {
            ReciboUsuarioDTO reciboUsuarioDTO = new ReciboUsuarioDTO();

            using (NpgsqlConnection npgsqlConnection = await ConectPostgreSQLGet.ConnAsync())
            {
                using (NpgsqlCommand cmd = npgsqlConnection.CreateCommand())
                {
                    ApiDAO apiDAO = new ApiDAO();

                    // Obtener datos de energía activa
                    EnergiaActivaDTO energiaActiva = await apiDAO.GetEnergiaActiva(cmd, request.Mes, request.Year, request.IdService);
                    reciboUsuarioDTO.EnergiaActiva.Add(energiaActiva);

                    // Obtener datos de comercialización de excedentes
                    ComercializacionExcedentesEnergiaDTO comercializacionExcedentes = await apiDAO.GetComercializacionExcedentesEnergia(cmd, request.Mes, request.Year, request.IdService);
                    reciboUsuarioDTO.ComercializacionExcedentes.Add(comercializacionExcedentes);

                    // Obtener datos de excedentes tipo uno
                    ExcedentesEnergiaTipoUnoDTO excedentesTipoUno = await apiDAO.ExcedentesEnergiaTipoUnoDAO(cmd, request.Mes, request.Year, request.IdService);
                    reciboUsuarioDTO.ExcedentesTipoUno.Add(excedentesTipoUno);

                    // Obtener datos de excedentes tipo dos
                    ExcedentesEnergiaTipoDosDTO excedentesTipoDos = await apiDAO.ExcedentesEnergiaTipoDosDAO(cmd, request.Mes, request.Year, request.IdService);
                    reciboUsuarioDTO.ExcedentesTipoDos.Add(excedentesTipoDos);

                    // Calcular el total de la cuenta
                    reciboUsuarioDTO.TotalPagar = energiaActiva.Monto +
                                                  comercializacionExcedentes.Monto +
                                                  excedentesTipoUno.Monto +
                                                  excedentesTipoDos.Monto;
                }
            }

            return reciboUsuarioDTO;
        }
    }
}
