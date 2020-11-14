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
            var token = context.CancellationToken;
            var (name, roomNo) = ReadParameters();
            
            var room = Rooms.GetOrAdd(roomNo, new Lazy<Room>()).Value;
            var member = new ChatMember(name);
            var added = room.Members.TryAdd(member.Id, member);

            while (!added)
            {
                added = room.Members.TryAdd(member.Id, member);
            }
            
            _logger.LogInformation($"{member.Id} connected to room {roomNo}");
            await room.EnqueueMessage(member.Id, $"Welcome to Room {roomNo}!", token);
            await room.EnqueueMessage($"{member.Name} joined the room.", token);
            
            var task = Task.Run(ReadMessages, token);
            var reader = member.Messages.Reader;
            
            while (await AwaitMessage())
            {
                if (!reader.TryRead(out var msg))
                {
                    continue;
                }

                yield return new ChatResponse
                {
                    Message = msg
                };
            }
            
            room.Members.Remove(member.Id, out _);
            await room.EnqueueMessage($"{member.Name} left the room.", CancellationToken.None);
            await task;

            (string name, int roomNo) ReadParameters() =>
                (context.RequestHeaders.Get("name").Value, int.Parse(context.RequestHeaders.Get("roomnumber").Value));

            async ValueTask ReadMessages()
            {
                try
                {
                    await foreach (var x in requestStream.WithCancellation(token))
                    {
                        await room.EnqueueMessage($"{member.Name} says: {x.Message}", token);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation($"{member.Id} disconnected from room {roomNo}");
                }
            }

            async ValueTask<bool> AwaitMessage()
            {
                try
                {
                    return await reader.WaitToReadAsync(token);
                }
                catch (OperationCanceledException)
                {
                    return false;
                }
            }
        }
    }
}