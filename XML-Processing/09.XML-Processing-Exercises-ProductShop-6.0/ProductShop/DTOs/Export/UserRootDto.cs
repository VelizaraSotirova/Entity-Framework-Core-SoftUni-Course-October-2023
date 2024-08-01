using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductShop.DTOs.Export
{
    using System.Xml.Serialization;

    [XmlRoot("users")]
    public class UserRootDto
    {
        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlElement("user")]
        public UserExportDto[] Users { get; set; }
    }
}
