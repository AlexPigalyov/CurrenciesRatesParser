//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CurrenciesRatesParser.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class DpdCities
    {
        public long cityId { get; set; }
        public bool cityIdSpecified { get; set; }
        public string countryCode { get; set; }
        public string countryName { get; set; }
        public int regionCode { get; set; }
        public bool regionCodeSpecified { get; set; }
        public string regionName { get; set; }
        public string cityCode { get; set; }
        public string cityName { get; set; }
        public string abbreviation { get; set; }
        public string indexMin { get; set; }
        public string indexMax { get; set; }
        public Nullable<long> Population { get; set; }
        public string Settled { get; set; }
        public Nullable<double> lat { get; set; }
        public Nullable<double> lng { get; set; }
    }
}
