using System;

namespace Polecola.Youtube.Api.Utils;

public class TimeSpan
{
    /// <summary>
    /// Parse mm:ss, hh:mm:ss, dd:hh:mm:ss
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    public static System.TimeSpan Parse(string timeSpan)
    {
        string[] timeParts = timeSpan.Split(':');
        var timeNumbers = Array.ConvertAll(timeParts, int.Parse);
        return timeNumbers.Length switch
        {
            2 => new System.TimeSpan(0, 0, timeNumbers[0], timeNumbers[1]),
            3 => new System.TimeSpan(0, timeNumbers[0], timeNumbers[1], timeNumbers[2]),
            4 => new System.TimeSpan(timeNumbers[0], timeNumbers[1], timeNumbers[2], timeNumbers[3]),
            _ => throw new ArgumentException("Invalid time span format", nameof(timeSpan))
        };
    }
}