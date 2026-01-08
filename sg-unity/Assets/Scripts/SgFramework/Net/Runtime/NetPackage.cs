using Cysharp.Threading.Tasks;
using Google.Protobuf;

namespace SgFramework.Net
{
    public struct NetPackage
    {
        public int MsgId { get; set; }
        public byte[] MsgName { get; set; }
        public IMessage Msg { get; set; }
        
        public UniTaskCompletionSource<IMessage> CompletionSource { get; set; }

        public UniTask<IMessage> Task => CompletionSource.Task;
    }
}