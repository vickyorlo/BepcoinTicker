using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace BepcoinTicker
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class CurrencyPage : Page
    {
        public Currency Currency;

        public string ChartTitle => $"Price history for {Currency.currency}";

        private readonly Windows.Storage.ApplicationDataContainer localSettings;

        public CurrencyPage()
        {
            this.InitializeComponent();
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is null)
            {
                //we resuming boys
                var composite = (Windows.Storage.ApplicationDataCompositeValue) localSettings.Values["currencyObj"];
                Currency = new Currency
                {
                    code = (string) composite["code"],
                    currency = (string) composite["currency"]
                };
                DateFrom.Date = (DateTimeOffset) composite["from"];
                DateTo.Date = (DateTimeOffset)composite["to"];

                UpdateGraph();
            }
            else
            {
                Currency = (Currency)e.Parameter;
                UpdateGraph();
                localSettings.Values["page"] = "curr";
            }
        }

        private async void UpdateGraph()
        {
            if (Currency == null || Currency.code == "") return;
            if (DateFrom.Date.Date < DateTime.Parse("23-09-02"))
            {
                errorTextBox.Text = "Error: invalid date!";
                return;
            }

            Windows.Storage.ApplicationDataCompositeValue composite =
                new Windows.Storage.ApplicationDataCompositeValue
                {
                    ["currency"] = Currency.currency,
                    ["code"] = Currency.code,
                    ["from"] = DateFrom.Date,
                    ["to"] = DateTo.Date
                };
            localSettings.Values["currencyObj"] = composite;


            errorTextBox.Text = "";
            var from = DateFrom.Date.Date;
            var to = DateTo.Date.Date;
            var tempPairs = new List<KeyValuePair<DateTime, double>>();
            var ab = new List<Task<List<Exchange>>>();
            while (to - from > TimeSpan.FromDays(356))
            {
                var mid = to.AddDays(-356);
                ab.Add(NbpApi.GetExchangesForSingleCurrencyInRange(mid, to, Currency.code));
                to = mid;
            }
            ab.Add(NbpApi.GetExchangesForSingleCurrencyInRange(from, to, Currency.code));

            await Task.Run(async () =>
            {
                tempPairs.AddRange(from l in await Task.WhenAll(ab) from ex in l select new KeyValuePair<DateTime, double>(ex.EffectiveDate, ex.mid));
            });
            LineChart.DataContext = tempPairs;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage),"back");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void DateFrom_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            UpdateGraph();
        }

        private void DateTo_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            UpdateGraph();
        }
    }
}
