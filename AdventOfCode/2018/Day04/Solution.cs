using System.Text.RegularExpressions;

using AdventOfCode.Lib;

namespace AdventOfCode._2018.Day04;

[ProblemName("Repose Record")]
public partial class Solution : ISolver
{
    public object PartOne(string input)
    {
        var records = ParseInput(input);

        var guardSleepTimes = GetGuardSleepTimes(records);
        var mostSleepyGuard = GetMostSleepyGuard(guardSleepTimes);
        var guardSleepMinutes = GetGuardSleepMinutes(records);
        var mostSleepyMinute = GetMostSleepyMinute(guardSleepMinutes[mostSleepyGuard]);

        return mostSleepyMinute * mostSleepyGuard;
    }

    public object PartTwo(string input)
    {
        var records = ParseInput(input);

        var guardSleepMinutes = GetGuardSleepMinutes(records);

        var (mostSleepyGuard, mostSleepyMinute) = GetGuardWithMostSleepyMinute(guardSleepMinutes);

        return mostSleepyGuard * mostSleepyMinute;
    }

    private static int GetMostSleepyMinute(Dictionary<int, int> guardSleepMinutes) =>
        guardSleepMinutes
            .OrderByDescending(kvp => kvp.Value)
            .FirstOrDefault()
            .Key;

    private static Dictionary<int, Dictionary<int, int>> GetGuardSleepMinutes(Record[] records)
    {
        var guardSleepMinutes = new Dictionary<int, Dictionary<int, int>>();
        var currentGuard = 0;
        var sleepStart = DateTime.MinValue;

        foreach (var record in records)
        {
            if (record.GuardId.HasValue)
            {
                currentGuard = record.GuardId.Value;
                if (!guardSleepMinutes.ContainsKey(currentGuard))
                {
                    guardSleepMinutes[currentGuard] = new Dictionary<int, int>();
                }
            }
            else if (record.Action == "falls asleep")
            {
                sleepStart = record.Time;
            }
            else if (record.Action == "wakes up")
            {
                for (var i = sleepStart.Minute; i < record.Time.Minute; i++)
                {
                    if (!guardSleepMinutes[currentGuard].ContainsKey(i))
                    {
                        guardSleepMinutes[currentGuard][i] = 0;
                    }
                    guardSleepMinutes[currentGuard][i]++;
                }
            }
        }

        return guardSleepMinutes;
    }

    private static int GetMostSleepyGuard(Dictionary<int, int> guardSleepTimes) =>
        guardSleepTimes
            .OrderByDescending(kvp => kvp.Value)
            .First()
            .Key;

    private static Dictionary<int, int> GetGuardSleepTimes(Record[] records)
    {
        var guardSleepTimes = new Dictionary<int, int>();
        var currentGuard = 0;
        var sleepStart = DateTime.MinValue;

        foreach (var record in records)
        {
            if (record.GuardId.HasValue)
            {
                currentGuard = record.GuardId.Value;
                guardSleepTimes.TryAdd(currentGuard, 0);
            }
            else if (record.Action == "falls asleep")
            {
                sleepStart = record.Time;
            }
            else if (record.Action == "wakes up")
            {
                guardSleepTimes[currentGuard] += (int)(record.Time - sleepStart).TotalMinutes;
            }
        }

        return guardSleepTimes;
    }

    private static Record[] ParseInput(string input)
    {
        var regex = RecordRegex();

        return input
            .Split("\n")
            .Select(
                line =>
                {
                    var match = regex.Match(line);

                    if (!match.Success)
                    {
                        throw new Exception($"Could not parse line: {line}");
                    }

                    var time = DateTime.Parse(match.Groups[1].Value);
                    var action = match.Groups[2].Value;
                    var guardId = match.Groups[3].Success
                        ? int.Parse(match.Groups[3].Value)
                        : (int?)null;

                    return new Record(time, action, guardId);
                })
            .OrderBy(record => record.Time)
            .ToArray();
    }

    private static (int guardId, int minute) GetGuardWithMostSleepyMinute(Dictionary<int, Dictionary<int, int>> guardSleepMinutes)
    {
        var mostSleepyGuard = 0;
        var mostSleepyMinute = 0;
        var mostSleep = 0;

        foreach (var guard in guardSleepMinutes)
        {
            var minute = GetMostSleepyMinute(guard.Value);
            var sleep = guard.Value.GetValueOrDefault(minute);

            if (sleep > mostSleep)
            {
                mostSleepyGuard = guard.Key;
                mostSleepyMinute = minute;
                mostSleep = sleep;
            }
        }

        return (mostSleepyGuard, mostSleepyMinute);
    }

    [GeneratedRegex(@"\[(\d{4}-\d{2}-\d{2} \d{2}:\d{2})\] (Guard #(\d+) begins shift|falls asleep|wakes up)")]
    private static partial Regex RecordRegex();

    private record struct Record(DateTime Time, string Action, int? GuardId);
}
