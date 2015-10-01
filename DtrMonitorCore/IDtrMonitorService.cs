using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace DtrMonitorCore {
    [ServiceContract]
    public interface IDtrMonitorService {
        [OperationContract]
        RemoteFileInfo isNext();
        [OperationContract]
        RemoteFileStream DownloadFile(RemoteFileInfo fi);
        [OperationContract]
        RemoteFileInfo UploadFile(RemoteFileStream fs);
    }

    [MessageContract]
    public class RemoteFileInfo {
        [MessageBodyMember]
        public bool fileExists;
        [MessageBodyMember]
        public string fileName;
        [MessageBodyMember]
        public string fileHash;
    }

    [MessageContract]
    public class RemoteFileStream {
        [MessageHeader(MustUnderstand = true)]
        public RemoteFileInfo fileInfo;
        [MessageHeader(MustUnderstand = true)]
        public UInt64 length;
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
