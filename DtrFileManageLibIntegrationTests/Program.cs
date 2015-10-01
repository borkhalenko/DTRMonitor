using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DtrFileManageLib;

namespace DtrFileManageLibIntegrationTests {
    class Program {
        static void Main(string[] args) {
            FSMonitor monitor = FSMonitor.GetInstance("C:/test");
            monitor.StartWatch();
            System.Threading.Thread.Sleep(10000000);
        }
    }
}
