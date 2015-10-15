using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DtrClientCore;

namespace DtrClientCoreIntegrationTests {
    class Program {
        static void Main(string[] args) {
            ClientMonitor monitor = new ClientMonitor();
            monitor.StartSync();
        }
    }
}
