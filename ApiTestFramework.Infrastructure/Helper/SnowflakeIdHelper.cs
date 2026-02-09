namespace ApiTestFramework.Infrastructure.Helper;

/// <summary>
/// 雪花ID生成器
/// 生成的ID格式示例：2016041201692626944
/// </summary>
public static class SnowflakeIdGenerator
{
    // 配置常量
    private const long Twepoch = 1288834974657L;
    private const int WorkerIdBits = 5;
    private const int DatacenterIdBits = 5;
    private const int SequenceBits = 12;

    // 最大值
    private const long MaxSequence = -1L ^ (-1L << SequenceBits);

    // 移位偏移量
    private const int WorkerIdShift = SequenceBits;
    private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
    private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

    // 静态字段
    private static long _lastTimestamp = -1L;
    private static long _sequence = 0L;
    private static readonly long _workerId = 1;
    private static readonly long _datacenterId = 1;
    private static readonly object _lock = new();

    /// <summary>
    /// 生成下一个ID
    /// </summary>
    /// <returns>雪花ID（长整型）</returns>
    public static long NextId()
    {
        lock (_lock)
        {
            var timestamp = GetCurrentTimestamp();

            if (timestamp < _lastTimestamp)
            {
                throw new Exception($"Clock moved backwards. Refusing to generate ID for {_lastTimestamp - timestamp} milliseconds");
            }

            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & MaxSequence;

                if (_sequence == 0)
                {
                    timestamp = GetNextMillisecond(_lastTimestamp);
                }
            }
            else
            {
                _sequence = 0L;
            }

            _lastTimestamp = timestamp;

            return ((timestamp - Twepoch) << TimestampLeftShift)
                   | (_datacenterId << DatacenterIdShift)
                   | (_workerId << WorkerIdShift)
                   | _sequence;
        }
    }

    private static long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static long GetNextMillisecond(long lastTimestamp)
    {
        var timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp)
        {
            Thread.Sleep(1);
            timestamp = GetCurrentTimestamp();
        }
        return timestamp;
    }
}
