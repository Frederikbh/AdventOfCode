using System.Text.RegularExpressions;

using AdventOfCode.Lib;

// ReSharper disable StringLiteralTypo

namespace AdventOfCode._2017.Day16;

[ProblemName("Permutation Promenade")]
public partial class Solution : ISolver
{
    public object PartOne(string input)
    {
        const string Dancers = "abcdefghijklmnop";
        var dance = ParseInput(input);

        return dance(Dancers);
    }

    public object PartTwo(string input)
    {
        var dance = ParseInput(input);
        var dancers = "abcdefghijklmnop";

        const int Performances = 1_000_000_000;
        var circular = CircularDance(dance, dancers);

        for (var i = 0; i < Performances % circular; i++)
        {
            dancers = dance(dancers);
        }

        return dancers;
    }

    private static int CircularDance(Func<string, string> dance, string dancers)
    {
        var curState = dancers;

        for (var i = 0;; i++)
        {
            curState = dance(curState);

            if (curState == dancers)
            {
                return i + 1;
            }
        }
    }

    private static Func<string, string> ParseInput(string input)
    {
        var functions = new List<Func<char[], char[]>>();
        var segments = input.Split(',');

        foreach (var segment in segments)
        {
            var function = ParseSpin(segment)
                ?? ParseExchange(segment)
                ?? ParsePartner(segment);

            functions.Add(function!);
        }

        return str =>
        {
            var chars = str.ToCharArray();

            foreach (var function in functions)
            {
                chars = function(chars);
            }

            return new string(chars);
        };
    }

    private static Func<char[], char[]>? ParseSpin(string move)
    {
        var match = SpinRegex()
            .Match(move);

        if (!match.Success)
        {
            return null;
        }

        var n = int.Parse(match.Groups[1].Value);

        return chars =>
        {
            var result = new char[chars.Length];
            Array.Copy(chars, chars.Length - n, result, 0, n);
            Array.Copy(chars, 0, result, n, chars.Length - n);

            return result;
        };
    }

    private static Func<char[], char[]>? ParseExchange(string move)
    {
        var match = ExchangeRegex()
            .Match(move);

        if (!match.Success)
        {
            return null;
        }

        var a = int.Parse(match.Groups[1].Value);
        var b = int.Parse(match.Groups[2].Value);

        return chars =>
        {
            (chars[a], chars[b]) = (chars[b], chars[a]);

            return chars;
        };
    }

    private static Func<char[], char[]>? ParsePartner(string move)
    {
        var match = PartnerRegex()
            .Match(move);

        if (!match.Success)
        {
            return null;
        }

        var a = match.Groups[1]
            .Value[0];
        var b = match.Groups[2]
            .Value[0];

        return chars =>
        {
            var aIndex = Array.IndexOf(chars, a);
            var bIndex = Array.IndexOf(chars, b);
            (chars[aIndex], chars[bIndex]) = (chars[bIndex], chars[aIndex]);

            return chars;
        };
    }

    [GeneratedRegex(@"s(\d+)")]
    private static partial Regex SpinRegex();

    [GeneratedRegex(@"x(\d+)/(\d+)")]
    private static partial Regex ExchangeRegex();

    [GeneratedRegex(@"p(\w)/(\w)")]
    private static partial Regex PartnerRegex();
}
