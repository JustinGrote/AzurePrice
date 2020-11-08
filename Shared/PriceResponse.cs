using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
// Generated from https://app.quicktype.io?share=BIWN5YNMt1MFeiqHapU1 and modified for System.Text.Json
namespace Client.Shared
{
    public class PriceResponse
    {
        public string BillingCurrency { get; set; }
        public string CustomerEntityId { get; set; }
        public string CustomerEntityType { get; set; }
        public Uri NextPageLink { get; set; }
        public int Count { get; set; }
        
        [JsonPropertyName("Items")]
        public List<Price> Prices { get; set; }
    }
}
