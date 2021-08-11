﻿using Dapper;
using SME.GoogleClassroom.Dados.Help;
using SME.GoogleClassroom.Dados.Interfaces;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Dados
{
    public class RepositorioFuncionarioEol : RepositorioEol, IRepositorioFuncionarioEol
    {
        public RepositorioFuncionarioEol(ConnectionStrings connectionStrings)
            : base(connectionStrings)
        {
        }

        public async Task<PaginacaoResultadoDto<FuncionarioEol>> ObterFuncionariosParaInclusaoAsync(DateTime dataReferencia, Paginacao paginacao, string rf, ParametrosCargaInicialDto parametrosCargaInicialDto)
        {
            var aplicarPaginacao = paginacao.QuantidadeRegistros > 0;
            var query = MontaQueryFuncionariosParaInclusao(aplicarPaginacao, dataReferencia, rf, parametrosCargaInicialDto);
            var parametros = new
            {
                dataReferencia = dataReferencia.Date,
                paginacao.QuantidadeRegistros,
                paginacao.QuantidadeRegistrosIgnorados,
                rf,
				parametrosCargaInicialDto.TiposUes,
				parametrosCargaInicialDto.Ues,
				parametrosCargaInicialDto.Turmas
			};

			using var conn = ObterConexao();
			using var multi = await conn.QueryMultipleAsync(query, parametros);

            var retorno = new PaginacaoResultadoDto<FuncionarioEol>();

            retorno.Items = multi.Read<FuncionarioEol>();
            retorno.TotalRegistros = multi.ReadFirst<int>();
            retorno.TotalPaginas = aplicarPaginacao ? (int)Math.Ceiling((double)retorno.TotalRegistros / paginacao.QuantidadeRegistros) : 1;

            return retorno;
        }

        public async Task<FuncionarioEol> ObterFuncionarioParaTratamentoDeErroAsync(long rf, int anoLetivo, ParametrosCargaInicialDto parametrosCargaInicialDto)
        {
            var query = MontaQueryFuncionariosParaInclusao(false, null, rf.ToString(), parametrosCargaInicialDto);
            var parametros = new
            {
                anoLetivo = anoLetivo,
                rf,
				parametrosCargaInicialDto.TiposUes,
				parametrosCargaInicialDto.Ues,
				parametrosCargaInicialDto.Turmas
			};

            using var conn = ObterConexao();
            return await conn.QuerySingleOrDefaultAsync<FuncionarioEol>(query, parametros);
        }

        public async Task<IEnumerable<FuncionarioCursoEol>> ObterCursosDoFuncionarioParaIncluirAsync(long? rf, int anoLetivo, ParametrosCargaInicialDto parametrosCargaInicialDto)
        {
            using var conn = ObterConexao();

			const string queryFuncionariosPorCargoFixo = @"
				DECLARE @cargoCP AS INT = 3379;
				DECLARE @cargoAD AS INT = 3085;
				DECLARE @cargoDiretor AS INT = 3360;
				DECLARE @tipoFuncaoPAP AS INT = 30;
				DECLARE @tipoFuncaoPAEE AS INT = 6;
				DECLARE @tipoFuncaoCIEJAASSISTPED AS INT = 42;
				DECLARE @tipoFuncaoCIEJAASSISTCOORD AS INT = 43;
				DECLARE @tipoFuncaoCIEJACOORD AS INT = 44; 

				-- 1. Busca os funcionários por cargo fixo
				IF OBJECT_ID('tempdb..#tempServidorCargosBase') IS NOT NULL
					DROP TABLE #tempServidorCargosBase;
				SELECT
					serv.cd_registro_funcional AS Rf,
					esc.cd_escola AS CdUe,
					cbc.cd_cargo AS CdCagoFuncao,
					cbc.cd_cargo_base_servidor
				INTO #tempServidorCargosBase
				FROM
					v_servidor_cotic serv (NOLOCK)
				INNER JOIN
					v_cargo_base_cotic cbc (NOLOCK)
					ON serv.cd_servidor = cbc.cd_servidor
				INNER JOIN
					lotacao_servidor ls (NOLOCK)
					ON ls.cd_cargo_base_servidor = cbc.cd_cargo_base_servidor
				INNER JOIN
					escola esc
					ON ls.cd_unidade_educacao = esc.cd_escola
				WHERE
					serv.cd_registro_funcional = @rf
					AND dt_fim_nomeacao IS NULL
					AND (ls.dt_fim IS NULL OR ls.dt_fim > GETDATE())";
			
			var queryBuscafuncionarioPorCargoFixoStringBuilder = new StringBuilder(queryFuncionariosPorCargoFixo);

			queryBuscafuncionarioPorCargoFixoStringBuilder.AdicionarCondicaoIn(parametrosCargaInicialDto.TiposUes, "esc.tp_escola", nameof(parametrosCargaInicialDto.TiposUes));
			queryBuscafuncionarioPorCargoFixoStringBuilder.AdicionarCondicaoIn(parametrosCargaInicialDto.Ues, "te.cd_escola", nameof(parametrosCargaInicialDto.TiposUes));
			queryBuscafuncionarioPorCargoFixoStringBuilder.AdicionarCondicaoIn(parametrosCargaInicialDto.Turmas, "te.cd_turma_escola", nameof(parametrosCargaInicialDto.TiposUes));
			queryBuscafuncionarioPorCargoFixoStringBuilder.Append(";");

			const string queryFuncionariosPorCargoSobrepostoFixo = @"
				-- 2. Busca os funcionários por cargo sobreposto fixo

				IF OBJECT_ID('tempdb..#tempServidorCargosSobrepostos') IS NOT NULL
					DROP TABLE #tempServidorCargosSobrepostos;
				SELECT
					serv.cd_registro_funcional AS Rf, 
					css.cd_unidade_local_servico AS CdUe,
					css.cd_cargo AS CdCagoFuncao,
					cbc.cd_cargo_base_servidor
				INTO #tempServidorCargosSobrepostos
				FROM
					v_servidor_cotic serv (NOLOCK)
				INNER JOIN
					v_cargo_base_cotic cbc (NOLOCK)
					ON serv.cd_servidor = cbc.cd_servidor
				INNER JOIN
					cargo_sobreposto_servidor css (NOLOCK)
					ON cbc.cd_cargo_base_servidor = css.cd_cargo_base_servidor
				INNER JOIN
					escola esc 
					ON css.cd_unidade_local_servico = esc.cd_escola
				WHERE
					serv.cd_registro_funcional = @rf
					AND (css.dt_fim_cargo_sobreposto IS NULL OR css.dt_fim_cargo_sobreposto > GETDATE())";
			
			var queryBuscafuncionarioPorCargoSobrepostoStringBuilder = new StringBuilder(queryFuncionariosPorCargoSobrepostoFixo);
			queryBuscafuncionarioPorCargoSobrepostoStringBuilder.AdicionarCondicaoIn(parametrosCargaInicialDto.TiposUes, "esc.tp_escola", nameof(parametrosCargaInicialDto.TiposUes));
			queryBuscafuncionarioPorCargoSobrepostoStringBuilder.AdicionarCondicaoIn(parametrosCargaInicialDto.Ues, "te.cd_escola", nameof(parametrosCargaInicialDto.TiposUes));
			queryBuscafuncionarioPorCargoSobrepostoStringBuilder.AdicionarCondicaoIn(parametrosCargaInicialDto.Turmas, "te.cd_turma_escola", nameof(parametrosCargaInicialDto.TiposUes));
			queryBuscafuncionarioPorCargoSobrepostoStringBuilder.Append(";");

			const string queryFuncionariosPorFuncionarios = @"
				-- 3. Busca os funcionários por função

				IF OBJECT_ID('tempdb..#tempServidorFuncao') IS NOT NULL
					DROP TABLE #tempServidorFuncao;
				SELECT
					serv.cd_registro_funcional AS Rf, 
					esc.cd_escola AS CdUe,
					facs.cd_tipo_funcao AS CdCagoFuncao,
					cbc.cd_cargo_base_servidor
				INTO #tempServidorFuncao
				FROM
					v_servidor_cotic serv (NOLOCK)
				INNER JOIN
					v_cargo_base_cotic cbc (NOLOCK)
					ON serv.cd_servidor = cbc.cd_servidor
				INNER JOIN
					funcao_atividade_cargo_servidor facs (NOLOCK)
					ON cbc.cd_cargo_base_servidor = facs.cd_cargo_base_servidor
				INNER JOIN
					escola esc 
					ON facs.cd_unidade_local_servico = esc.cd_escola
				WHERE
					serv.cd_registro_funcional = @rf
					AND (facs.dt_fim_funcao_atividade IS NULL OR facs.dt_fim_funcao_atividade > GETDATE())
					AND dt_fim_nomeacao IS NULL";
			
			var queryBuscafuncionarioPorFuncaoStringBuilder = new StringBuilder(queryFuncionariosPorFuncionarios);
			
			queryBuscafuncionarioPorFuncaoStringBuilder.AdicionarCondicaoIn(parametrosCargaInicialDto.TiposUes, "esc.tp_escola", nameof(parametrosCargaInicialDto.TiposUes));
			queryBuscafuncionarioPorFuncaoStringBuilder.AdicionarCondicaoIn(parametrosCargaInicialDto.Ues, "te.cd_escola", nameof(parametrosCargaInicialDto.Ues));
			queryBuscafuncionarioPorFuncaoStringBuilder.AdicionarCondicaoIn(parametrosCargaInicialDto.Turmas, "te.cd_turma_escola", nameof(parametrosCargaInicialDto.Turmas));
			queryBuscafuncionarioPorFuncaoStringBuilder.Append(";");

			const string queryMain = @"
				IF OBJECT_ID('tempdb..#tempServidorCargos') IS NOT NULL
					DROP TABLE #tempServidorCargos;
				SELECT
					base.Rf,
					CASE
						WHEN NOT sobreposto.CdCagoFuncao IS NULL THEN sobreposto.CdCagoFuncao
						WHEN NOT funcao.CdCagoFuncao IS NULL THEN funcao.CdCagoFuncao
						ELSE base.CdCagoFuncao
					END AS CdCagoFuncao,
					CASE
						WHEN NOT sobreposto.CdUe IS NULL THEN sobreposto.CdUe
						WHEN NOT funcao.CdUe IS NULL THEN funcao.CdUe
						ELSE base.CdUe
					END AS CdUe
				INTO #tempServidorCargos
				FROM
					#tempServidorCargosBase base
				LEFT JOIN
					#tempServidorCargosSobrepostos sobreposto
					ON base.cd_cargo_base_servidor = sobreposto.cd_cargo_base_servidor
				LEFT JOIN
					#tempServidorFuncao funcao
					ON base.cd_cargo_base_servidor = funcao.cd_cargo_base_servidor;

				IF OBJECT_ID('tempdb..#tempServidorCargoFinal') IS NOT NULL
					DROP TABLE #tempServidorCargoFinal;
				SELECT
					*
				INTO #tempServidorCargoFinal
				FROM
					#tempServidorCargos
				WHERE
					CdCagoFuncao IN (@cargoCP, @cargoAD, @cargoDiretor, @tipoFuncaoPAP, @tipoFuncaoPAEE, @tipoFuncaoCIEJAASSISTPED, @tipoFuncaoCIEJAASSISTCOORD, @tipoFuncaoCIEJACOORD);

				IF OBJECT_ID('tempdb..#tempTurmasComponentesRegulares') IS NOT NULL 
					DROP TABLE #tempTurmasComponentesRegulares
				SELECT
					DISTINCT
					CASE
						WHEN etapa_ensino.cd_etapa_ensino = 1 THEN 512
					ELSE
						cc.cd_componente_curricular
					END ComponenteCurricularId,
					te.cd_turma_escola TurmaId,
					ue.cd_unidade_educacao AS CdUe	
				INTO #tempTurmasComponentesRegulares
				FROM
					#tempServidorCargoFinal temp
				INNER JOIN
					turma_escola te (NOLOCK)
					ON temp.CdUe = te.cd_escola
				INNER JOIN
					escola esc (NOLOCK) 
					ON te.cd_escola = esc.cd_escola
				INNER JOIN
					v_cadastro_unidade_educacao ue (NOLOCK) 
					ON ue.cd_unidade_educacao = esc.cd_escola
				INNER JOIN
					tipo_escola tpe (NOLOCK) 
					ON esc.tp_escola = tpe.tp_escola
				INNER JOIN
					unidade_administrativa dre (NOLOCK) 
					ON ue.cd_unidade_administrativa_referencia = dre.cd_unidade_administrativa	
				INNER JOIN
					serie_turma_escola (NOLOCK) 
					ON serie_turma_escola.cd_turma_escola = te.cd_turma_escola
				INNER JOIN
					serie_turma_grade (NOLOCK) 
					ON serie_turma_grade.cd_turma_escola = serie_turma_escola.cd_turma_escola AND serie_turma_grade.dt_fim IS NULL
				INNER JOIN
					escola_grade (NOLOCK) 
					ON serie_turma_grade.cd_escola_grade = escola_grade.cd_escola_grade
				INNER JOIN
					grade (NOLOCK) 
					ON escola_grade.cd_grade = grade.cd_grade
				INNER JOIN
					grade_componente_curricular gcc (NOLOCK) 
					ON gcc.cd_grade = grade.cd_grade
				INNER JOIN
					componente_curricular cc (NOLOCK) 
					ON cc.cd_componente_curricular = gcc.cd_componente_curricular AND cc.dt_cancelamento IS NULL
				INNER JOIN
					serie_ensino (NOLOCK) 
					ON grade.cd_serie_ensino = serie_ensino.cd_serie_ensino
				INNER JOIN
					etapa_ensino (NOLOCK) 
					ON serie_ensino.cd_etapa_ensino = etapa_ensino.cd_etapa_ensino
				WHERE  
					te.an_letivo = @anoLetivo
					AND	  te.st_turma_escola in ('O', 'A', 'C')								
					AND   (serie_turma_grade.dt_fim IS NULL OR serie_turma_grade.dt_fim >= GETDATE())";
				
	        var queryMainStringBuilder = new StringBuilder(queryMain);


			queryMainStringBuilder.AdicionarCondicaoIn(parametrosCargaInicialDto.TiposUes, "esc.tp_escola", nameof(parametrosCargaInicialDto.TiposUes));
			queryMainStringBuilder.AdicionarCondicaoIn(parametrosCargaInicialDto.Ues, "te.cd_escola", nameof(parametrosCargaInicialDto.Ues));
			queryMainStringBuilder.AdicionarCondicaoIn(parametrosCargaInicialDto.Turmas, "te.cd_turma_escola", nameof(parametrosCargaInicialDto.Turmas));
			queryMainStringBuilder.Append(";");
				
	        var query = $@"
					{queryBuscafuncionarioPorCargoFixoStringBuilder}
					{queryBuscafuncionarioPorCargoSobrepostoStringBuilder}
					{queryBuscafuncionarioPorFuncaoStringBuilder}
					{queryMainStringBuilder}
			SELECT
				servidor.Rf,
				cursos.TurmaId,
				cursos.ComponenteCurricularId,
				cursos.CdUe AS UeCodigo
			FROM
				#tempServidorCargoFinal servidor
			INNER JOIN
				#tempTurmasComponentesRegulares cursos
				ON servidor.CdUe = cursos.CdUe;";
	

			return await conn.QueryAsync<FuncionarioCursoEol>(query, new { 
				rf, 
				anoLetivo,
				parametrosCargaInicialDto.TiposUes,
				parametrosCargaInicialDto.Ues,
				parametrosCargaInicialDto.Turmas,
			});
        }

        private static string MontaQueryFuncionariosParaInclusao(bool aplicarPaginacao, DateTime? dataReferencia, string rf, ParametrosCargaInicialDto parametrosCargaInicialDto)
        {
			// 1.Cargos base Fixos
			var query = new StringBuilder(@$"
                DECLARE @cargoCP AS INT = 3379;
				DECLARE @cargoAD AS INT = 3085;
				DECLARE @cargoDiretor AS INT = 3360;
				DECLARE @cargoSupervisor AS INT = 3352;
				DECLARE @cargoSupervisorTecnico433 AS INT = 433;
				DECLARE @cargoSupervisorTecnico434 AS INT = 434;
				DECLARE @cargoATE AS INT = 4906;
				DECLARE @cargoAuxDesenvolvimentoInfantil AS INT = (SELECT cd_cargo FROM cargo (NOLOCK) WHERE dc_cargo = 'AUXILIAR DE DESENVOLVIMENTO INFANTIL');

				-- 1.Cargos base Fixos
				IF OBJECT_ID('tempdb..#tempCargosBaseFuncionarios_Fixos') IS NOT NULL
					DROP TABLE #tempCargosBaseFuncionarios_Fixos;
				SELECT
					cbc.cd_cargo_base_servidor,
					cbc.cd_servidor,
					cbc.cd_cargo,
					CASE
						WHEN cbc.cd_cargo = @cargoSupervisorTecnico433 OR cbc.cd_cargo = @cargoSupervisorTecnico434 THEN 1
						WHEN cbc.cd_cargo = @cargoSupervisor THEN 2
						WHEN cbc.cd_cargo = @cargoDiretor THEN 3
						WHEN cbc.cd_cargo = @cargoAD THEN 4
						WHEN cbc.cd_cargo = @cargoCP THEN 5
						WHEN cbc.cd_cargo = @cargoATE THEN 8
						WHEN cbc.cd_cargo = @cargoAuxDesenvolvimentoInfantil THEN 9
					END AS prioridade
				INTO #tempCargosBaseFuncionarios_Fixos
				FROM
					v_cargo_base_cotic cbc (NOLOCK)
				INNER JOIN
					lotacao_servidor ls (NOLOCK)
					ON ls.cd_cargo_base_servidor = cbc.cd_cargo_base_servidor
				INNER JOIN
					escola esc  (NOLOCK)
					ON ls.cd_unidade_educacao = esc.cd_escola
				INNER JOIN
					turma_escola te (NOLOCK)
					ON te.cd_escola = esc.cd_escola	
				WHERE
					cbc.cd_cargo IN (@cargoCP, @cargoAD, @cargoDiretor, @cargoSupervisor, @cargoSupervisorTecnico433, @cargoSupervisorTecnico434, @cargoATE, @cargoAuxDesenvolvimentoInfantil)
					AND (dt_fim_nomeacao IS NULL OR dt_fim_nomeacao > GETDATE())
					{(dataReferencia.HasValue ? "AND cbc.dt_nomeacao >= @dataReferencia " : " ")} ");

			query.AdicionarCondicaoIn(parametrosCargaInicialDto.TiposUes, "esc.tp_escola", nameof(parametrosCargaInicialDto.TiposUes));
			query.AdicionarCondicaoIn(parametrosCargaInicialDto.Ues, "te.cd_escola", nameof(parametrosCargaInicialDto.Ues));
			query.AdicionarCondicaoIn(parametrosCargaInicialDto.Turmas, "te.cd_turma_escola", nameof(parametrosCargaInicialDto.Turmas));
			query.AppendLine(";");

			// 2. Cargos sobrepostos fixos
			query.AppendLine(@$"-- 2. Cargos sobrepostos fixos
				IF OBJECT_ID('tempdb..#tempCargosSobrepostosFuncionarios_Fixos') IS NOT NULL
					DROP TABLE #tempCargosSobrepostosFuncionarios_Fixos;
				SELECT
					css.cd_cargo_base_servidor,
					cbc.cd_servidor,
					css.cd_cargo,
					CASE
						WHEN css.cd_cargo = @cargoSupervisorTecnico433 OR css.cd_cargo = @cargoSupervisorTecnico434 THEN 1
						WHEN css.cd_cargo = @cargoSupervisor THEN 2
						WHEN css.cd_cargo = @cargoDiretor THEN 3
						WHEN css.cd_cargo = @cargoAD THEN 4
						WHEN css.cd_cargo = @cargoCP THEN 5
						WHEN css.cd_cargo = @cargoATE THEN 8
						WHEN css.cd_cargo = @cargoAuxDesenvolvimentoInfantil THEN 9
					END AS prioridade
				INTO #tempCargosSobrepostosFuncionarios_Fixos
				FROM
					v_servidor_cotic serv (NOLOCK)
				INNER JOIN
					v_cargo_base_cotic cbc (NOLOCK)
					ON serv.cd_servidor = cbc.cd_servidor
				INNER JOIN
					cargo_sobreposto_servidor css (NOLOCK)
					ON cbc.cd_cargo_base_servidor = css.cd_cargo_base_servidor
				INNER JOIN
					escola esc (NOLOCK)
					ON css.cd_unidade_local_servico = esc.cd_escola
				INNER JOIN
					turma_escola te (NOLOCK)
					ON te.cd_escola = esc.cd_escola
				WHERE
					css.cd_cargo IN (@cargoCP, @cargoAD, @cargoDiretor, @cargoSupervisor, @cargoSupervisorTecnico433, @cargoSupervisorTecnico434, @cargoATE, @cargoAuxDesenvolvimentoInfantil)
					AND (css.dt_fim_cargo_sobreposto IS NULL OR css.dt_fim_cargo_sobreposto > GETDATE())
					{(dataReferencia.HasValue ? "AND css.dt_nomeacao_cargo_sobreposto >= @dataReferencia " : "")}
					{(!string.IsNullOrEmpty(rf) ? $"AND serv.cd_registro_funcional = @rf " : " ")} ");

			query.AdicionarCondicaoIn(parametrosCargaInicialDto.TiposUes, "esc.tp_escola", nameof(parametrosCargaInicialDto.TiposUes));
			query.AdicionarCondicaoIn(parametrosCargaInicialDto.Ues, "te.cd_escola", nameof(parametrosCargaInicialDto.Ues));
			query.AdicionarCondicaoIn(parametrosCargaInicialDto.Turmas, "te.cd_turma_escola", nameof(parametrosCargaInicialDto.Turmas));
			query.AppendLine(";");

			// 3. União das tabelas de cargo fixo
			query.AppendLine(@$"-- 3. União das tabelas de cargo fixo
				IF OBJECT_ID('tempdb..#tempCargosFuncionarios_Fixos') IS NOT NULL
					DROP TABLE #tempCargosFuncionarios_Fixos;

				SELECT
					*
				INTO #tempCargosFuncionarios_Fixos
				FROM
					(SELECT * FROM #tempCargosSobrepostosFuncionarios_Fixos) AS sobrepostos
				UNION
					(SELECT * FROM #tempCargosBaseFuncionarios_Fixos
						WHERE NOT cd_cargo_base_servidor IN (SELECT DISTINCT cd_cargo_base_servidor FROM #tempCargosSobrepostosFuncionarios_Fixos)); ");

			// 4. Funções específicas ativas
			query.AppendLine(@$"-- 4. Funções específicas ativas
				DECLARE @tipoFuncaoPAP AS INT = 30;
				DECLARE @tipoFuncaoPAEE AS INT = 6;
				DECLARE @tipoFuncaoCIEJAASSISTPED AS INT = 42;
				DECLARE @tipoFuncaoCIEJAASSISTCOORD AS INT = 43;
				DECLARE @tipoFuncaoCIEJACOORD AS INT = 44;

				IF OBJECT_ID('tempdb..#tempProfessores_PAP_PAEE_CIEJA') IS NOT NULL
					DROP TABLE #tempProfessores_PAP_PAEE_CIEJA;
				SELECT
					cbc.cd_cargo_base_servidor,
					cbc.cd_servidor,
					cbc.cd_cargo,
					CASE
						WHEN facs.cd_tipo_funcao = @tipoFuncaoPAP THEN 6
						WHEN facs.cd_tipo_funcao = @tipoFuncaoPAEE THEN 7
						ELSE 10
					END AS prioridade
				INTO #tempProfessores_PAP_PAEE_CIEJA
				FROM
					v_cargo_base_cotic cbc (NOLOCK)
				INNER JOIN
					funcao_atividade_cargo_servidor facs (NOLOCK)
					ON cbc.cd_cargo_base_servidor = facs.cd_cargo_base_servidor
				INNER JOIN
					escola esc (NOLOCK)
					ON facs.cd_unidade_local_servico = esc.cd_escola
				INNER JOIN
					turma_escola te (NOLOCK)
					ON te.cd_escola = esc.cd_escola
				WHERE
					facs.cd_tipo_funcao IN (@tipoFuncaoPAP, @tipoFuncaoPAEE, @tipoFuncaoCIEJAASSISTPED, @tipoFuncaoCIEJAASSISTCOORD, @tipoFuncaoCIEJACOORD)
					AND (facs.dt_fim_funcao_atividade IS NULL OR facs.dt_fim_funcao_atividade > GETDATE())
					AND dt_fim_nomeacao IS NULL
					{(dataReferencia.HasValue ? "AND facs.dt_designacao >= @dataReferencia " : " ")} ");

			query.AdicionarCondicaoIn(parametrosCargaInicialDto.TiposUes, "esc.tp_escola", nameof(parametrosCargaInicialDto.TiposUes));
			query.AdicionarCondicaoIn(parametrosCargaInicialDto.Ues, "te.cd_escola", nameof(parametrosCargaInicialDto.Ues));
			query.AdicionarCondicaoIn(parametrosCargaInicialDto.Turmas, "te.cd_turma_escola", nameof(parametrosCargaInicialDto.Turmas));
			query.AppendLine(";");

			// 5. União das tabelas de cargo fixo e função
			query.AppendLine(@$"-- 5. União das tabelas de cargo fixo e função
				IF OBJECT_ID('tempdb..#tempCargosFuncionarios') IS NOT NULL
					DROP TABLE #tempCargosFuncionarios;
				SELECT
					*
				INTO #tempCargosFuncionarios
				FROM
					(SELECT * FROM #tempCargosFuncionarios_Fixos) AS fixos
				UNION
					(SELECT * FROM #tempProfessores_PAP_PAEE_CIEJA);

				-- 6. Ajustar prioridade para funcionários com mais de um cargo
				IF OBJECT_ID('tempdb..#tempFuncionariosCargoPrioridade') IS NOT NULL
					DROP TABLE #tempFuncionariosCargoPrioridade;
				SELECT
					cd_servidor,
					MIN(prioridade) AS prioridade
				INTO #tempFuncionariosCargoPrioridade
				FROM
					#tempCargosFuncionarios
				GROUP BY
					cd_servidor;

				IF OBJECT_ID('tempdb..#tempCargosFuncionariosRemovendoDuplicados') IS NOT NULL
					DROP TABLE #tempCargosFuncionariosRemovendoDuplicados;
				SELECT
					DISTINCT
					t2.cd_cargo,
					t1.cd_servidor
				INTO #tempCargosFuncionariosRemovendoDuplicados
				FROM
					#tempFuncionariosCargoPrioridade t1
				CROSS APPLY
				(
					SELECT TOP 1 * FROM #tempCargosFuncionarios temp WHERE temp.cd_servidor = t1.cd_servidor AND temp.prioridade = t1.prioridade
				) AS t2; 

				IF OBJECT_ID('tempdb..#tempCargosFuncionariosRemovendoDuplicadosFinal') IS NOT NULL
					DROP TABLE #tempCargosFuncionariosRemovendoDuplicadosFinal;
				SELECT
					*
				INTO #tempCargosFuncionariosRemovendoDuplicadosFinal
				FROM
					#tempCargosFuncionariosRemovendoDuplicados
				ORDER BY cd_servidor 
				{(aplicarPaginacao ? " OFFSET @quantidadeRegistrosIgnorados ROWS  FETCH NEXT @quantidadeRegistros ROWS ONLY;" : ";")} ");

            query.Append(@$"
				SELECT
					serv.cd_registro_funcional AS Rf,
					serv.nm_pessoa AS NomePessoa,
					serv.nm_social AS NomeSocial,
					'True' AS Ativo,
					[dbo].[proc_gerar_unidade_organizacional_funcionario_v2](temp.cd_cargo, '') AS OrganizationPath
				FROM
					v_servidor_cotic serv (NOLOCK)
				INNER JOIN
					#tempCargosFuncionariosRemovendoDuplicadosFinal temp
					ON temp.cd_servidor = serv.cd_servidor
				{(!string.IsNullOrEmpty(rf) ? $"WHERE serv.cd_registro_funcional = @rf; " : "; ")}

				SELECT
					COUNT(*)
				FROM
					#tempCargosFuncionariosRemovendoDuplicados temp				
				{(!string.IsNullOrEmpty(rf) ? $@"INNER JOIN v_servidor_cotic serv (NOLOCK)
												 ON temp.cd_servidor = serv.cd_servidor
												WHERE serv.cd_registro_funcional = @rf; " : "; ")}
				");

            return query.ToString();

        }

        public async Task<PaginacaoResultadoDto<RemoverAtribuicaoFuncionarioTurmaEolDto>> ObterFuncionariosParaRemoverCursoPaginado(string turmaId, DateTime dataInicio, DateTime dataFim, Paginacao paginacao, ParametrosCargaInicialDto parametrosCargaInicialDto)
        {
            var parametros = new
            {
                turmaId,
                dataInicio,
                dataFim,
                paginacao.QuantidadeRegistros,
                paginacao.QuantidadeRegistrosIgnorados,
				parametrosCargaInicialDto.TiposUes,
				parametrosCargaInicialDto.Ues,
				parametrosCargaInicialDto.Turmas
			};
            var queryContador = MontarQueryFuncionariosRemoverCursos(turmaId, true, false, parametrosCargaInicialDto);

            var retorno = new PaginacaoResultadoDto<RemoverAtribuicaoFuncionarioTurmaEolDto>();

            using var conn = ObterConexao();
            var totalRegistros = await conn.QueryFirstOrDefaultAsync<int>(queryContador, parametros);

            var query = MontarQueryFuncionariosRemoverCursos(turmaId, false, true, parametrosCargaInicialDto);
            retorno.Items = await conn.QueryAsync<RemoverAtribuicaoFuncionarioTurmaEolDto>(query, parametros);
            retorno.TotalRegistros = totalRegistros;
            retorno.TotalPaginas = (int)Math.Ceiling((double)retorno.TotalRegistros / paginacao.QuantidadeRegistros);

            return retorno;
        }

        private string MontarQueryFuncionariosRemoverCursos(string turmaId, bool contador, bool paginar, ParametrosCargaInicialDto parametrosCargaInicialDto)
        {
            var filtroTurma = string.IsNullOrEmpty(turmaId) ? "" : "and te.cd_turma_escola = @turmaId";

            var queryBuscarfuncionarioPorCargoFixo = $@"
				DECLARE @cargoCP AS INT = 3379;
				DECLARE @cargoAD AS INT = 3085;
				DECLARE @cargoDiretor AS INT = 3360;
				DECLARE @tipoFuncaoPAP AS INT = 30;
				DECLARE @tipoFuncaoPAEE AS INT = 6;
				DECLARE @tipoFuncaoCIEJAASSISTPED AS INT = 42;
				DECLARE @tipoFuncaoCIEJAASSISTCOORD AS INT = 43;
				DECLARE @tipoFuncaoCIEJACOORD AS INT = 44;

				-- 1. Busca os funcionários por cargo fixo
				IF OBJECT_ID('tempdb..#tempServidorCargosBase') IS NOT NULL
					DROP TABLE #tempServidorCargosBase
				SELECT
					serv.cd_registro_funcional AS Rf,
					esc.cd_escola AS CdUe,
					te.cd_turma_escola as CdTurma,
					cbc.cd_cargo AS CdCagoFuncao,
					cbc.cd_cargo_base_servidor,
					dt_fim_nomeacao as FimNomeacao
				INTO #tempServidorCargosBase
				FROM
					v_servidor_cotic serv (NOLOCK)
				INNER JOIN
					v_cargo_base_cotic cbc (NOLOCK)
					ON serv.cd_servidor = cbc.cd_servidor
				INNER JOIN
					lotacao_servidor ls (NOLOCK)
					ON ls.cd_cargo_base_servidor = cbc.cd_cargo_base_servidor
				INNER JOIN
					escola esc  (NOLOCK)
					ON ls.cd_unidade_educacao = esc.cd_escola
				INNER JOIN
					turma_escola te (NOLOCK)
					ON te.cd_escola = esc.cd_escola
				WHERE te.st_turma_escola in ('O', 'A', 'C')
					AND te.cd_tipo_turma in (1,2,3,5,6,7)
					and dt_fim_nomeacao between @dataInicio and @dataFim
					and cbc.cd_cargo IN (@cargoCP, @cargoAD, @cargoDiretor, @tipoFuncaoPAP, @tipoFuncaoPAEE, @tipoFuncaoCIEJAASSISTPED, @tipoFuncaoCIEJAASSISTCOORD, @tipoFuncaoCIEJACOORD)
					{filtroTurma}";

				var stringBuilderQueryBuscarfuncionarioPorCargoFixo = new StringBuilder(queryBuscarfuncionarioPorCargoFixo);
				stringBuilderQueryBuscarfuncionarioPorCargoFixo.AdicionarCondicaoIn(parametrosCargaInicialDto.TiposUes, "esc.tp_escola", nameof(parametrosCargaInicialDto.TiposUes));
				stringBuilderQueryBuscarfuncionarioPorCargoFixo.AdicionarCondicaoIn(parametrosCargaInicialDto.Ues, "te.cd_escola", nameof(parametrosCargaInicialDto.Ues));
				stringBuilderQueryBuscarfuncionarioPorCargoFixo.AdicionarCondicaoIn(parametrosCargaInicialDto.Turmas, "te.cd_turma_escola", nameof(parametrosCargaInicialDto.Turmas));
				stringBuilderQueryBuscarfuncionarioPorCargoFixo.Append(";");

				var queryBuscarfuncionarioPorCargoSobreposto = $@"-- 2. Busca os funcionários por cargo sobreposto fixo
				IF OBJECT_ID('tempdb..#tempServidorCargosSobrepostos') IS NOT NULL
					DROP TABLE #tempServidorCargosSobrepostos
				SELECT
					serv.cd_registro_funcional AS Rf, 
					css.cd_unidade_local_servico AS CdUe,
					te.cd_turma_escola as CdTurma,
					css.cd_cargo AS CdCagoFuncao,
					cbc.cd_cargo_base_servidor,
					css.dt_fim_cargo_sobreposto as FimNomeacao
				INTO #tempServidorCargosSobrepostos
				FROM
					v_servidor_cotic serv (NOLOCK)
				INNER JOIN
					v_cargo_base_cotic cbc (NOLOCK)
					ON serv.cd_servidor = cbc.cd_servidor
				INNER JOIN
					cargo_sobreposto_servidor css (NOLOCK)
					ON cbc.cd_cargo_base_servidor = css.cd_cargo_base_servidor
				INNER JOIN
					escola esc (NOLOCK)
					ON css.cd_unidade_local_servico = esc.cd_escola
				INNER JOIN
					turma_escola te (NOLOCK)
					ON te.cd_escola = esc.cd_escola
				WHERE te.st_turma_escola in ('O', 'A', 'C')
					AND te.cd_tipo_turma in (1,2,3,5,6,7)
					and css.dt_fim_cargo_sobreposto between @dataInicio and @dataFim
					and css.cd_cargo IN (@cargoCP, @cargoAD, @cargoDiretor, @tipoFuncaoPAP, @tipoFuncaoPAEE, @tipoFuncaoCIEJAASSISTPED, @tipoFuncaoCIEJAASSISTCOORD, @tipoFuncaoCIEJACOORD)
					{filtroTurma}";
					
				var stringBuilderQueryBuscarfuncionarioPorCargoSobreposto = new StringBuilder(queryBuscarfuncionarioPorCargoSobreposto);
				stringBuilderQueryBuscarfuncionarioPorCargoSobreposto.AdicionarCondicaoIn(parametrosCargaInicialDto.TiposUes, "esc.tp_escola", nameof(parametrosCargaInicialDto.TiposUes));
				stringBuilderQueryBuscarfuncionarioPorCargoSobreposto.AdicionarCondicaoIn(parametrosCargaInicialDto.Ues, "te.cd_escola", nameof(parametrosCargaInicialDto.Ues));
				stringBuilderQueryBuscarfuncionarioPorCargoSobreposto.AdicionarCondicaoIn(parametrosCargaInicialDto.Turmas, "te.cd_turma_escola", nameof(parametrosCargaInicialDto.Turmas));
				stringBuilderQueryBuscarfuncionarioPorCargoSobreposto.Append(";");

				var queryBuscarFuncionarioPorFuncao = $@"-- 3. Busca os funcionários por função
				IF OBJECT_ID('tempdb..#tempServidorFuncao') IS NOT NULL
					DROP TABLE #tempServidorFuncao
				SELECT
					serv.cd_registro_funcional AS Rf, 
					esc.cd_escola AS CdUe,
					te.cd_turma_escola as CdTurma,
					facs.cd_tipo_funcao AS CdCagoFuncao,
					cbc.cd_cargo_base_servidor,
					dt_fim_nomeacao as FimNomeacao
				INTO #tempServidorFuncao
				FROM
					v_servidor_cotic serv (NOLOCK)
				INNER JOIN
					v_cargo_base_cotic cbc (NOLOCK)
					ON serv.cd_servidor = cbc.cd_servidor
				INNER JOIN
					funcao_atividade_cargo_servidor facs (NOLOCK)
					ON cbc.cd_cargo_base_servidor = facs.cd_cargo_base_servidor
				INNER JOIN
					escola esc (NOLOCK)
					ON facs.cd_unidade_local_servico = esc.cd_escola
				INNER JOIN
					turma_escola te (NOLOCK)
					ON te.cd_escola = esc.cd_escola
				WHERE te.st_turma_escola in ('O', 'A', 'C')
					AND te.cd_tipo_turma in (1,2,3,5,6,7)
					and dt_fim_nomeacao between @dataInicio and @dataFim
					and facs.cd_tipo_funcao IN (@cargoCP, @cargoAD, @cargoDiretor, @tipoFuncaoPAP, @tipoFuncaoPAEE, @tipoFuncaoCIEJAASSISTPED, @tipoFuncaoCIEJAASSISTCOORD, @tipoFuncaoCIEJACOORD)
					{filtroTurma}";


				var stringBuilderQueryBuscarFuncionarioPorFuncao = new StringBuilder(queryBuscarFuncionarioPorFuncao);
				stringBuilderQueryBuscarFuncionarioPorFuncao.AdicionarCondicaoIn(parametrosCargaInicialDto.TiposUes, "esc.tp_escola", nameof(parametrosCargaInicialDto.TiposUes));
				stringBuilderQueryBuscarFuncionarioPorFuncao.AdicionarCondicaoIn(parametrosCargaInicialDto.Ues, "te.cd_escola", nameof(parametrosCargaInicialDto.Ues));
				stringBuilderQueryBuscarFuncionarioPorFuncao.AdicionarCondicaoIn(parametrosCargaInicialDto.Turmas, "te.cd_turma_escola", nameof(parametrosCargaInicialDto.Turmas));
				stringBuilderQueryBuscarFuncionarioPorFuncao.Append(";");

				var query = $@"
				{stringBuilderQueryBuscarfuncionarioPorCargoFixo}
				{stringBuilderQueryBuscarfuncionarioPorCargoSobreposto}
				{stringBuilderQueryBuscarFuncionarioPorFuncao}
				IF OBJECT_ID('tempdb..#tempServidorCargos') IS NOT NULL
					DROP TABLE #tempServidorCargos
				select * 
				INTO #tempServidorCargos
				from (
					select base.Rf, base.CdCagoFuncao, base.CdUe, base.CdTurma, base.FimNomeacao, 1 AS TipoCargo
					FROM #tempServidorCargosBase base
					union all
					select sobreposto.Rf, sobreposto.CdCagoFuncao, sobreposto.CdUe, sobreposto.CdTurma, sobreposto.FimNomeacao, 2 AS TipoCargo
					FROM #tempServidorCargosSobrepostos sobreposto
					union all
					select funcao.Rf, funcao.CdCagoFuncao, funcao.CdUe, funcao.CdTurma, funcao.FimNomeacao, 3 AS TipoCargo
					FROM #tempServidorFuncao funcao
				) tmp;


				IF OBJECT_ID('tempdb..#tempTurmasComponentesRegulares') IS NOT NULL 
					DROP TABLE #tempTurmasComponentesRegulares
				SELECT
					DISTINCT
					CASE
						WHEN etapa_ensino.cd_etapa_ensino = 1 THEN 512
					ELSE
						cc.cd_componente_curricular
					END ComponenteCurricularId,
					temp.CdTurma TurmaId,
					temp.CdUe	
				INTO #tempTurmasComponentesRegulares
				FROM
					#tempServidorCargos temp
				INNER JOIN
					serie_turma_escola (NOLOCK) 
					ON serie_turma_escola.cd_turma_escola = temp.CdTurma
				INNER JOIN
					serie_turma_grade (NOLOCK) 
					ON serie_turma_grade.cd_turma_escola = serie_turma_escola.cd_turma_escola 
				INNER JOIN
					escola_grade (NOLOCK) 
					ON serie_turma_grade.cd_escola_grade = escola_grade.cd_escola_grade
				INNER JOIN
					grade (NOLOCK) 
					ON escola_grade.cd_grade = grade.cd_grade
				INNER JOIN
					grade_componente_curricular gcc (NOLOCK) 
					ON gcc.cd_grade = grade.cd_grade
				INNER JOIN
					componente_curricular cc (NOLOCK) 
					ON cc.cd_componente_curricular = gcc.cd_componente_curricular AND cc.dt_cancelamento IS NULL
				INNER JOIN
					serie_ensino (NOLOCK) 
					ON grade.cd_serie_ensino = serie_ensino.cd_serie_ensino
				INNER JOIN
					etapa_ensino (NOLOCK) 
					ON serie_ensino.cd_etapa_ensino = etapa_ensino.cd_etapa_ensino; ";

            query += contador ? "select count(*) " :
                @"SELECT
					servidor.Rf as UsuarioRf,
					serv.nm_pessoa as UsuarioNome,
					cursos.TurmaId as TurmaCodigo,
					cursos.ComponenteCurricularId as ComponenteCurricularCodigo,
					cursos.CdUe AS UeCodigo,
					servidor.FimNomeacao,
					case servidor.TipoCargo
						when 1 then 'Cargo Base'
						when 2 then 'Cargo Sobreposto'
						else 'Função Atividade'
					end as TipoCargo ";

            query +=
                @"FROM
					#tempServidorCargos servidor
				INNER JOIN
					#tempTurmasComponentesRegulares cursos
					ON servidor.CdUe = cursos.CdUe
				inner join
					v_servidor_cotic serv
					on serv.cd_registro_funcional = servidor.Rf ";

            if (!contador)
                query += " order by servidor.FimNomeacao, cursos.CdUe, cursos.TurmaId ";

            if (paginar)
                query += " OFFSET @quantidadeRegistrosIgnorados ROWS  FETCH NEXT @quantidadeRegistros ROWS ONLY ";

            query += ";";

            return query;
        }

        public async Task<IEnumerable<RemoverAtribuicaoFuncionarioTurmaEolDto>> ObterFuncionariosParaRemoverCurso(string turmaId, DateTime dataInicio, DateTime dataFim, ParametrosCargaInicialDto parametrosCargaInicialDto)
        {
            var query = MontarQueryFuncionariosRemoverCursos(turmaId, false, false, parametrosCargaInicialDto);
            var parametros = new
            {
                turmaId,
                dataInicio,
                dataFim
            };

            using var conn = ObterConexao();
            return await conn.QueryAsync<RemoverAtribuicaoFuncionarioTurmaEolDto>(query, parametros);
        }

        public async Task<PaginacaoResultadoDto<FuncionarioEol>> ObterFuncionariosQueSeraoInativados(Paginacao paginacao, DateTime dataReferencia, string codigoRf)
        {
            var querySelectDados = @$" SELECT DISTINCT 
											serv.cd_registro_funcional as rf,
											serv.nm_pessoa as NomePessoa,
											serv.nm_social as NomeSocial ";

            var querySelectCount = "SELECT COUNT(DISTINCT serv.cd_registro_funcional) ";

            var queryFrom = new StringBuilder(@$" FROM v_servidor_cotic serv
														INNER JOIN v_cargo_base_cotic AS cba ON cba.CD_SERVIDOR = serv.cd_servidor
														INNER JOIN cargo AS car ON cba.cd_cargo = car.cd_cargo
														INNER JOIN lotacao_servidor AS ls
																	ON cba.cd_cargo_base_servidor = ls.cd_cargo_base_servidor
														WHERE cba.dt_fim_nomeacao <= @dataReferencia
															{(String.IsNullOrEmpty(codigoRf) == false ? " AND serv.cd_registro_funcional = @codigoRf " : "")}
															AND serv.cd_registro_funcional NOT IN(
																SELECT
																	distinct serv.cd_registro_funcional
																FROM v_servidor_cotic serv
																	INNER JOIN v_cargo_base_cotic AS cba ON cba.CD_SERVIDOR = serv.cd_servidor
																	INNER JOIN cargo AS car ON cba.cd_cargo = car.cd_cargo
																	INNER JOIN lotacao_servidor AS ls
																ON cba.cd_cargo_base_servidor = ls.cd_cargo_base_servidor
																WHERE cba.dt_fim_nomeacao IS NULL) ");

            var queryPaginacao = @"ORDER BY serv.cd_registro_funcional offset @quantidadeRegistrosIgnorados rows fetch next @quantidadeRegistros rows only;";

            var query = new StringBuilder(querySelectDados);
            query.Append(queryFrom);
            query.Append(queryPaginacao);
            query.Append(querySelectCount);
            query.Append(queryFrom);

            using var conn = ObterConexao();
            using var multi = await conn.QueryMultipleAsync(query.ToString(),
                new
                {
                    quantidadeRegistros = paginacao.QuantidadeRegistros,
                    quantidadeRegistrosIgnorados = paginacao.QuantidadeRegistrosIgnorados,
                    dataReferencia,
                    codigoRf
                }, commandTimeout: 6000);

            var retorno = new PaginacaoResultadoDto<FuncionarioEol>
            {
                Items = multi.Read<FuncionarioEol>(),
                TotalRegistros = multi.ReadFirst<int>()
            };

            retorno.TotalPaginas = paginacao.QuantidadeRegistros > 0 ? (int)Math.Ceiling((double)retorno.TotalRegistros / paginacao.QuantidadeRegistros) : 1;
            return retorno;
        }
    }
}