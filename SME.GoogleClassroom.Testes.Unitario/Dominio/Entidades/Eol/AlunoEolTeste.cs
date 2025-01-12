﻿using SME.GoogleClassroom.Dominio;
using System;
using Xunit;

namespace SME.GoogleClassroom.Testes.Unitario.Dominio.Entidades.Eol
{
    public class AlunoEolTeste
    {
        [Theory(DisplayName = "Valida a definição do nome e e-mail tratando nome social."), MemberData(nameof(DadosParaValidacaoDefinicaoDeNome))]
        public void Valida_Definicao_Do_Nome_E_Email_Valido_Passando_Os_Resultados_Esperados(int codigoAluno, string nomePessoa, string nomeSocial, DateTime dataNascimento, string nomeEsperado, string emailEsperado)
        {
            // Arrange
            var alunoEol = new AlunoEol(codigoAluno, nomePessoa, nomeSocial, string.Empty, dataNascimento);

            // Assert
            Assert.NotNull(alunoEol);
            Assert.Equal(nomeEsperado, alunoEol.Nome);
            Assert.Equal(emailEsperado, alunoEol.Email);
        }

        [Theory(DisplayName = "Valida a geração de e-mail válido de aluno."), MemberData(nameof(DadosParaValidacaoDeEmailValido))]
        private void Valida_Geracao_Email_Valido_Passando_O_Resultado_Esperado(string nome, DateTime dataNascimento, string emailEsperado)
        {
            // Arange
            var aluno = new AlunoEol(1, nome, string.Empty, string.Empty, dataNascimento);

            // Assert
            Assert.Equal(emailEsperado, aluno.Email);
        }

        [Theory(DisplayName = "Valida a geração de e-mail inválido de aluno."), MemberData(nameof(DadosParaValidacaoDeEmailInvalido))]
        private void Valida_Geracao_Email_Invalido_Passando_O_Resultado_Esperado(string nome, DateTime dataNascimento, string emailEsperado)
        {
            // Arrange
            var aluno = new AlunoEol(1, nome, string.Empty, string.Empty, dataNascimento);

            // Assert
            Assert.NotEqual(emailEsperado, aluno.Email);
        }

        [Theory(DisplayName = ""), MemberData(nameof(DadosParaValidacaoDeEmailValidoPorTentativa))]
        private void Valida_Geracao_Email_Alternativo_Para_Tratamento_De_Duplicidade_Passando_O_Resultado_Esperado(int tentativa, string nome, DateTime dataNascimento, string emailEsperado)
        {
            // Arrange
            var aluno = new AlunoEol(1, nome, string.Empty, string.Empty, dataNascimento);

            // Act
            aluno.DefinirEmail(tentativa);

            // Assert
            Assert.Equal(emailEsperado, aluno.Email);
        }

        public static readonly object[][] DadosParaValidacaoDeEmailValido =
        {
            new object[] { "", null, null },
            new object[] { "Maria Jesus", new DateTime(1992, 06, 06), "mariajesus.06061992@edu.sme.prefeitura.sp.gov.br" },
            new object[] { "Maria Tereza Jesus", new DateTime(2017, 2, 1), "mariatjesus.01022017@edu.sme.prefeitura.sp.gov.br" },
            new object[] { "Maria Tereza Santos Jesus", new DateTime(2017,3,1), "mariatsjesus.01032017@edu.sme.prefeitura.sp.gov.br" },
            new object[] { "Maria Tereza de Santos Jesus", new DateTime(1992, 06, 06),"mariatsjesus.06061992@edu.sme.prefeitura.sp.gov.br" },
            new object[] { "José da Silva", new DateTime(1992, 06, 06) , "josesilva.06061992@edu.sme.prefeitura.sp.gov.br" }
        };

        public static readonly object[][] DadosParaValidacaoDeEmailInvalido =
        {
            new object[] { "Maria Jesus", new DateTime(1992, 06, 06), "mariaesus.06061992@edu.sme.prefeitura.sp.gov.br" },
            new object[] { "Maria Tereza Jesus", new DateTime(2017, 2, 1), "marjesus.01022017@edu.sme.prefeitura.sp.gov.br" },
            new object[] { "Maria Tereza Santos Jesus", new DateTime(2017,3,1), "marisus.01032017@edu.sme.prefeitura.sp.gov.br" },
            new object[] { "Maria Tereza de Santos Jesus", new DateTime(1992, 06, 06),"sjesus.06061992@edu.sme.prefeitura.sp.gov.br" },
            new object[] { "José da Silva", new DateTime(1992, 06, 06) , "josédasilva.06061992@edu.sme.prefeitura.sp.gov.br" }
        };

        public static readonly object[][] DadosParaValidacaoDeEmailValidoPorTentativa =
        {
            new object[] { 0, "José da Silva", new DateTime(1992, 06, 06) , "josesilva.06061992@edu.sme.prefeitura.sp.gov.br" },
            new object[] { 1, "José da Silva", new DateTime(1992, 06, 06) , "jose.silva.06061992@edu.sme.prefeitura.sp.gov.br" },
            new object[] { 2, "José da Silva", new DateTime(1992, 06, 06) , "jose_silva.06061992@edu.sme.prefeitura.sp.gov.br" },
            new object[] { 3, "José da Silva", new DateTime(1992, 06, 06) , "jose-silva.06061992@edu.sme.prefeitura.sp.gov.br" }
        };

        public static readonly object[][] DadosParaValidacaoDefinicaoDeNome =
        {
            new object[] { 1234567, "Maria de Jesus", "", new DateTime(1992, 06, 06), "Maria de Jesus", "mariajesus.06061992@edu.sme.prefeitura.sp.gov.br" },
            new object[] { 1234567, "Maria de Jesus", "José da Silva", new DateTime(1992, 06, 06), "José da Silva", "josesilva.06061992@edu.sme.prefeitura.sp.gov.br" },
            new object[] { 1234567, "Maria de Jesus", "Maria Jesus", new DateTime(1992, 06, 06), "Maria de Jesus", "mariajesus.06061992@edu.sme.prefeitura.sp.gov.br" },
            new object[] { 1234567, "Maria de Jesus", "Maria", new DateTime(1992, 06, 06), "Maria de Jesus", "mariajesus.06061992@edu.sme.prefeitura.sp.gov.br" },
            new object[] { 1234567, "José da Silva", "José Gonçalves", new DateTime(1992, 06, 06), "José da Silva", "josesilva.06061992@edu.sme.prefeitura.sp.gov.br" }
        };
    }
}
