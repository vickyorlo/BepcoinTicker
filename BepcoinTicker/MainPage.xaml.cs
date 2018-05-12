using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http.Headers;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BepcoinTicker
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Currency> CurrencyCollection;

        public Collection<Exchange> ExchangeCollection;

        public Currency currency;

        public MainPage()
        {
            InitializeComponent();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<TableA>));

            string url = @"http://api.nbp.pl/api/exchangerates/tables/A/?format=json";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = Task.Run( ()=>request.GetResponseAsync() ).GetAwaiter().GetResult();

            //using (var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII))
            //{
            //    string responseText = reader.ReadToEnd();
            //}


            CurrencyCollection = new ObservableCollection<Currency>(((List<TableA>)ser.ReadObject(response.GetResponseStream()))[0].rates);


            string exchangeHistoryURL = @"http://api.nbp.pl/api/exchangerates/rates/a/gbp/2012-01-01/2012-01-10/?format=json";

            ser = new DataContractJsonSerializer(typeof(RatesTable));
            request = (HttpWebRequest)WebRequest.Create(exchangeHistoryURL);
            response = Task.Run(() => request.GetResponseAsync()).GetAwaiter().GetResult();

            List<Exchange> exchanges = ((RatesTable)ser.ReadObject(response.GetResponseStream())).rates;

            (LineChart.Series[0] as LineSeries).ItemsSource = exchanges;


        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(CurrencyPage), e.ClickedItem);
        }


        private void Resume()
        {

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.Equals(""))
            {
                return;
            }
            if ((bool)e.Parameter == true)
            {
                Resume();
            }

        }
    }

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
    public class Exchange
    {
        [DataMember]
        public string no;

        public DateTime _effectiveDate;

        [DataMember]
        public string effectiveDate
        {
            get { return _effectiveDate.Date.ToString(); }
            set { _effectiveDate = DateTime.Parse(value); }
        }

        [DataMember]
        public double mid;
    }

}