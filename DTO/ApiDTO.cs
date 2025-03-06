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
    }
    public class ComercializacionExcedentesEnergiaDTO
    {
        public decimal CantidadEC { get; set; }
        public decimal TarifaEC { get; set; }
    }
    public class ExcedentesEnergiaTipoUnoDTO
    {
        public int IdService { get; set; }
        public decimal EE1 { get; set;}
        public decimal tarifaCUNegativa { get; set; }
    }
}
