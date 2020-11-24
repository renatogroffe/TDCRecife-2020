using System;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using FunctionAppAcoes.Data;
using FunctionAppAcoes.Models;
using FunctionAppAcoes.Validators;

namespace FunctionAppAcoes
{
    public class AcoesSqlServiceBusTopicTrigger
    {
        private readonly AcoesContext _context;

        public AcoesSqlServiceBusTopicTrigger(AcoesContext context)
        {
            _context = context;
        }

        [FunctionName("AcoesSqlServiceBusTopicTrigger")]
        public void Run([ServiceBusTrigger("topic-acoes", "sqlserver0", Connection = "AzureServiceBus")]string mySbMsg, ILogger log)
        {
            log.LogInformation($"AcoesSqlServiceBusTopicTrigger - Dados: {mySbMsg}");

            DadosAcao dadosAcao = null;
            try
            {
                dadosAcao = JsonSerializer.Deserialize<DadosAcao>(mySbMsg,
                    new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                log.LogError("AcoesSqlServiceBusTopicTrigger - Erro durante a deserializacao!");
            }

            if (dadosAcao != null)
            {
                var validationResult = new AcaoValidator().Validate(dadosAcao);
                if (validationResult.IsValid)
                {
                    log.LogInformation($"AcoesSqlServiceBusTopicTrigger - Dados pos formatacao: {JsonSerializer.Serialize(dadosAcao)}");

                    _context.Acoes.Add(new Acao()
                    {
                        Codigo = dadosAcao.Codigo,
                        DataReferencia = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Valor = dadosAcao.Valor.Value
                    });
                    _context.SaveChanges();

                    log.LogInformation("AcoesSqlServiceBusTopicTrigger - Acao registrada com sucesso!");
                }
                else
                {
                    log.LogError("AcoesSqlServiceBusTopicTrigger - Dados invalidos para a Acao");
                    foreach (var error in validationResult.Errors)
                        log.LogError($"AcoesSqlServiceBusTopicTrigger - {error.ErrorMessage}");
                }
            }
        }
    }
}