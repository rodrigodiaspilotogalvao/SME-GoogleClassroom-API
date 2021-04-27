﻿using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;
using MediatR;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Registry;
using Polly.Retry;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.GoogleClassroom.Aplicacao
{
    public class InserirProfessorGoogleCommandHandler : BaseIntegracaoGoogleClassroomHandler<InserirProfessorGoogleCommand>
    {
        private readonly IMediator mediator;
        private readonly IConfiguration configuration;
        private readonly IAsyncPolicy policy;

        public InserirProfessorGoogleCommandHandler(IMediator mediator, IConfiguration configuration, IReadOnlyPolicyRegistry<string> registry, VariaveisGlobaisOptions variaveisGlobais) 
            : base(variaveisGlobais)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.policy = registry.Get<IAsyncPolicy>("RetryPolicy");
        }

        protected override async Task<bool> ExecutarAsync(InserirProfessorGoogleCommand request, CancellationToken cancellationToken)
        {
            var diretorioClassroom = await mediator.Send(new ObterDirectoryServiceGoogleClassroomQuery());
            await policy.ExecuteAsync(() => IncluirProfessorNoGoogle(request.ProfessorGoogle, diretorioClassroom));
            return true;
        }

        private async Task IncluirProfessorNoGoogle(ProfessorGoogle professorGoogle, DirectoryService diretorioClassroom)
        {
            var usuarioParaIncluirNoGoogle = new User
            {
                Name = new UserName { FamilyName = professorGoogle.Sobrenome, GivenName = professorGoogle.PrimeiroNome, FullName = professorGoogle.Nome },
                PrimaryEmail = professorGoogle.Email,
                OrgUnitPath = professorGoogle.OrganizationPath,
                Password = configuration["GoogleClassroomConfig:PasswordPadraoParaUsuarioNovo"],
                ChangePasswordAtNextLogin = true
            };

            var requestCreate = diretorioClassroom.Users.Insert(usuarioParaIncluirNoGoogle);
            var usuarioIncluido = await requestCreate.ExecuteAsync();

            if (usuarioIncluido is null)
                throw new NegocioException("Não foi possível obter o professor incluído no Google Classroom.");

            professorGoogle.GoogleClassroomId = usuarioIncluido.Id;
        }
    }
}