using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DtrFileManageLib {
    public delegate void FileChanged(string fileName, DateTime dateTime);
    public delegate void ErrorOccured(string errorMessage, DateTime dateTime);
    public sealed class FSMonitor {
        //private static Dictionary<string, FSMonitor> monitors=new Dictionary<string, FSMonitor>();
        private FileSystemWatcher fsWatcher;
        public event FileChanged FileCreated;
        public event FileChanged FileModified;
        public event FileChanged FileDeleted;
        public event ErrorOccured ErrorDetected;

        public FSMonitor(string newPath) {
            fsWatcher = new FileSystemWatcher();
            fsWatcher.Path=newPath;
            fsWatcher.Created += FsWatcher_Created;
            fsWatcher.Deleted += FsWatcher_Deleted;
            fsWatcher.Changed += FsWatcher_Changed;
            fsWatcher.Error += FsWatcher_Error;
        }

        private void FsWatcher_Error(object sender, ErrorEventArgs e) {
            ErrorDetected("The FSMonitor has detected an error. Message: "+e.GetException().Message, DateTime.Now);
        }

        private void FsWatcher_Changed(object sender, FileSystemEventArgs e) {
            //Console.WriteLine("File {0} was changed.", e.Name);
            //Console.WriteLine("Hashsum={0}", FileAttributes.ComputeHash(e.FullPath));
            FileModified(e.FullPath, DateTime.Now);
        }

        private void FsWatcher_Deleted(object sender, FileSystemEventArgs e) {
            //Console.WriteLine("File {0} was deleted.", e.Name);
            FileDeleted(e.FullPath, DateTime.Now);
        }

        private void FsWatcher_Created(object sender, FileSystemEventArgs e) {
            //Console.WriteLine("File {0} was created.", e.Name);
            //Console.WriteLine("Hashsum={0}", FileAttributes.ComputeHash(e.FullPath));
            FileCreated(e.FullPath, DateTime.Now);
        }

        public void StartWatch() {
            fsWatcher.EnableRaisingEvents = true;
        }

        public void StopWatch() {
            fsWatcher.EnableRaisingEvents = false;
        }

        //public static FSMonitor GetInstance(string path) {
        //    if (monitors.ContainsKey(path)) {
        //        return monitors[path];
        //    }
        //    else {
        //        FSMonitor newItem = new FSMonitor(path);
        //        monitors.Add(path, newItem);
        //        return newItem;
        //    } 
        //}
    }
}
