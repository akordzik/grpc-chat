using System.Runtime.Serialization;

namespace GrpcChat.Contracts
{
    [DataContract]
    public class ChatResponse
    {
        [DataMember(Order = 1)] public string Message { get; set; }
    }
}