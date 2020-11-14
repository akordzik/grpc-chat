using System;
using System.Threading.Channels;

namespace GrpcChat.Server.Services
{
    public class ChatMember
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; }
        public Channel<string> Messages { get; } = Channel.CreateUnbounded<string>();

        public ChatMember(string name)
        {
            Name = name;
        }
    }
}