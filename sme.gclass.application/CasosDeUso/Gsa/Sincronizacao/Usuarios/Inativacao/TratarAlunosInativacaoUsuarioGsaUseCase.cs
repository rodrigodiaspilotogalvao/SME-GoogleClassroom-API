﻿using MediatR;
using SME.GoogleClassroom.Infra;
using System.Linq;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class TratarAlunosInativacaoUsuarioGsaUseCase : ITratarAlunosInativacaoUsuarioUseCase
    {
        private readonly IMediator mediator;

        public TratarAlunosInativacaoUsuarioGsaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(MensagemRabbit mensagemRabbit)
        {
            var dto = mensagemRabbit.ObterObjetoMensagem<FiltroAlunoInativacaoUsuarioDto>();
            var alunosCodigos = dto.AlunosIds.ToArray();
            var alunosGoogle = await mediator.Send(new ObterAlunosPorCodigosQuery(alunosCodigos));

            if (alunosGoogle != null && alunosGoogle.Any())
            {
                foreach (var alunoGoogle in alunosGoogle)
                {
                    var alunoInativar = new AlunoUsuarioInativarDto(alunoGoogle.Indice, alunoGoogle.GoogleClassroomId, alunoGoogle.Email);
                    await mediator.Send(new PublicaFilaRabbitCommand(RotasRabbit.FilaGsaInativarUsuarioIncluir, RotasRabbit.FilaGsaInativarUsuarioIncluir, alunoInativar));
                }
            }
            return true;
        }
    }
}
