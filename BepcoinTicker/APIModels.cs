using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BepcoinTicker
{
        [DataContract]
        public class TableA
        {
            [DataMember]
            public string table;
            [DataMember]
            public string no;
            [DataMember]
            public string effectiveDate;
            [DataMember]
            public List<Currency> rates;
        }

        [DataContract]
        public class Currency
        {
            [DataMember]
            public string currency;
            [DataMember]
            public string code;
            [DataMember]
            public double mid;
        }

        [DataContract]
        public class RatesTable
        {
            [DataMember]
            public string table;
            [DataMember]
            public string currency;
            [DataMember]
            public string code;
            [DataMember]
            public List<Exchange> rates;
        }

        [DataContract]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class Exchange
        {
            [DataMember]
            public string no;

            public DateTime EffectiveDate;

            [DataMember]
#pragma warning disable IDE1006 // Naming Styles
            public string effectiveDate
#pragma warning restore IDE1006 // Naming Styles
            {
                get => EffectiveDate.Date.ToString(CultureInfo.InvariantCulture);
                set => EffectiveDate = DateTime.Parse(value);
            }

            [DataMember]
            public double mid; //this doeesn't work

            public double Mid => mid; //but this does
        }
}
