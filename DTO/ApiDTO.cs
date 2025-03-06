using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxFact.DTO
{
    public class EnergiaActivaDTO
    {
        public decimal CantidadEA { get; set; }
        public decimal TarifaEA { get; set; }
        public decimal Monto { get; set; }
    }
    public class ComercializacionExcedentesEnergiaDTO
    {
        public decimal CantidadEC { get; set; }
        public decimal TarifaEC { get; set; }
        public decimal Monto { get; set; }
    }
    public class ExcedentesEnergiaTipoUnoDTO
    {
        public int IdService { get; set; }
        public decimal EE1 { get; set;}
        public decimal TarifaCUNegativa { get; set; }
        public decimal Monto { get; set; }
    }
    public class ExcedentesEnergiaTipoDosDTO
    {
        public int IdService { get; set; }
        public decimal EE2 { get; set; }
        public decimal TotalTariffCost { get; set; }
        public decimal Monto { get; set; }
    }
}
