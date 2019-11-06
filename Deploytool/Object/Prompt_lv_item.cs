using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deploytool.Object
{
    [Serializable]
    class Prompt_lv_item
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool Checked { get; set; }
    }
}
