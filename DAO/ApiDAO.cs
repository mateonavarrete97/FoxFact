using FoxFact.DTO;
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
        public async Task<List<ComercializacionExcedentesEnergiaDTO>> GetComercializacionExcedentesEnergia(NpgsqlCommand cmd, int mes, int year)
        {
            List<ComercializacionExcedentesEnergiaDTO> comercializacionExcedentesEnergiaDTOs = new List<ComercializacionExcedentesEnergiaDTO>();

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();

            string query = @"SELECT 
                                SUM(i.value) AS cantidad_EC, 
                                t.c AS tarifa_EC
                            FROM injection i
                            INNER JOIN records r ON r.id_record = i.id_record
                            INNER JOIN services s ON r.id_service = s.id_service
                            INNER JOIN tariffs t ON s.id_market = t.id_market
                            WHERE EXTRACT(MONTH FROM r.record_timestamp) = @mes
                                AND EXTRACT(YEAR FROM r.record_timestamp) = @year
                            GROUP BY t.c;";

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@mes", mes);
            cmd.Parameters.AddWithValue("@year", year);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    ComercializacionExcedentesEnergiaDTO comercializacionExcedentesEnergiaDTO = new ComercializacionExcedentesEnergiaDTO();

                    comercializacionExcedentesEnergiaDTO.CantidadEC = reader["cantidad_EC"] != DBNull.Value ? Convert.ToDecimal(reader["cantidad_EC"]) : 0;
                    comercializacionExcedentesEnergiaDTO.TarifaEC = reader["tarifa_EC"] != DBNull.Value ? Convert.ToDecimal(reader["tarifa_EC"]) : 0;

                    comercializacionExcedentesEnergiaDTOs.Add(comercializacionExcedentesEnergiaDTO);
                }
            }
            return comercializacionExcedentesEnergiaDTOs;
        }

        public async Task<List<ExcedentesEnergiaTipoUnoDTO>> ExcedentesEnergiaTipoUnoDAO(NpgsqlCommand cmd, int mes, int year, int idService)
        {
            List<ExcedentesEnergiaTipoUnoDTO> excedentesEnergiaTipoUnoDTOs = new List<ExcedentesEnergiaTipoUnoDTO>();

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();

            string query = @"WITH monthly_data AS (
                            SELECT 
                            r.id_service,
                            SUM(i.value) AS total_injection,
                            SUM(c.value) AS total_consumption
                            FROM records r
                            LEFT JOIN injection i ON r.id_record = i.id_record
                            LEFT JOIN consumption c ON r.id_record = c.id_record
                            WHERE EXTRACT(YEAR FROM r.record_timestamp) = @year
                            AND EXTRACT(MONTH FROM r.record_timestamp) = @mes
                            AND r.id_service = @idservice
                            GROUP BY r.id_service
                            )
                            SELECT 
                            md.id_service,
                            LEAST(md.total_injection, md.total_consumption) AS EE1,
                            -t.cu AS tarifa_cu_negativa
                            FROM monthly_data md
                            JOIN services s ON md.id_service = s.id_service
                            JOIN tariffs t ON s.id_market = t.id_market 
                            AND s.cdi = t.cdi 
                            AND s.voltage_level = t.voltage_level;";

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@mes", mes);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@idservice", idService);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    ExcedentesEnergiaTipoUnoDTO excedentesEnergiaTipoUnoDTO = new ExcedentesEnergiaTipoUnoDTO();

                    excedentesEnergiaTipoUnoDTO.IdService = reader["id_service"] != DBNull.Value ? Convert.ToInt32(reader["id_service"]) : 0;
                    excedentesEnergiaTipoUnoDTO.EE1 = reader["EE1"] != DBNull.Value ? Convert.ToDecimal(reader["EE1"]) : 0;
                    excedentesEnergiaTipoUnoDTO.TarifaCUNegativa = reader["tarifa_cu_negativa"] != DBNull.Value ? Convert.ToDecimal(reader["tarifa_cu_negativa"]) : 0;

                    excedentesEnergiaTipoUnoDTOs.Add(excedentesEnergiaTipoUnoDTO);
                }
            }
            return excedentesEnergiaTipoUnoDTOs;
        }
        public async Task<List<ExcedentesEnergiaTipoDosDTO>> ExcedentesEnergiaTipoDosDAO(NpgsqlCommand cmd, int mes, int year, int idService)
        {
            List<ExcedentesEnergiaTipoDosDTO> excedentesEnergiaTipoDosDTOs = new List<ExcedentesEnergiaTipoDosDTO>();

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();

            string query = @"WITH hourly_data AS (
                            SELECT 
                            r.id_service,
                            r.record_timestamp,
                            COALESCE(i.value, 0) AS injection_value,
                            COALESCE(c.value, 0) AS consumption_value,
                            x.value AS hourly_tariff
                            FROM records r
                            LEFT JOIN injection i ON r.id_record = i.id_record
                            LEFT JOIN consumption c ON r.id_record = c.id_record
                            LEFT JOIN xm_data_hourly_per_agent x ON r.record_timestamp = x.record_timestamp
                            WHERE EXTRACT(YEAR FROM r.record_timestamp) = @year
                            AND EXTRACT(MONTH FROM r.record_timestamp) = @mes
                            AND r.id_service = @idservice
                            ),
                            excess_energy AS (
                            SELECT 
                            id_service,
                            record_timestamp,
                            CASE 
                            WHEN injection_value > consumption_value THEN injection_value - consumption_value
                            ELSE 0 
                            END AS EE2_hourly,
                            hourly_tariff
                            FROM hourly_data
                            )
                            SELECT 
                            id_service,
                            SUM(EE2_hourly) AS total_EE2,
                            SUM(EE2_hourly * hourly_tariff) AS total_tariff_cost
                            FROM excess_energy
                            GROUP BY id_service;
                            ";

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@mes", mes);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@idservice", idService);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    ExcedentesEnergiaTipoDosDTO excedentesEnergiaTipoDosDTO = new ExcedentesEnergiaTipoDosDTO();

                    excedentesEnergiaTipoDosDTO.IdService = reader["id_service"] != DBNull.Value ? Convert.ToInt32(reader["id_service"]) : 0;
                    excedentesEnergiaTipoDosDTO.EE2 = reader["total_EE2"] != DBNull.Value ? Convert.ToDecimal(reader["total_EE2"]) : 0;
                    excedentesEnergiaTipoDosDTO.TotalTariffCost = reader["total_tariff_cost"] != DBNull.Value ? Convert.ToDecimal(reader["total_tariff_cost"]) : 0;

                    excedentesEnergiaTipoDosDTOs.Add(excedentesEnergiaTipoDosDTO);
                }
            }
            return excedentesEnergiaTipoDosDTOs;
        }
    }
}
