﻿using Castle.Core.Configuration;
using MediatR;
using Moq;
using Polly;
using Polly.Registry;
using Polly.Retry;
using SME.GoogleClassroom.Aplicacao;
using SME.GoogleClassroom.Dados;
using SME.GoogleClassroom.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.GoogleClassroom.Testes
{
    public class InserirProfessorCursoGoogleCommandHandlerTeste
    {
        private readonly Mock<IMediator> mediator;        
        //private readonly IReadOnlyPolicyRegistry<string> registry;
        private readonly Mock<IRepositorioCursoUsuario> repositorioCursoUsuario;
        private readonly InserirProfessorCursoGoogleCommandHandler inserirProfessorCursoGoogleCommandHandler;

        public InserirProfessorCursoGoogleCommandHandlerTeste()
        {
            mediator = new Mock<IMediator>();
            IReadOnlyPolicyRegistry<string> registry = null;            
            repositorioCursoUsuario = new Mock<IRepositorioCursoUsuario>();
            inserirProfessorCursoGoogleCommandHandler = new InserirProfessorCursoGoogleCommandHandler(mediator.Object, registry);
        }
        //[Fact]
        //public async Task Deve_Inserir_Professor_Curso_Google()
        //{
        //    // Arrange
        //    var professorGoogle = new ProfessorGoogle(123456, "Jose da Silva", "", "");
        //    mediator.Setup(a => a.Send(It.IsAny<ObterProfessoresPorRfsQuery>(), It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(new List<ProfessorGoogle>()
        //        {
        //            professorGoogle
        //        });            

        //    mediator.Setup(a => a.Send(It.IsAny<ObterCursoPorTurmaComponenteCurricularQuery>(), It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(new Curso()
        //        {
        //            Id = 1,
        //            TurmaId = 1234,
        //            ComponenteCurricularId = 43
        //        });

        //    mediator.Setup(a => a.Send(It.IsAny<ExisteProfessorCursoGoogleQuery>(), It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(false);

        //    //Act
        //    var professorCursoeol = new ProfessorCursoEol()
        //    {
        //        Rf = 123456,
        //        ComponenteCurricularId = 43,
        //        TurmaId = 1234
        //    };
        //    var inserido = await inserirProfessorCursoGoogleCommandHandler.Handle(new InserirProfessorCursoGoogleCommand(professorCursoeol), new CancellationToken());

        //    // Assert
        //    repositorioCursoUsuario.Verify(x => x.SalvarAsync(It.IsAny<CursoUsuario>()), Times.Once);
        //    Assert.True(inserido);
        //}

        [Fact]
        public async Task Nao_Deve_Inserir_Existe_Professor_Curso_Google_Cadastrado()
        {
            // Arrange
            var professorGoogle = new ProfessorGoogle(123456, "Jose da Silva", "", "");
            mediator.Setup(a => a.Send(It.IsAny<ObterProfessoresPorRfsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProfessorGoogle>()
                {
                    professorGoogle
                });

            mediator.Setup(a => a.Send(It.IsAny<ObterCursoPorTurmaComponenteCurricularQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Curso()
                {
                    Id = 1,
                    TurmaId = 1234,
                    ComponenteCurricularId = 43
                });

            mediator.Setup(a => a.Send(It.IsAny<ExisteProfessorCursoGoogleQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            //Act
            var professorCursoeol = new ProfessorCursoEol()
            {
                Rf = 123456,
                ComponenteCurricularId = 43,
                TurmaId = 1234
            };
            var inserido = await inserirProfessorCursoGoogleCommandHandler.Handle(new InserirProfessorCursoGoogleCommand(professorCursoeol), new CancellationToken());

            // Assert            
            Assert.True(inserido);
        }

        [Fact]
        public async Task Nao_Deve_Inserir_Nao_Existe_Professor_Google_Cadastrado()
        {
            // Arrange
            var professorGoogle = new ProfessorGoogle(123456, "Jose da Silva", "", "");
            mediator.Setup(a => a.Send(It.IsAny<ObterProfessoresPorRfsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProfessorGoogle>());

            mediator.Setup(a => a.Send(It.IsAny<ObterCursoPorTurmaComponenteCurricularQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Curso()
                {
                    Id = 1,
                    TurmaId = 1234,
                    ComponenteCurricularId = 43
                });

            mediator.Setup(a => a.Send(It.IsAny<ExisteProfessorCursoGoogleQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            //Act
            var professorCursoeol = new ProfessorCursoEol()
            {
                Rf = 123456,
                ComponenteCurricularId = 43,
                TurmaId = 1234
            };
            var inserido = await inserirProfessorCursoGoogleCommandHandler.Handle(new InserirProfessorCursoGoogleCommand(professorCursoeol), new CancellationToken());

            // Assert
            //repositorioCursoUsuario.Verify(x => x.SalvarAsync(It.IsAny<CursoUsuario>()), Times.Once);
            Assert.True(!inserido);
        }

        [Fact]
        public async Task Nao_Deve_Inserir_Nao_Existe_Curso_Google_Cadastrado()
        {
            // Arrange
            var professorGoogle = new ProfessorGoogle(123456, "Jose da Silva", "", "");
            mediator.Setup(a => a.Send(It.IsAny<ObterProfessoresPorRfsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProfessorGoogle>()
                {
                    professorGoogle
                });

            Curso curso = null;

            mediator.Setup(a => a.Send(It.IsAny<ObterCursoPorTurmaComponenteCurricularQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(curso);

            mediator.Setup(a => a.Send(It.IsAny<ExisteProfessorCursoGoogleQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            //Act
            var professorCursoeol = new ProfessorCursoEol()
            {
                Rf = 123456,
                ComponenteCurricularId = 43,
                TurmaId = 1234
            };
            var inserido = await inserirProfessorCursoGoogleCommandHandler.Handle(new InserirProfessorCursoGoogleCommand(professorCursoeol), new CancellationToken());

            // Assert
            //repositorioCursoUsuario.Verify(x => x.SalvarAsync(It.IsAny<CursoUsuario>()), Times.Once);
            Assert.True(!inserido);
        }
    }
}
