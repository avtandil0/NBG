using System;
using System.Collections.Generic;

#nullable disable

namespace NBG.Models
{
    public partial class Rate
    {
        public int Id { get; set; }
        public string SourceCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public DateTime? DateL { get; set; }
        public double RateExchange { get; set; }
        public double RateBuy { get; set; }
        public double RateSell { get; set; }
        public double RateOfficial { get; set; }
        public short? Division { get; set; }
        public DateTime Syscreated { get; set; }
        public int Syscreator { get; set; }
        public DateTime Sysmodified { get; set; }
        public int Sysmodifier { get; set; }
        public Guid Sysguid { get; set; }
        public byte[] Timestamp { get; set; }
    }
}
