using System;

namespace GrpcChat.Server.Services
{
    public class ChatMember
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; }

        public ChatMember(string name)
        {
            Name = name;
        }
    }
}