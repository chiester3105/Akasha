using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Akasha.Contracts;
using Confluent.Kafka;
using Cysharp.Threading.Tasks;
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

            //protobuf serialization
            byte[] payload;
            try
            {
                using (var ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, record);
                    payload = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                AkashaPlugin.Logger.LogError($"Serialization failed: {ex}");
                throw;
            }

            //kafka msg
            var msg = new Message<string, byte[]>
            {
                Key = record.MatchId,
                Value = payload,
                Timestamp = new Timestamp(DateTime.UtcNow)
            };

            //sending msg
            try
            {
                var deliveryResult = await _producer.ProduceAsync(_topic, msg, token);

                AkashaPlugin.Logger.LogInfo($"Match sent: {record.MatchId}," +
                    $"\npartition: {deliveryResult.Partition}" +
                    $"\noffset: {deliveryResult.Offset}");
            }
            catch (Exception ex)
            {
                AkashaPlugin.Logger.LogError($"Failed to send match: {record.MatchId}." +
                    $"\nException: {ex}");

                SaveLocally(record.MatchId, payload, token).Forget();
            }
        }
        public void Dispose()
        {
            if (_disposed) return;
            _producer?.Flush(TimeSpan.FromSeconds(10));
            _producer?.Dispose();
            _disposed = true;
        }

        //fallback 
        private async UniTask SaveLocally(string id, byte[] data, CancellationToken ct)
        {
            try
            {
                await UniTask.RunOnThreadPool(() =>
                {
                    var fallbackDir = AkashaPlugin.MessagesPath;

                    if (!Directory.Exists(fallbackDir))
                    {
                        Directory.CreateDirectory(fallbackDir);
                    }

                    var fileName = $"{id}_{DateTime.UtcNow:yyyyMMddHHmmss}.bin";
                    var path = Path.Combine(fallbackDir, fileName);

                    File.WriteAllBytes(path, data);
                }, cancellationToken: ct);

                AkashaPlugin.Logger.LogWarning($"Fallback saved for match {id}");
            }
            catch (Exception ex)
            {
                AkashaPlugin.Logger.LogError($"Failed to save fallback for match {id}:\n" +
                    $"{ex}");
            }
        }
    }
}
