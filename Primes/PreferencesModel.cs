using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Primes
{
    public class PreferencesModel
    {
        public int WindowTop { get; set; } = 100;
        public int WindowLeft { get; set; } = 100;
        public int  WindowHeight { get; set; } = 800;
        public int WindowWidth { get; set; } = 1200;
        public FormWindowState WindowState { get; set; }
    }
}
