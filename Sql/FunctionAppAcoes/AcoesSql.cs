using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FunctionAppAcoes.Data;

namespace FunctionAppAcoes
{
    public class AcoesSql
    {
        private readonly AcoesContext _context;

        public AcoesSql(AcoesContext context)
        {
            _context = context;
        }

        [FunctionName("AcoesSql")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var listaAcoes = _context.Acoes
                .OrderByDescending(a => a.Id).ToArray();
            log.LogInformation(
                $"AcoesSql HTTP trigger - número atual de lançamentos: {listaAcoes.Count()}");
            return new OkObjectResult(listaAcoes);
        }
    }
}