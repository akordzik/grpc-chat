using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcChat.Contracts;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;

namespace GrpcChat.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;
            var channel = GrpcChannel.ForAddress("http://localhost:9696");
            var client = channel.CreateGrpcService<IChatService>();

            var responseStream = client.JoinChat(GetMessagesAsync(), new CallContext());
            await foreach (var line in responseStream)
            {
                Console.WriteLine(line.Message);
            }
        }

        static async IAsyncEnumerable<ChatRequest> GetMessagesAsync()
        {
            var line = Console.ReadLine();

            while (true)
            {
                yield return new ChatRequest
                {
                    Message = line
                };
                line = Console.ReadLine();
            }
        }
    }
}