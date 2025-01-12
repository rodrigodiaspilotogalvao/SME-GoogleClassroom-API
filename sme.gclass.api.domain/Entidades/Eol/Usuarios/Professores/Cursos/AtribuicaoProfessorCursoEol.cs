﻿using System;

namespace SME.GoogleClassroom.Dominio
{
    public class AtribuicaoProfessorCursoEol
    {
        public long Rf { get; set; }
        public long TurmaId { get; set; }
        public long ComponenteCurricularId { get; set; }
        public DateTime DataAtribuicao { get; set; }
        public string CdUe { get; set; }
        public int Modalidade { get; set; }

        protected AtribuicaoProfessorCursoEol()
        {
        }

        public AtribuicaoProfessorCursoEol(long rf, long turmaId, long componenteCurricularId, DateTime dataAtribuicao, string cdUe, int modalidade)
        {
            Rf = rf;
            TurmaId = turmaId;
            ComponenteCurricularId = componenteCurricularId;
            DataAtribuicao = dataAtribuicao;
            CdUe = cdUe;
            Modalidade = modalidade;
        }
    }
}