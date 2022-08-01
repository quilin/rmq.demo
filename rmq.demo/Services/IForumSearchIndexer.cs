using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using rmq.demo.Models;

namespace rmq.demo.Services;

public interface IForumSearchIndexer
{
    Task Index(params SearchEntity[] entities);
    Task<IEnumerable<SearchEntity>> Find(string query);
}

internal class ForumSearchIndexer : IForumSearchIndexer
{
    private readonly IElasticClient elasticClient;

    public ForumSearchIndexer(
        IElasticClient elasticClient)
    {
        this.elasticClient = elasticClient;
    }
    
    public async Task Index(params SearchEntity[] entities)
    {
        await DeclareIndex();
        await elasticClient.IndexManyAsync(entities);
    }

    public async Task<IEnumerable<SearchEntity>> Find(string query)
    {
        await DeclareIndex();
        var searchResponse = await elasticClient.SearchAsync<SearchEntity>(s => s
            .Query(q => q
                .Match(mt => mt.Field(f => f.Text)
                    .Query(query))));
        if (searchResponse is not { IsValid: true })
        {
            throw new Exception();
        }

        return searchResponse.Hits.Select(h => h.Source);
    }

    private async Task DeclareIndex()
    {
        var existsResponse = await elasticClient.Indices.ExistsAsync("search");
        if (existsResponse is { IsValid: true, Exists: true })
        {
            return;
        }

        var response = await elasticClient.Indices.CreateAsync("search", i => i
            .Settings(s => s
                .Analysis(a => a
                    .Analyzers(an => an
                        .Custom("test_analyzer", ca => ca
                            .CharFilters("html_strip")
                            .Tokenizer("standard")
                            .Filters("lowercase", "stop"))
                        .Custom("test_search_analyzer", ca => ca
                            .Tokenizer("standard")
                            .Filters("lowercase", "stop")))))
            .Map<SearchEntity>(m => m
                .AutoMap()
                .Properties(p => p
                    .Text(t => t
                        .Name(n => n.Text)
                        .Analyzer("test_analyzer")
                        .SearchAnalyzer("test_search_analyzer")))));

        if (response is { IsValid: false })
        {
            throw new Exception();
        }
    }
}