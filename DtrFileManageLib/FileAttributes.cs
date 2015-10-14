using System;
using System.Security.Cryptography;
using System.IO;

namespace DtrFileManageLib {
    public static class FileAttributes {
        public static string ComputeHashSum(this FileInfo fInfo) {
            //if (!File.Exists(fileName))
            //    throw new FileNotFoundException("File not found.", fileName);
            using (var md5 = MD5.Create()) {
                using (var stream=fInfo.OpenRead()){
                    return BitConverter.ToString(md5.ComputeHash(stream));
                }
            }
        }

        //public static long getFileSize(string fileName) {
        //    FileInfo fileInfo = new FileInfo(fileName);
        //    if (!fileInfo.Exists) throw new FileNotFoundException("File not found.", fileName);
        //    return fileInfo.Length;
        //} 
    }
}
