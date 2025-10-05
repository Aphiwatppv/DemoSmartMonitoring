using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LogSystem
{
    [XmlRoot("LogSettings")]
    public class LogConfig
    {
        [XmlElement] public int RetentionDays { get; set; } = 7;
        [XmlElement] public int MaxFileSizeMB { get; set; } = 10;
    }
}
