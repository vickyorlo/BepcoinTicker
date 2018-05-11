using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
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
        public ObservableCollection<TableA> CurrentExchanges;
        public MainPage()
        {
            InitializeComponent();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ObservableCollection<TableA>));

            string url = @"http://api.nbp.pl/api/exchangerates/tables/A/?format=json";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = Task.Run( ()=>request.GetResponseAsync() ).GetAwaiter().GetResult();

            //using (var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII))
            //{
            //    string responseText = reader.ReadToEnd();
            //}


            CurrentExchanges = (ObservableCollection<TableA>) ser.ReadObject(response.GetResponseStream());

            (LineChart.Series[0] as LineSeries).ItemsSource = CurrentExchanges;

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
        public List<Exchange> rates;
    }

    [DataContract]
    public class Exchange
    {
        [DataMember]
        public string currency;
        [DataMember]
        public string code;
        [DataMember]
        public decimal mid;
    }
}