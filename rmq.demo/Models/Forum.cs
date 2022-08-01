using System;
using System.Collections.Generic;

namespace rmq.demo.Models;

public record Forum(Guid Id, string Title, ICollection<Topic> Topics);

public record Topic(Guid Id, string Author, DateTimeOffset CreatedAt, ICollection<Comment> Comments)
{
    public string Title { get; set; }
};

public record Comment(Guid Id, string Author, DateTimeOffset CreatedAt, ICollection<string> LikedBy)
{
    public string Text { get; set; }
};