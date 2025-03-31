using System;
using System.Xml.Serialization;

namespace CrackHash.Models
{
    [XmlRoot("CrackHashManagerRequest")]
    public class CrackHashManagerRequest
    {
        [XmlElement("RequestId")]
        public required string RequestId { get; set; }

        [XmlElement("Hash")]
        public required string Hash { get; set; }

        [XmlElement("MaxLength")]
        public required int MaxLength { get; set; }

        [XmlElement("PartNumber")]
        public required int PartNumber { get; set; }

        [XmlElement("PartCount")]
        public required int PartCount { get; set; }

        [XmlElement("Alphabet")]
        public Alphabet Alphabet { get; set; } = new Alphabet();
    }

    public class Alphabet
    {
        [XmlElement("symbols")]
        public string Symbols { get; set; } = "abcdefghijklmnopqrstuvwxyz0123456789";
    }
}
