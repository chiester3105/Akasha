using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Akasha.Contracts;
using Confluent.Kafka;
using ProtoBuf;
using UnityEngine;

namespace Akasha.Infrastructure.Kafka
{
    public class KafkaMatchResultProducer : IMatchResultProducer, IDisposable
    {
        private readonly IProducer<string, byte[]> _producer;
        private readonly string _topic;
        private bool _disposed;

        public KafkaMatchResultProducer(string bootstrapServers, string topic, string serverId)
        {
            _topic = topic ?? throw new ArgumentNullException(nameof(topic));
            
            var config = new ProducerConfig()
            {
                BootstrapServers = bootstrapServers,
                ClientId = serverId,
                Acks = Acks.All,
                EnableIdempotence = true,
                CompressionType = Confluent.Kafka.CompressionType.Snappy,
                MessageSendMaxRetries = 3,
                LingerMs = 5
            };

            _producer = new ProducerBuilder<string, byte[]>(config).Build();
            AkashaPlugin.Logger.LogInfo($"Kafka producer initialized for topic '{_topic}' at {bootstrapServers}");
        }

        public async Task SendAsync(MatchRecord record, CancellationToken token = default)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            if (_disposed) throw new ObjectDisposedException(nameof(KafkaMatchResultProducer));

            try
            {
                byte[] bytes;
                using (var ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, record);
                    bytes = ms.ToArray();
                }

                var msg = new Message<string, byte[]>
                {
                    Key = record.MatchId,
                    Value = bytes,
                    Timestamp = new Timestamp(DateTime.UtcNow)
                };

                var deliveryResult = await _producer.ProduceAsync(_topic, msg, token);

                AkashaPlugin.Logger.LogInfo($"Match sent: {record.MatchId}," +
                    $"\npartition: {deliveryResult.Partition}" +
                    $"\noffset: {deliveryResult.Offset}");
            }
            catch (Exception ex)
            {
                AkashaPlugin.Logger.LogError($"Failed to send match: {record.MatchId}." +
                    $"\nException: {ex}");
                SaveLocally(record);
            }
        }
        public void Dispose()
        {
            if (_disposed) return;
            _producer?.Flush(TimeSpan.FromSeconds(10));
            _producer?.Dispose();
            _disposed = true;
        }

        public void SaveLocally(MatchRecord record)
        {

        }
    }
}
