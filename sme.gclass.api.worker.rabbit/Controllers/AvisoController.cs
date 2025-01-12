﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SME.GoogleClassroom.Aplicacao.Interfaces;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using SME.GoogleClassroom.Worker.Rabbit.Filters;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Worker.Rabbit.Controllers
{
    /// <summary>
    /// Avisos
    /// </summary>
    [ApiController]
    [ChaveIntegracaoGoogleClassroomApi]
    [Route("api/v1/avisos")]
    public class AvisoController : Controller
    {
        /// <summary>
        /// Retorna os avisos do mural do Google Classroom.
        /// </summary>
        /// <remarks>
        /// **Importante:** Para retornar todos os registros sem aplicar paginação informe o valor 0 (zero) nos campos *PaginaNumero* e *RegistrosQuantidade*:
        ///
        ///     GET
        ///     {
        ///        "DataReferencia": "2021-02-20",
        ///        "PaginaNumero" : "0",
        ///        "RegistrosQuantidade" : "0"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">A consulta foi realizada com sucesso.</response>
        /// <response code="500">Ocorreu um erro inesperado durante a consulta.</response>
        /// <response code="601">Houve uma falha de validação durante a consulta.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginacaoResultadoDto<AvisoGsa>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RetornoBaseDto), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(RetornoBaseDto), 601)]
        public async Task<IActionResult> ObterAvisos([FromServices] IObterAvisoUseCase obterAvisoUseCase,
            [FromQuery] FiltroObterAvisoDto filtro)
        {
            var retorno = await obterAvisoUseCase.Executar(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Executa o tratamento dos erros de importação dos avisos dos murais GSA.
        /// Insere o registro na base GCA e envia para o SGP a vincular com a aula.
        /// </summary>
        /// <response code="200">O início da sincronização ocorreu com sucesso.</response>
        [HttpPost("erros/tratamentos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RetornoBaseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProcessarErros([FromServices] IIncluirAvisosGsaErroProcessarUseCase useCase)
        {
            await useCase.Executar();

            return Ok();
        }

        /// <summary>
        /// Inicia a sincronização de Mural de Avisos do Google Classroom para o SGP.
        /// </summary>
        /// <remarks>
        /// **Importante:** Visando a melhoria de performance, a sincronização dos alunos acontece de forma assíncrona e descentralizada,
        /// não sendo possível assim acompanhar em tempo real sua evolução.
        /// </remarks>
        /// <response code="200">O início da sincronização ocorreu com sucesso.</response>
        [HttpPost("sincronizacao")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> IniciarSincronizacao([FromServices] IIniciarSyncGoogleMuralAvisosUseCase useCase, long? cursoId = null, int? pagina = null, int? totalPaginas = null)
        {
            await useCase.Executar(cursoId);
            return Ok();
        }

    }
}
