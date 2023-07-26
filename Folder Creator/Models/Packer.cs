using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Folder_Creator.Models
{
    public class Packer
    {
        [Name("Base Document Reference")]
        public string PackerNumber { get; set; } = string.Empty;
    }
}
