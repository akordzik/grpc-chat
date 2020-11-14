using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcChat.Server.Services
{
    public class Room
    {
        public ConcurrentDictionary<Guid, ChatMember> Members { get; } = new ConcurrentDictionary<Guid, ChatMember>();

        public async ValueTask EnqueueMessage(string msg, CancellationToken ct)
        {
            foreach (var chatMember in Members)
            {
                await chatMember.Value.Messages.Writer.WriteAsync(msg, ct);
            }
        }
    }
}