using System.Collections.Generic;
using System.ServiceModel;
using ProtoBuf.Grpc;

namespace GrpcChat.Contracts
{
    [ServiceContract(Name = "ChatService")]
    public interface IChatService
    {
        [OperationContract]
        IAsyncEnumerable<ChatResponse> JoinChat(IAsyncEnumerable<ChatRequest> requestStream,
            CallContext context);
    }
}