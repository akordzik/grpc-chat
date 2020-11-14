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

            Console.WriteLine("Introduce yourself.");
            var name = Console.ReadLine();
            Console.WriteLine("Which room would you like to join?");
            var roomNo = Console.ReadLine();

            var metadata = new Metadata
            {
                new Metadata.Entry("name", name),
                new Metadata.Entry("roomNumber", roomNo)
            };
            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            var options = new CallOptions(metadata, cancellationToken: ct);
            var context = new CallContext(options);

            var responseStream = client.JoinChat(GetMessagesAsync(cts), context);
            try
            {
                await foreach (var line in responseStream.WithCancellation(ct))
                {
                    Console.WriteLine(line.Message);
                }
            }
            catch (RpcException){}
        }

        static async IAsyncEnumerable<ChatRequest> GetMessagesAsync(CancellationTokenSource cts)
        {
            var line = Console.ReadLine();

            while (line != "quit")
            {
                yield return new ChatRequest
                {
                    Message = line
                };
                line = Console.ReadLine();
            }
            cts.Cancel();
        }
    }
}