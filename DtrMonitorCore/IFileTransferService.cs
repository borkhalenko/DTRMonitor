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
        void RemoveFileFromServer(RemoteFileInfo fi);
        [OperationContract]
        RemoteFileStream ReceiveNextFromServer();
        [OperationContract]
        RemoteFileInfo SendNextToServer(RemoteFileStream fs);
    }

    [MessageContract]
    public class RemoteFileInfo {
        [MessageBodyMember]
        public bool fileExists;
        [MessageBodyMember]
        public string fileName;
        [MessageBodyMember]
        public string fileHash;
        [MessageBodyMember]
        public Int64 length;
    }

    [MessageContract]
    public class RemoteFileStream {
        [MessageHeader(MustUnderstand = true)]
        public RemoteFileInfo fileInfo;
        [MessageBodyMember]
        public System.IO.Stream FileByteStream;
        public void Dispose() {
            if (FileByteStream != null) {
                FileByteStream.Close();
                FileByteStream = null;
            }
        }
    }
}
