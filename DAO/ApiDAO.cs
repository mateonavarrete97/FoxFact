﻿using FoxFact.DTO;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxFact.DAO
{
    class ApiDAO
    {
        public async Task<List<EnergiaActivaDTO>> GetEnergiaActiva(NpgsqlCommand cmd,int mes, int year)
        {
            List<EnergiaActivaDTO> energiaActivaDTOs = new List<EnergiaActivaDTO>();

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();

            string query = @"SELECT 
                        SUM(c.value) AS cantidad_EA, 
                        t.cu AS tarifa_EA
                     FROM consumption c
                     INNER JOIN records r ON c.id_record = r.id_record
                     INNER JOIN services s ON s.id_service = r.id_service
                     INNER JOIN tariffs t ON t.id_market = s.id_market
                     WHERE EXTRACT(MONTH FROM r.record_timestamp) = @mes
                       AND EXTRACT(YEAR FROM r.record_timestamp) = @year
                     GROUP BY t.cu;";
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@mes", mes);
            cmd.Parameters.AddWithValue("@year", year);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    EnergiaActivaDTO energiaActivaDTO = new EnergiaActivaDTO
                    {
                        CantidadEA = reader["cantidad_EA"] != DBNull.Value ? Convert.ToDecimal(reader["cantidad_EA"]) : 0,
                        TarifaEA = reader["tarifa_EA"] != DBNull.Value ? Convert.ToDecimal(reader["tarifa_EA"]) : 0
                    };
                    energiaActivaDTOs.Add(energiaActivaDTO);
                }
            }
            return energiaActivaDTOs;
        }

    }
}
