using System;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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

        public Currency Currency = new Currency();

        public DateTime? SelectedDate = DateTime.Today;

        public ObservableCollection<DateTime> SelectableDates;

        private readonly ApplicationDataContainer localSettings;

        public string DateDescription => $"Exchange for day {SelectedDate}";

        public MainPage()
        {
            InitializeComponent();

            DatesListView.DataContext = GetDaysBetweenTwoDates(DateTime.Today.AddDays(-90), DateTime.Today);

            CurrenciesGrid.DataContext = NbpApi.GetAllCurrencyExchangeRatesForDay(DateTime.Today);

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["page"] = "main";
        }

        private ObservableCollection<DateTime> GetDaysBetweenTwoDates(DateTime from, DateTime to)
        {
            var result = new ObservableCollection<DateTime>();
            var current = from;
            while (current < to)
            {
                result.Add(current);
                current = current.AddDays(1);
            }

            return result;
        }

        private void Resume()
        {
            if (SelectedDate == null) return;
            CurrenciesGrid.DataContext = NbpApi.GetAllCurrencyExchangeRatesForDay((DateTime) SelectedDate);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && (string) e.Parameter == "resuming")
                CurrenciesGrid.DataContext =
                    NbpApi.GetAllCurrencyExchangeRatesForDay(((DateTimeOffset) localSettings.Values["date"]).DateTime);
            localSettings.Values["page"] = "main";
        }

        private void DatesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedDate == null) return;
            CurrenciesGrid.DataContext = NbpApi.GetAllCurrencyExchangeRatesForDay((DateTime) SelectedDate);
            localSettings.Values["Date"] = (DateTimeOffset)SelectedDate;
        }

        private void CurrenciesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Frame.Navigate(typeof(CurrencyPage), Currency);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}