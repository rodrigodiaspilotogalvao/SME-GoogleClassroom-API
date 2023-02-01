using System.Collections.Generic;
using System.Threading.Tasks;
using SME.GoogleClassroom.Infra;

namespace SME.GoogleClassroom.Aplicacao
{
    public interface IObterAulasPorTurmaComponenteCurricularDataUseCase
    {
        Task<IEnumerable<AulaQuantidadeTipoDto>> Executar(FiltroAulasPorTurmaComponenteDataDto filtro);
    }
}