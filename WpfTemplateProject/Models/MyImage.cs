using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgDiffTool.Models
{
    class MyImage
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Filename { get; set; }
        public bool Untracked { get; set; }
    }
}
