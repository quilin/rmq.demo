using System;
using System.Collections.Generic;
using System.Linq;
using rmq.demo.Models;

namespace rmq.demo.Services;

public interface IForumDataProvider
{
    ICollection<Forum> GetFora();
    Comment CreateComment(string text);
}

internal class ForumDataProvider : IForumDataProvider
{

    private static readonly string[] Texts =
    {
        "Affronting everything discretion men now own did. Still round match we to. Frankness pronounce daughters remainder extensive has but. Happiness cordially one determine concluded fat. Plenty season beyond by hardly giving of. Consulted or acuteness dejection an smallness if. Outward general passage another as it. Very his are come man walk one next. Delighted prevailed supported too not remainder perpetual who furnished. Nay affronting bed projection compliment instrument.",
        "Pasture he invited mr company shyness. But when shot real her. Chamber her observe visited removal six sending himself boy. At exquisite existence if an oh dependent excellent. Are gay head need down draw. Misery wonder enable mutual get set oppose the uneasy. End why melancholy estimating her had indulgence middletons. Say ferrars demands besides her address. Blind going you merit few fancy their.",
        "Him rendered may attended concerns jennings reserved now. Sympathize did now preference unpleasing mrs few. Mrs for hour game room want are fond dare. For detract charmed add talking age. Shy resolution instrument unreserved man few. She did open find pain some out. If we landlord stanhill mr whatever pleasure supplied concerns so. Exquisite by it admitting cordially september newspaper an. Acceptance middletons am it favourable. It it oh happen lovers afraid.",
        "Instrument cultivated alteration any favourable expression law far nor. Both new like tore but year. An from mean on with when sing pain. Oh to as principles devonshire companions unsatiable an delightful. The ourselves suffering the sincerity. Inhabit her manners adapted age certain. Debating offended at branched striking be subjects.",
        "View fine me gone this name an rank. Compact greater and demands mrs the parlors. Park be fine easy am size away. Him and fine bred knew. At of hardly sister favour. As society explain country raising weather of. Sentiments nor everything off out uncommonly partiality bed.",
        "To sorry world an at do spoil along. Incommode he depending do frankness remainder to. Edward day almost active him friend thirty piqued. People as period twenty my extent as. Set was better abroad ham plenty secure had horses. Admiration has sir decisively excellence say everything inhabiting acceptance. Sooner settle add put you sudden him.",
        "On insensible possession oh particular attachment at excellence in. The books arose but miles happy she. It building contempt or interest children mistress of unlocked no. Offending she contained mrs led listening resembled. Delicate marianne absolute men dashwood landlord and offended. Suppose cottage between and way. Minuter him own clothes but observe country. Agreement far boy otherwise rapturous incommode favourite.",
        "In entirely be to at settling felicity. Fruit two match men you seven share. Needed as or is enough points. Miles at smart ﻿no marry whole linen mr. Income joy nor can wisdom summer. Extremely depending he gentleman improving intention rapturous as.",
        "Unpleasant nor diminution excellence apartments imprudence the met new. Draw part them he an to he roof only. Music leave say doors him. Tore bred form if sigh case as do. Staying he no looking if do opinion. Sentiments way understood end partiality and his.",
        "Is at purse tried jokes china ready decay an. Small its shy way had woody downs power. To denoting admitted speaking learning my exercise so in. Procured shutters mr it feelings. To or three offer house begin taken am at. As dissuade cheerful overcame so of friendly he indulged unpacked. Alteration connection to so as collecting me. Difficult in delivered extensive at direction allowance. Alteration put use diminution can considered sentiments interested discretion. An seeing feebly stairs am branch income me unable.",
    };

    private static readonly string[] Users =
    {
        "Zachary Peterson",
        "Jarrett Bates",
        "Jair Dudley",
        "Laurel Chang",
        "Jay Dixon",
        "Shelby Rollins",
        "Thaddeus Bernard",
        "Kaiya Yu",
        "Jacob Tyler",
    };

    private static readonly string[] Titles =
    {
        "Yellow Menace 666",
        "Devil Blade Zero",
        "Ms Piggy Revenge Alpha",
        "Angel",
        "Sentinel Torrential Terry",
        "BallisticFury",
        "Warden Addicted 2 Coins",
        "Mercedes man",
        "SpitFire",
        "Mustang",
    };

    private static readonly Random Random = new();

    private static readonly ICollection<Forum> Fora = new[]
    {
        new Forum(Guid.NewGuid(), "General", new List<Topic>
        {
            GetTopic(),
            GetTopic(),
            GetTopic(),
        }),
        new Forum(Guid.NewGuid(), "Bug reports", new List<Topic>
        {
            GetTopic(),
            GetTopic(),
            GetTopic(),
            GetTopic(),
        }),
        new Forum(Guid.NewGuid(), "Donations", new List<Topic>()),
    };

    public ICollection<Forum> GetFora() => Fora;
    public Comment CreateComment(string text)
    {
        var comment = GetComment();
        comment.Text = text;
        return comment;
    }
    private static string GetTitle() => Titles[Random.Next(0, Titles.Length - 1)];
    private static string GetText() => Texts[Random.Next(0, Texts.Length - 1)];
    private static string GetUserName() => Users[Random.Next(0, Users.Length - 1)];

    private static DateTimeOffset GetMoment() => new(
        2022,
        Random.Next(4, 7),
        Random.Next(1, 30),
        Random.Next(5, 23),
        Random.Next(0, 59),
        Random.Next(0, 59),
        TimeSpan.FromHours(Random.Next(-3, 3)));

    private static Comment GetComment() => new(Guid.NewGuid(), GetUserName(), GetMoment(), new List<string>())
    {
        Text = GetText()
    };

    private static Topic GetTopic() => new(
        Guid.NewGuid(), GetUserName(), GetMoment(),
        Enumerable.Repeat(0, Random.Next(0, 5)).Select(_ => GetComment()).ToList())
    {
        Title = GetTitle()
    };
}