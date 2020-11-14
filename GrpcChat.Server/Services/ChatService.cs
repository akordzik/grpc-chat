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
        private static readonly ConcurrentDictionary<int, Lazy<Room>> Rooms =
            new ConcurrentDictionary<int, Lazy<Room>>();
        
        private readonly ILogger<ChatService> _logger;

        public ChatService(ILogger<ChatService> logger)
        {
            _logger = logger;
        }

        public async IAsyncEnumerable<ChatResponse> JoinChat(IAsyncEnumerable<ChatRequest> requestStream,
            CallContext context)
        {
            var (name, roomNo) = ReadParameters();
            
            var room = Rooms.GetOrAdd(roomNo, new Lazy<Room>()).Value;
            var member = new ChatMember(name);
            var added = room.Members.TryAdd(member.Id, member);

            while (!added)
            {
                added = room.Members.TryAdd(member.Id, member);
            }
            
            _logger.LogInformation($"{member.Id} connected to room {roomNo}");
            
            yield return new ChatResponse
            {
                Message = $"Welcome to Room {roomNo}!"
            };
            
            await foreach (var request in requestStream)
            {
                yield return new ChatResponse
                {
                    Message = $"{member.Name} says: {request.Message}"
                };
            }

            (string name, int roomNo) ReadParameters() =>
                (context.RequestHeaders.Get("name").Value, int.Parse(context.RequestHeaders.Get("roomnumber").Value));
        }
    }
}