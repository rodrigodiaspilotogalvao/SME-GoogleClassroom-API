﻿using SME.GoogleClassroom.Infra;
using SME.GoogleClassroom.Infra.Enumeradores;
using System.Collections.Generic;

namespace SME.GoogleClassroom.Dados
{
    public class RepositorioComponenteCurricularFormacaoCidade : IRepositorioComponenteCurricularFormacaoCidade
    {
        public RepositorioComponenteCurricularFormacaoCidade()
        {}

        public IEnumerable<SalaComponenteModalidadeDto> ObterComponenteCurricular()
        {
            var componentesCurriculares = new List<SalaComponenteModalidadeDto>()
            {
                new SalaComponenteModalidadeDto()
                {
                    SalaVirtual = "EDUCAÇÃO FÍSICA",
                    ComponentesCurricularesIds = "6",
                    ModalidadesIds = "3,5",
                    Modalidade = "EJA Regular",
                    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                    TipoSala = (int)TipoSala.DRE,
                    IncluirAlunoCurso = true
                },

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "GEOGRAFIA",
                //    ComponentesCurricularesIds = "8",
                //    ModalidadesIds = "3,5",
                //    Modalidade = "EJA Regular",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.DRE,
                //    IncluirAlunoCurso = true
                //},                

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "HISTÓRIA",
                //    ComponentesCurricularesIds = "7",
                //    ModalidadesIds = "3,5",
                //    Modalidade = "EJA Regular",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.DRE,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "LÍNGUA INGLESA",
                //    ComponentesCurricularesIds = "9",
                //    ModalidadesIds = "3,5",
                //    Modalidade = "EJA Regular",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.DRE,
                //    IncluirAlunoCurso = true
                //},
                
                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "ARTE",
                //    ComponentesCurricularesIds = "139",
                //    ModalidadesIds = "3,5",
                //    Modalidade = "EJA Regular",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.DRE,
                //    IncluirAlunoCurso = true
                //},
                                
                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "CIÊNCIAS NATURAIS",
                //    ComponentesCurricularesIds = "89",
                //    ModalidadesIds = "3,5",
                //    Modalidade = "EJA Regular",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.DRE,
                //    IncluirAlunoCurso = true
                //},                               

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "LÍNGUA PORTUGUESA",
                //    ComponentesCurricularesIds = "138",
                //    ModalidadesIds = "3,5",
                //    Modalidade = "EJA Regular",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.DRE,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "MATEMÁTICA",
                //    ComponentesCurricularesIds = "2",
                //    ModalidadesIds = "3,5",
                //    Modalidade = "EJA Regular",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.DRE,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "TPA (Informática)",
                //    ComponentesCurricularesIds = "1060",
                //    ModalidadesIds = "3,5",
                //    Modalidade = "EJA Regular",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.DRE,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "SALA DE LEITURA",
                //    ComponentesCurricularesIds = "1061",
                //    ModalidadesIds = "3,5",
                //    Modalidade = "EJA Regular",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.DRE,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "MATEMÁTICA E CIÊNCIAS TURMA 1",
                //    ComponentesCurricularesIds = "2,9",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "108100", "108200", "108300", "108400", "108600", "109100" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "MATEMÁTICA E CIÊNCIAS TURMA 2",
                //    ComponentesCurricularesIds = "2,9",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "108500", "109300", "108900", "109200" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "MATEMÁTICA E CIÊNCIAS TURMA 3",
                //    ComponentesCurricularesIds = "2,9",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "109000", "108800" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "HISTÓRIA E GEOGRAFIA TURMA 1",
                //    ComponentesCurricularesIds = "7,8",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "108100", "108200", "108300", "108400", "108600", "109100" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "HISTÓRIA E GEOGRAFIA TURMA 2",
                //    ComponentesCurricularesIds = "7,8",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "108500", "108700", "109300", "108900", "109200" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "HISTÓRIA E GEOGRAFIA TURMA 3",
                //    ComponentesCurricularesIds = "7,8",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "109000", "108800" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "LÍNGUA PORTUGUESA  E LÍNGUA INGLESA TURMA 1",
                //    ComponentesCurricularesIds = "9,138",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja },
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "108100", "108200","108300","108600","108700","108900","109100" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "LÍNGUA PORTUGUESA  E LÍNGUA INGLESA TURMA 2",
                //    ComponentesCurricularesIds = "9,138",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja },
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "108800","109300","109200" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "LÍNGUA PORTUGUESA  E LÍNGUA INGLESA TURMA 3",
                //    ComponentesCurricularesIds = "9,138",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja },
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "108400", "108500", "109000" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "ARTE E EDUCAÇÃO FÍSICA TURMA 1",
                //    ComponentesCurricularesIds = "139",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja },
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "108100","108300","108200","108400","108600","109000","109100" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "ARTE E EDUCAÇÃO FÍSICA TURMA 2",
                //    ComponentesCurricularesIds = "139",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja },
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "108500","108700","108800","109300","108900","109200" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "ALFABETIZAÇÃO",
                //    ComponentesCurricularesIds = "1113,1114,1247,1248",
                //    ModalidadesIds = "2,3",
                //    Modalidade = "EJA Regular",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.DRE,
                //    IncluirAlunoCurso = true
                //},                

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "ALFABETIZAÇÃO TURMA 1",
                //    ComponentesCurricularesIds = "1247,1248",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja },
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "108200" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "ALFABETIZAÇÃO TURMA 2",
                //    ComponentesCurricularesIds = "1247,1248",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja },
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "109100","108300","108600" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "ALFABETIZAÇÃO TURMA 3",
                //    ComponentesCurricularesIds = "1247,1248",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja },
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "108500","108700","109300","109200" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "ALFABETIZAÇÃO TURMA 4",
                //    ComponentesCurricularesIds = "1247,1248",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja },
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "108100","108900","108800" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "ALFABETIZAÇÃO TURMA 5",
                //    ComponentesCurricularesIds = "1247,1248",
                //    ModalidadesIds = "2",
                //    Modalidade = "CIEJA",
                //    TipoEscola = new int[] {(int)TipoEscola.Cieja },
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.AGRUPAR_DRES,
                //    IncluirAlunoCurso = true,
                //    AgruparPorDres = new string[] { "109000", "108400" },
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "CICLO ALFABETIZAÇÃO",
                //    ComponentesCurricularesIds = "1105,1211,1212,1213",
                //    ModalidadesIds = "5",
                //    Modalidade = "Regular",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.DRE,
                //    AnoTurma = "1,2,3",
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "CICLO INTERDISCIPLINAR – 4º E 5º ANOS",
                //    ComponentesCurricularesIds = "1105,1211,1212,1213",
                //    ModalidadesIds = "5",
                //    Modalidade = "Regular",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.DRE,
                //    AnoTurma = "4,5",
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "EMEBS - Regência de classe",
                //    ComponentesCurricularesIds = "1301,1115,1117",
                //    ModalidadesIds = "13",
                //    Modalidade = "Educação Especial",
                //    TipoEscola = new int[] {(int)TipoEscola.EducacaoEspecial},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.SME,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "EMEBS - Língua Portuguesa",
                //    ComponentesCurricularesIds = "138",
                //    ModalidadesIds = "13",
                //    Modalidade = "Educação Especial",
                //    TipoEscola = new int[] {(int)TipoEscola.EducacaoEspecial},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.SME,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "EMEBS - LIBRAS",
                //    ComponentesCurricularesIds = "218",
                //    ModalidadesIds = "13",
                //    Modalidade = "Educação Especial",
                //    TipoEscola = new int[] {(int)TipoEscola.EducacaoEspecial},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.SME,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "Linguagens e suas tecnologias",
                //    ComponentesCurricularesIds = "138,139,9,6,537,1311,1346",
                //    ModalidadesIds = "6,9",
                //    Modalidade = "Médio",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.SME,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "Matemática e suas tecnologias",
                //    ComponentesCurricularesIds = "2",
                //    ModalidadesIds = "6,9",
                //    Modalidade = "Médio",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.SME,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "Ciências Naturais e suas tecnologias",
                //    ComponentesCurricularesIds = "51,52,53",
                //    ModalidadesIds = "6,9",
                //    Modalidade = "Médio",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.SME,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "Ciências Humanas e sociais aplicadas e suas tecnologias",
                //    ComponentesCurricularesIds = "7,8,98,103",
                //    ModalidadesIds = "6,9",
                //    Modalidade = "Médio",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.ComponenteCurricular,
                //    TipoSala = (int)TipoSala.SME,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "COORDENAÇÃO PEDAGÓGICA",
                //    ComponentesCurricularesIds = null,
                //    ModalidadesIds = null,
                //    Modalidade = "Coordenadores Pedagógico",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental, (int)TipoEscola.Medio, (int)TipoEscola.EducacaoEspecial, (int)TipoEscola.CentroEducacionalUnidicadoEMEF},
                //    TipoConsulta = (int)TipoConsulta.CP,
                //    TipoSala = (int)TipoSala.DRE,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "PAP",
                //    ComponentesCurricularesIds = null,
                //    ModalidadesIds = null,
                //    Modalidade = "Professores designados para a função de PAP",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental},
                //    TipoConsulta = (int)TipoConsulta.PAP,
                //    TipoSala = (int)TipoSala.DRE,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "SRM",
                //    ComponentesCurricularesIds = null,
                //    ModalidadesIds = null,
                //    Modalidade = "PAEE - Professores designados das salas de recursos multifuncionais e colaborativo por DRE - Educação Especial",
                //    TipoEscola = new int[] {(int)TipoEscola.Fundamental},
                //    TipoConsulta = (int)TipoConsulta.PAEE,
                //    TipoSala = (int)TipoSala.DRE,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "ESCOLAS DIRETAS",
                //    ComponentesCurricularesIds = null,
                //    ModalidadesIds = null,
                //    Modalidade = "Coordenadores Pedagógicos atuantes nos CEIs,EMEIs e CEMEIs",
                //    TipoEscola = new int[] {(int)TipoEscola.InfantilEmei, (int)TipoEscola.InfantilCeiDiret, (int)TipoEscola.InfantilCeuEmei, (int)TipoEscola.InfantilCeuCei, (int)TipoEscola.InfantilCemei,(int)TipoEscola.InfantilCruCemei},
                //    TipoConsulta = (int)TipoConsulta.CP,
                //    TipoSala = (int)TipoSala.DRE,
                //    IncluirAlunoCurso = true
                //},

                //new SalaComponenteModalidadeDto()
                //{
                //    SalaVirtual = "ESCOLAS PARCEIRAS",
                //    ComponentesCurricularesIds = null,
                //    ModalidadesIds = null,
                //    Modalidade = "Coordenadores Pedagógicos atuantes nos CEIs parceiros",
                //    TipoEscola = new int[] {(int)TipoEscola.InfantilEmei, (int)TipoEscola.InfantilCeiDiret, (int)TipoEscola.InfantilCeuEmei, (int)TipoEscola.InfantilCeuCei, (int)TipoEscola.InfantilCemei,(int)TipoEscola.InfantilCruCemei},
                //    TipoConsulta = (int)TipoConsulta.CP,
                //    TipoSala = (int)TipoSala.DRE,
                //    AnoTurma = null,
                //    IncluirAlunoCurso = false
                //}
            };

            return componentesCurriculares;
        }

    }
}
