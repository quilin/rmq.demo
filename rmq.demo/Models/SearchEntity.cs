using System;

namespace rmq.demo.Models;

public class SearchEntity
{
    public Guid Id { get; set; }
    public SearchEntityType EntityType { get; set; }
    public string Text { get; set; }
}

public enum SearchEntityType
{
    Forum = 0,
    Topic = 1,
    Comment = 2,
}