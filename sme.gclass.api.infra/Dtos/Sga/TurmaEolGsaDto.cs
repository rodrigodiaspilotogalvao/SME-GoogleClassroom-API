﻿using System.Collections.Generic;

namespace SME.GoogleClassroom.Infra.Dtos.Gsa
{
    public class TurmaEolGsaDto
    {
        public long TurmaCodigo { get; set; }
        public string TurmaAnoModalidade { get; set; }
        public string TumaModalidade { get; set; }
        public string NomeAno { get; set; }
        public string TurmaNome { get; set; }
        public string ComplementoTurma { get; set; }
        public IEnumerable<ComponeteCurricularEolSgaDto> ComponentesCurriculares { get; set; }
    }
}