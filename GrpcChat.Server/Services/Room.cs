using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcChat.Server.Services
{
    public class Room
    {
        public ConcurrentDictionary<Guid, ChatMember> Members { get; } = new ConcurrentDictionary<Guid, ChatMember>();
    }
}