using System.Threading;
using System.Threading.Tasks;
using Akasha.Data;

namespace Akasha.Infrastructure.Kafka
{
    /// <summary>
    /// Abstraction for handling match results.
    /// </summary>
    public interface IMatchResultProducer
    {
        /// <summary>
        /// Asynchronously sends data somewhere.
        /// Implementation decides how to handle MatchResult (protobuf, json, etc).
        /// </summary>
        /// <param name="result"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SendAsync(MatchResult result, CancellationToken token = default);
    }
}
