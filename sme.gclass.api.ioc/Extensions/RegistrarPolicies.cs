﻿using Google;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Registry;
using Sentry;
using SME.GoogleClassroom.Dominio;
using SME.GoogleClassroom.Infra.Politicas;
using System;

namespace SME.GoogleClassroom.IoC
{
    public static class RegistrarPolicies
    {
        public static void AddPolicies(this IServiceCollection services)
        {
            IPolicyRegistry<string> registry = services.AddPolicyRegistry();

            Random jitterer = new Random();
            var policyGSync = Policy.Handle<Exception>(ex => !(ex is GoogleApiException || ex is NegocioException))
              .WaitAndRetryAsync(3,    // exponential back-off plus some jitter
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                      + TimeSpan.FromMilliseconds(jitterer.Next(0, 30)));
            registry.Add(PoliticaPolly.PolicyGoogleSync, policyGSync);

            var policyFilas = Policy.Handle<Exception>()
              .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(5));
            registry.Add(PoliticaPolly.PolicyPublicaFila, policyFilas);

            var policyGSyncRemocaoProfessor = Policy.Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(60),
                    TimeSpan.FromSeconds(60),
                    TimeSpan.FromSeconds(60),
                    TimeSpan.FromSeconds(60),
                    TimeSpan.FromSeconds(60)
                }, (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"RETRY policyGSyncRemocaoProfessor - {retryCount}: {exception.Message}");
                });
            //.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(30));
            registry.Add(PoliticaPolly.PolicyRemocaoProfessor, policyGSyncRemocaoProfessor);

            RegistrarPolicyGsa(registry);
        }

        private static void RegistrarPolicyGsa(IPolicyRegistry<string> registry)
        {
            var policy = Policy.Handle<Exception>()
              .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(60));

            registry.Add(PoliticaPolly.PolicyCargaGsa, policy);
        }
    }
}