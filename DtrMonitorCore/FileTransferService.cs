using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DtrMonitorCore {
    class FileTransferService : IDtrMonitorService {
        public RemoteFileStream DownloadFile(RemoteFileInfo fi) {
            throw new NotImplementedException();
        }

        public RemoteFileInfo isNext() {
            throw new NotImplementedException();
        }

        public RemoteFileInfo UploadFile(RemoteFileStream fs) {
            throw new NotImplementedException();
        }
    }
}
