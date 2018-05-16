using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace BepcoinTicker
{
    static class NbpApi
    {
        public static ObservableCollection<Currency> GetAllCurrencyExchangeRatesForDay(DateTime day)
        {
            var ser = new DataContractJsonSerializer(typeof(List<TableA>));
            var a = day.GetDateTimeFormats();
            string url = $@"http://api.nbp.pl/api/exchangerates/tables/A/{day.GetDateTimeFormats()[4]}/?format=json";
            var request = (HttpWebRequest) WebRequest.Create(url);
            try
            {
                var response = Task.Run(() => request.GetResponseAsync()).GetAwaiter().GetResult();
                return new ObservableCollection<Currency>(
                    ((List<TableA>) ser.ReadObject(response.GetResponseStream()))[0]
                    .rates);
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.ProtocolError || ex.Response == null) throw;
                var resp = (HttpWebResponse) ex.Response;
                if (resp.StatusCode == HttpStatusCode.NotFound)
                    return new ObservableCollection<Currency>
                    {
                        new Currency {code = "", currency = "The exchange is closed.", mid = 0}
                    };
                throw;
            }
        }

        public static List<Exchange> GetExchangesForSingleCurrencyInRange(DateTime from, DateTime to, string currency)
        {
            string url =
                $@"http://api.nbp.pl/api/exchangerates/rates/a/{currency}/{from.GetDateTimeFormats()[4]}/{
                        to.GetDateTimeFormats()[4]
                    }/?format=json";

            var ser = new DataContractJsonSerializer(typeof(RatesTable));
            var request = (HttpWebRequest) WebRequest.Create(url);
            try
            {
                var response = Task.Run(() => request.GetResponseAsync()).GetAwaiter().GetResult();

                return ((RatesTable) ser.ReadObject(response.GetResponseStream())).rates;
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.ProtocolError || ex.Response == null) throw;
                var resp = (HttpWebResponse) ex.Response;
                if (resp.StatusCode == HttpStatusCode.NotFound)
                    return new List<Exchange>
                    {
                        new Exchange {EffectiveDate = DateTime.Today, effectiveDate = DateTime.Today.ToString(), mid = 0, no = ""}
                    };
                throw;
            }
        }
    }
}