using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgDiffTool.Models
{
    class ImageDiffContext : DbContext
    {
        public DbSet<MyImage> MyImages { get; set; }
    }
}
