// SnowflakeIdGenerator.cs
using System;
using System.Threading;

namespace ApiTestFramework.Helper;

/// <summary>
/// 雪花ID生成器
/// 生成的ID格式示例：2016041201692626944
/// </summary>
public class SnowflakeIdGenerator
{
    // 配置常量
    private const long Twepoch = 1288834974657L; // 起始时间戳（2010-11-04 09:42:54.657 UTC）
    private const int WorkerIdBits = 5;          // 机器ID位数
    private const int DatacenterIdBits = 5;      // 数据中心ID位数
    private const int SequenceBits = 12;         // 序列号位数

    // 最大值
    private const long MaxSequence = -1L ^ (-1L << SequenceBits);         // 最大序列号：4095

    // 移位偏移量
    private const int WorkerIdShift = SequenceBits;                       // 机器ID左移位数：12
    private const int DatacenterIdShift = SequenceBits + WorkerIdBits;    // 数据中心ID左移位数：17
    private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits; // 时间戳左移位数：22

    // 字段
    private long _lastTimestamp = -1L;
    private long _sequence = 0L;

    private readonly long _workerId=1;
    private readonly long _datacenterId=1;
    private readonly object _lock = new object();


    /// <summary>
    /// 生成下一个ID
    /// </summary>
    /// <returns>雪花ID（长整型）</returns>
    public  long NextId()
    {
        lock (_lock)
        {
            var timestamp = GetCurrentTimestamp();

            // 如果当前时间小于上次时间，说明时钟回拨
            if (timestamp < _lastTimestamp)
            {
                throw new Exception($"Clock moved backwards. Refusing to generate ID for {_lastTimestamp - timestamp} milliseconds");
            }

            // 如果是同一毫秒生成的，则序列号递增
            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & MaxSequence;

                // 序列号超出范围，等待下一毫秒
                if (_sequence == 0)
                {
                    timestamp = GetNextMillisecond(_lastTimestamp);
                }
            }
            else
            {
                // 不同毫秒，序列号重置
                _sequence = 0L;
            }

            _lastTimestamp = timestamp;

            // 生成ID
            return ((timestamp - Twepoch) << TimestampLeftShift)
                   | (_datacenterId << DatacenterIdShift)
                   | (_workerId << WorkerIdShift)
                   | _sequence;
        }
    }



    /// <summary>
    /// 获取当前时间戳（毫秒）
    /// </summary>
    private long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 获取下一毫秒的时间戳
    /// </summary>
    private long GetNextMillisecond(long lastTimestamp)
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
