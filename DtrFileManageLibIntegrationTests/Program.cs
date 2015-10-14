using System;
using System.Linq;
using System.IO;
using System.ServiceModel;

namespace DtrMonitorCoreIntegrationTests {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("****** Service started.********");
            using (ServiceHost serviceHost = new ServiceHost(typeof(DtrMonitorCore.FileTransferService))) {
                serviceHost.Open();
                Console.ReadKey();
            }
        }
    }
}
