using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GrpcChat.Contracts;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;

namespace GrpcChat.Server.Services
{
    public class ChatService : IChatService
    {
        private readonly ILogger<ChatService> _logger;

        public ChatService(ILogger<ChatService> logger)
        {
            _logger = logger;
        }

        public async IAsyncEnumerable<ChatResponse> JoinChat(IAsyncEnumerable<ChatRequest> requestStream,
            CallContext context)
        {
            var name = context.RequestHeaders.Get("name").Value;
            var member = new ChatMember(name);
            _logger.LogInformation($"{member.Id} connected");
            
            await foreach (var request in requestStream)
            {
                yield return new ChatResponse
                {
                    Message = $"{member.Name} says: {request.Message}"
                };
            }
        }
    }
}