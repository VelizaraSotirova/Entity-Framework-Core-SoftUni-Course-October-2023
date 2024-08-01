using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlRoot("cars")]
    public class CarCollection
    {
        [XmlElement("car")]
        public ExportCarsWithDistance[] Cars { get; set; }
    }
}
