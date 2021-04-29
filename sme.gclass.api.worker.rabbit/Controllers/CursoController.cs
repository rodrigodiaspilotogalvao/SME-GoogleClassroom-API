﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SME.GoogleClassroom.Aplicacao;
using SME.GoogleClassroom.Aplicacao.Interfaces;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using SME.GoogleClassroom.Worker.Rabbit.Filters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Worker.Rabbit
{
    /// <summary>
    /// Cursos
    /// </summary>
    [ApiController]
    [ChaveIntegracaoGoogleClassroomApi]
    [Route("api/v1/cursos")]
    public class CursoController : Controller
    {
        /// <summary>
        /// Retorna os cursos do EOL que serão incluídos no Google Classroom.
        /// </summary>
        /// <remarks>
        /// **Importante:** Para retornar todos os registros sem aplicar paginação informe o valor 0 (zero) nos campos *PaginaNumero* e *RegistrosQuantidade*:
        ///
        ///     GET
        ///     {
        ///        "TurmaId": "null",
        ///        "ComponenteCurricularId": "null",
        ///        "UltimaExecucao": "2021-02-20",
        ///        "PaginaNumero" :" 0",
        ///        "RegistrosQuantidade" : "0"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">A consulta foi realizada com sucesso.</response>
        /// <response code="500">Ocorreu um erro inesperado durante a consulta.</response>
        /// <response code="601">Houve uma falha de validação durante a consulta.</response>
        [HttpGet("novos")]
        [ProducesResponseType(typeof(PaginacaoResultadoDto<CursoGoogle>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RetornoBaseDto), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(RetornoBaseDto), 601)]
        public async Task<IActionResult> ObterTodosCursosParaIncluir([FromServices] IObterCursosParaIncluirGoogleUseCase obterCursosParaIncluirGoogleUseCase,
            [FromQuery] FiltroObterCursosIncluirGoogleDto filtro)
        {
            var retorno = await obterCursosParaIncluirGoogleUseCase.Executar(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna os cursos já incluídos no Google Classroom.
        /// </summary>
        /// <response code="200">A consulta foi realizada com sucesso.</response>
        /// <response code="500">Ocorreu um erro inesperado durante a consulta.</response>
        /// <response code="601">Houve uma falha de validação durante a consulta.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginacaoResultadoDto<CursoGoogle>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RetornoBaseDto), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(RetornoBaseDto), 601)]
        public async Task<IActionResult> ObterTodosCursos([FromServices] IObterCursosCadastradosUseCase obterCursosCadastradosUseCase,
            [FromQuery] FiltroObterCursosCadastradosDto filtro)
        {
            var retorno = await obterCursosCadastradosUseCase.Executar(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Inicia a sincronização de cursos do EOL para o Google Classroom.
        /// </summary>
        /// <remarks>
        /// **Importante:** Visando a melhoria de performance, a sincronização dos cursos acontece de forma assíncrona e descentralizada,
        /// não sendo possível assim acompanhar em tempo real sua evolução.
        /// </remarks>
        /// <response code="200">O início da sincronização ocorreu com sucesso.</response>
        [HttpPost("sincronizacao")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> IniciarSincronizacao([FromServices] IIniciarSyncGoogleCursoUseCase iniciarSyncGoogleCursoUseCase, long? turmaId = null, long? componenteCurricularId = null)
        {
            var retorno = await iniciarSyncGoogleCursoUseCase.Executar(turmaId, componenteCurricularId);
            return Ok(retorno);
        }

        /// <summary>
        /// Envia novamente para inserir no GSA, os cursos que estão na tabela de cursos_erro
        /// </summary>
        /// <remarks>
        /// **Importante:** Visando a melhoria de performance, a sincronização dos cursos acontece de forma assíncrona e descentralizada,
        /// não sendo possível assim acompanhar em tempo real sua evolução.
        /// </remarks>
        /// <response code="200">O início da sincronização ocorreu com sucesso.</response>
        [HttpPost("erros/tratamentos")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> TratarErros([FromServices] IIniciarSyncGoogleCursoErrosUseCase iniciarSyncGoogleCursoErrosUseCase)
        {
            await iniciarSyncGoogleCursoErrosUseCase.Executar();
            return Ok();
        }

        /// <summary>
        /// Retorna as grades dos cursos para incluir no Google Classroom.
        /// </summary>
        /// <response code="200">A consulta foi realizada com sucesso.</response>
        /// <response code="500">Ocorreu um erro inesperado durante a consulta.</response>
        /// <response code="601">Houve uma falha de validação durante a consulta.</response>
        [HttpGet("grades")]
        [ProducesResponseType(typeof(PaginacaoResultadoDto<GradeCursoEol>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RetornoBaseDto), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(RetornoBaseDto), 601)]
        public async Task<IActionResult> ObterGradesDeCursosDosAlunos([FromServices] IObterGradesDeCursosUseCase useCase,
            [FromQuery] FiltroObterGradesDeCursosDto filtro)
        {
            var retorno = await useCase.Executar(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Inicia a sincronização das grades de cursos do EOL para o Google Classroom.
        /// </summary>
        /// <remarks>
        /// **Importante:** Visando a melhoria de performance, a sincronização das grades acontece de forma assíncrona e descentralizada,
        /// não sendo possível assim acompanhar em tempo real sua evolução.
        /// </remarks>
        /// <response code="200">O início da sincronização ocorreu com sucesso.</response>
        [HttpPost("grades/sincronizacao")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> IniciarSincronizacaoGrades([FromServices] IIniciarSyncGoogleGradesUseCase iniciarSyncGoogleAtribuicoesDosAlunosUseCase)
        {
            var retorno = await iniciarSyncGoogleAtribuicoesDosAlunosUseCase.Executar();
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna os alunos do curso do EOL que serão incluídos no Google Classroom.
        /// </summary>
        /// <response code="200">A consulta foi realizada com sucesso.</response>
        /// <response code="500">Ocorreu um erro inesperado durante a consulta.</response>
        /// <response code="601">Houve uma falha de validação durante a consulta.</response>
        [HttpGet("alunos/novos")]
        [ProducesResponseType(typeof(IEnumerable<AlunoCursoEol>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RetornoBaseDto), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(RetornoBaseDto), 601)]
        public async Task<IActionResult> ObterAlunosCursosGoogle([FromServices] IObterCursosAlunosParaIncluirGoogleUseCase useCase,
            [FromQuery][Required] long turmaId, [FromQuery][Required] long componenteCurricularId)
        {
            var retorno = await useCase.Executar(turmaId, componenteCurricularId);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna os funcionários do curso do EOL que serão incluídos no Google Classroom.
        /// </summary>
        /// <response code="200">A consulta foi realizada com sucesso.</response>
        /// <response code="500">Ocorreu um erro inesperado durante a consulta.</response>
        /// <response code="601">Houve uma falha de validação durante a consulta.</response>
        [HttpGet("funcionarios/novos")]
        [ProducesResponseType(typeof(IEnumerable<FuncionarioCursoEol>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RetornoBaseDto), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(RetornoBaseDto), 601)]
        public async Task<IActionResult> ObterFuncionariosCursosGoogle([FromServices] IObterCursosFuncionariosParaIncluirGoogleUseCase useCase,
            [FromQuery][Required] long turmaId, [FromQuery][Required] long componenteCurricularId)
        {
            var retorno = await useCase.Executar(turmaId, componenteCurricularId);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna os cursos que serão incluídos para comparativo.
        /// </summary>
        /// <response code="200">A consulta foi realizada com sucesso.</response>
        /// <response code="500">Ocorreu um erro inesperado durante a consulta.</response>
        /// <response code="601">Houve uma falha de validação durante a consulta.</response>
        [HttpGet("comparativos")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RetornoBaseDto), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(RetornoBaseDto), 601)]
        public async Task<IActionResult> ObterCursosComparativos([FromServices] IIniciarCargaCursosUseCase useCase)
        {
            var retorno = await useCase.Executar();
            return Ok(retorno);
        }
    }
}