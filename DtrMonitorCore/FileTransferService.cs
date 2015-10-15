using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DtrFileManageLib;

namespace DtrMonitorCore {
    public class FileTransferService : IFileTransferService {
        private readonly string downloadPath;
        private readonly string uploadPath;
        private const int chunkSize = 2048;
        private Queue<string> fileQueue = new Queue<string>();
        private FileSystemWatcher fsWatcher = new FileSystemWatcher();

        public FileTransferService(): this("C:\\test\\Server\\Download\\", "C:\\test\\Server\\Upload\\") { }

        public FileTransferService(string downloadPath, string uploadPath) {
            this.downloadPath = downloadPath;
            this.uploadPath = uploadPath;
            ScanUploadPath();
            fsWatcher.Path = uploadPath;
            fsWatcher.Created += new FileSystemEventHandler(AddedNewFile);
            fsWatcher.EnableRaisingEvents = true;
        }

        public void RemoveFileFromServer(string fileName) {
            string fileToRemove = Path.Combine(uploadPath, fileName);
            if (File.Exists(fileToRemove)) File.Delete(Path.Combine(uploadPath, fileToRemove));
            if (fileQueue.Peek() == fileToRemove) fileQueue.Dequeue();
        }

        public RemoteFileData ReceiveNextFromServer() {
            RemoteFileData rfData = new RemoteFileData();
            if (fileQueue.Count == 0) {
                rfData.fileExists = false;
                return rfData;
            }
            string nextFile = fileQueue.Peek();
            if (File.Exists(nextFile) == false) {
                Console.WriteLine("Error: File \"" + nextFile + "\" doesn't exist.");
                Console.WriteLine("Removing file "+nextFile+" from queue.");
                fileQueue.Dequeue();
            }
            FileInfo fInfo = new FileInfo(nextFile);
            rfData.fileExists = true;
            rfData.fileName = fInfo.Name;
            rfData.fileHash = fInfo.ComputeHashSum();
            rfData.length = fInfo.Length;
            rfData.FileByteStream = new FileStream(nextFile, FileMode.Open, FileAccess.Read);
            return rfData;
        }

        public RemoteFileHash SendNextToServer(RemoteFileData fd) {
            try {
                if (!Directory.Exists(downloadPath)) Directory.CreateDirectory(downloadPath);
                string newFile = Path.Combine(downloadPath, fd.fileName);
                if (File.Exists(newFile)) File.Delete(newFile);
                byte[] buffer = new byte[chunkSize];
                using (FileStream writeStream = new FileStream(newFile, FileMode.CreateNew, FileAccess.Write)) {
                    do {
                        int bytesRead = fd.FileByteStream.Read(buffer, 0, chunkSize);
                        if (bytesRead == 0) break;
                        writeStream.Write(buffer, 0, bytesRead);
                    } while (true);
                    writeStream.Close();
                }
                RemoteFileHash hash = new RemoteFileHash();
                hash.fileHash = new FileInfo(newFile).ComputeHashSum();
                return hash;
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine("Error: Application doesn't have the required perrmissions to the folder \"" + downloadPath + "\". Exception message: " + e.Message);
                RemoteFileHash hash = new RemoteFileHash();
                hash.fileHash = String.Empty;
                return hash;
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

    }
}
