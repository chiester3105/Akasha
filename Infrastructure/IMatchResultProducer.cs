using System.Threading;
using System.Threading.Tasks;
using Akasha.Contracts;

namespace Akasha.Infrastructure
{
    /// <summary>
    /// Abstraction for handling match results.
    /// </summary>
    public interface IMatchResultProducer
    {
        /// <summary>
        /// Asynchronously sends data somewhere.
        /// Implementation decides how to handle result (protobuf, json, etc).
        /// </summary>
        /// <param name="record"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SendAsync(MatchRecord record, CancellationToken token = default);
    }
}
