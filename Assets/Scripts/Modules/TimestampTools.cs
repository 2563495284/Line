using System;
using UnityEngine;

/// <summary>
/// 时间戳与字符串转换工具类
/// </summary>
public static class TimestampTool
{
    /// <summary>
    /// Unix 纪元时间（1970-01-01 00:00:00 UTC）
    /// </summary>
    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    #region 时间戳转时间字符串
    /// <summary>
    /// 毫秒级时间戳转时间字符串（默认格式：yyyy-MM-dd HH:mm:ss.fff）
    /// </summary>
    /// <param name="millisTimeStamp">毫秒级时间戳（13位）</param>
    /// <param name="format">自定义格式（如 "yyyy-MM-dd HH:mm:ss"）</param>
    /// <returns>可读时间字符串</returns>
    public static string FormatMillTime(long millisTimeStamp, string format = "yyyy-MM-dd HH:mm:ss.fff")
    {
        // 1. 毫秒级时间戳 → DateTime（UTC 时间）
        DateTime utcTime = UnixEpoch.AddMilliseconds(millisTimeStamp);
        // 2. 转换为本地时间（适配当前时区，如北京时间）
        DateTime localTime = utcTime.ToLocalTime();
        // 3. 格式化为字符串
        return localTime.ToString(format);
    }

    #endregion

    #region 获取当前时间戳（辅助功能）
    /// <summary>
    /// 获取当前毫秒级时间戳（13位）
    /// </summary>
    public static long Now()
    {
        TimeSpan span = DateTime.UtcNow - UnixEpoch;
        return (long)span.TotalMilliseconds;
    }

    #endregion

    #region 时间字符串转时间戳（反向转换，可选）
    /// <summary>
    /// 时间字符串转毫秒级时间戳
    /// </summary>
    /// <param name="timeStr">时间字符串（如 "2024-06-01 12:00:00"）</param>
    /// <param name="format">字符串格式（需与 timeStr 匹配）</param>
    public static long TimeStrToMillis(string timeStr, string format = "yyyy-MM-dd HH:mm:ss")
    {
        if (DateTime.TryParseExact(timeStr, format, null, System.Globalization.DateTimeStyles.None, out DateTime localTime))
        {
            DateTime utcTime = localTime.ToUniversalTime();
            TimeSpan span = utcTime - UnixEpoch;
            return (long)span.TotalMilliseconds;
        }
        Debug.LogError($"时间字符串格式错误！请确保格式「{format}」与输入「{timeStr}」匹配");
        return 0;
    }
    #endregion
}