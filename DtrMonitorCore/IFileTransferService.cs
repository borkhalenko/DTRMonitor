using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace DtrMonitorCore {
    [ServiceContract]
    public interface IFileTransferService {
        [OperationContract]
        bool hasNext();
        [OperationContract]
        void RemoveFileFromServer(string fileName);
        [OperationContract]
        RemoteFileData ReceiveNextFromServer();
        [OperationContract]
        RemoteFileHash SendNextToServer(RemoteFileData fData);
    }

    [MessageContract]
    public class RemoteFileHash {
        [MessageBodyMember]
        public string fileHash;
    }

    [MessageContract]
    public class RemoteFileData: IDisposable {
        [MessageHeader(MustUnderstand = true)]
        public string fileName;
        [MessageHeader(MustUnderstand = true)]
        public string fileHash;
        [MessageHeader(MustUnderstand = true)]
        public Int64 length;
        [MessageBodyMember(Order = 1)]
        public System.IO.Stream fileByteStream;
        public void Dispose() {
            if (fileByteStream != null) {
                fileByteStream.Close();
                fileByteStream = null;
            }
        }
    }
}
