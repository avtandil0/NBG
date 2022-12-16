using System;
using System.Collections.Generic;

#nullable disable

namespace NBG.Models
{
    public partial class NlCeOct19CurrencyExchangeRate437dbf0e84ff417a965dEd2bb9650972
    {
        public byte[] Timestamp { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime StartingDate { get; set; }
        public decimal ExchangeRateAmount { get; set; }
        public decimal AdjustmentExchRateAmount { get; set; }
        public string RelationalCurrencyCode { get; set; }
        public decimal RelationalExchRateAmount { get; set; }
        public int FixExchangeRateAmount { get; set; }
        public decimal RelationalAdjmtExchRateAmt { get; set; }
        public Guid SystemId { get; set; }
        public DateTime SystemCreatedAt { get; set; }
        public Guid SystemCreatedBy { get; set; }
        public DateTime SystemModifiedAt { get; set; }
        public Guid SystemModifiedBy { get; set; }
    }
}
