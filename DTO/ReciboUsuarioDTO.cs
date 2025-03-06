namespace FoxFact.DTO
{
    public class ReciboUsuarioDTO
    {
        public List<EnergiaActivaDTO> EnergiaActiva { get; set; }
        public List<ComercializacionExcedentesEnergiaDTO> ComercializacionExcedentes { get; set; }
        public List<ExcedentesEnergiaTipoUnoDTO> ExcedentesTipoUno { get; set; }
        public List<ExcedentesEnergiaTipoDosDTO> ExcedentesTipoDos { get; set; }
        public decimal TotalPagar { get; set; }

        public ReciboUsuarioDTO()
        {
            EnergiaActiva = new List<EnergiaActivaDTO>();
            ComercializacionExcedentes = new List<ComercializacionExcedentesEnergiaDTO>();
            ExcedentesTipoUno = new List<ExcedentesEnergiaTipoUnoDTO>();
            ExcedentesTipoDos = new List<ExcedentesEnergiaTipoDosDTO>();
            TotalPagar = 0;
        }
    }
}

