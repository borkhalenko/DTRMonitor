using System;
using System.Security.Cryptography;
using System.IO;

namespace DtrFileManageLib {
    public static class FileAttributes {
        public static string ComputeHash(string fileName) {
            using (var md5 = MD5.Create()) {
                using (var stream=File.OpenRead(fileName)){
                    return BitConverter.ToString(md5.ComputeHash(stream));
                }
            }
        }
    }
}
