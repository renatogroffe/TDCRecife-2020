using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using FunctionAppAcoes.Models;
using FunctionAppAcoes.Documents;

namespace FunctionAppAcoes.Data
{
    public class AcoesRepository
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<AcaoDocument> _collection;

        public AcoesRepository()
        {
            _client = new MongoClient(
                Environment.GetEnvironmentVariable("MongoConnection"));
            _db = _client.GetDatabase(
                Environment.GetEnvironmentVariable("MongoDatabase"));
            _collection = _db.GetCollection<AcaoDocument>(
                Environment.GetEnvironmentVariable("MongoCollection"));
        }

        public void Save(Acao acao)
        {
            _collection.InsertOne(new AcaoDocument()
            {
                Sigla = acao.Codigo,
                Data = acao.UltimaAtualizacao.ToString("yyyy-MM-dd HH:mm:ss"),
                Valor = acao.Valor.Value,
                HistLancamento = acao.Codigo + "-" + acao.UltimaAtualizacao.ToString("yyyyMMdd-HHmmss")
            });
        }

        public List<AcaoDocument> ListAll()
        {
            return _collection.Find(all => true).ToEnumerable()
                .OrderByDescending(d => d.Data).ToList();
        }
    }
}