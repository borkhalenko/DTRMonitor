using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using DtrClientCore.DtrServiceRef;
using DtrFileManageLib;

namespace DtrClientCore {
    public class ClientMonitor {
        private string downloadPath;
        private string uploadPath;
        private const int chunkSize = 2048;
        private const int waitSec = 5;
        private Queue<string> fileQueue = new Queue<string>();
        private FileSystemWatcher fsWatcher = new FileSystemWatcher();
        private string lastRemovedFile = String.Empty;

        public ClientMonitor() : this("C:\\test\\Client\\Download\\", "C:\\test\\Client\\Upload\\") {
        }
        public ClientMonitor(string downloadPath, string uploadPath) {
            this.downloadPath = downloadPath;
            this.uploadPath = uploadPath;
        }

        public void RemoveFile(string fileName) {
            string fileToRemove = Path.Combine(uploadPath, fileName);
            if (File.Exists(fileToRemove)) File.Delete(Path.Combine(uploadPath, fileToRemove));
            if (fileQueue.Peek() == fileToRemove) fileQueue.Dequeue();
            ScanUploadPath();
            fsWatcher.Path = uploadPath;
            fsWatcher.Created += new FileSystemEventHandler(AddedNewFile);
            fsWatcher.Deleted += new FileSystemEventHandler(DeleteFile);
            fsWatcher.EnableRaisingEvents = true;
        }

        public void StartSync() {
            using (FileTransferServiceClient server = new FileTransferServiceClient()) {
                RemoteFileData receivedData = new RemoteFileData();
                while (true) {
                    if (server.hasNext()) {
                        Console.WriteLine("Yes");
                        receivedData.fileHash = server.ReceiveNextFromServer(out receivedData.fileName, out receivedData.length, out receivedData.fileByteStream);
                        string newFileName = Path.Combine(downloadPath, receivedData.fileName);
                        SaveFile(receivedData.fileByteStream, newFileName, receivedData.length);
                        receivedData.fileByteStream.Dispose();
                        server.RemoveFileFromServer(receivedData.fileName);
                        if (receivedData.fileHash==new FileInfo(newFileName).ComputeHashSum())
                            Console.WriteLine("File received");
                        else
                            Console.WriteLine("Receive error! Bad hash sum");
                    }
                    else
                        Console.WriteLine("No");
                    System.Threading.Thread.Sleep(waitSec * 1000);
                }
            }

        }

        private void SaveFile(Stream fstream, string fullName, Int64 length) {
            using (FileStream writeStream = new FileStream(fullName, FileMode.CreateNew, FileAccess.Write)) {
                byte[] buffer = new byte[chunkSize];
                do {
                    int bytesRead = fstream.Read(buffer, 0, chunkSize);
                    if (bytesRead == 0) break;
                    writeStream.Write(buffer, 0, bytesRead);
                } while (true);
                writeStream.Close();
            }
        }

        private void ScanUploadPath() {
            DirectoryInfo dirInfo = new DirectoryInfo(downloadPath);
            try {
                foreach (var file in dirInfo.GetFiles().OrderBy(f => f.CreationTime)) {
                    fileQueue.Enqueue(file.FullName);
                }
            }
            catch (DirectoryNotFoundException e) {
                Console.WriteLine("Error: Directory \"" + downloadPath + "\" not found. Exception message: " + e.Message);
            }
        }

        private void AddedNewFile(object source, FileSystemEventArgs e) {
            fileQueue.Enqueue(e.FullPath);
        }

        private void DeleteFile(object source, FileSystemEventArgs e) {
            if (e.FullPath != lastRemovedFile) {
                fileQueue.Clear();
                ScanUploadPath();
            }
        }
    }
}
