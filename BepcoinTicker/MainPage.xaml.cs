using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public DateTime? SelectedDate;

        public ObservableCollection<DateTime> SelectableDates;

        public MainPage()
        {
            InitializeComponent();

            DatesListView.DataContext = GetDaysBetweenTwoDates(DateTime.Today.AddDays(-90),DateTime.Today);

            CurrenciesGrid.DataContext = NbpApi.GetAllCurrencyExchangeRatesForDay(DateTime.Today);
        }

        private ObservableCollection<DateTime> GetDaysBetweenTwoDates(DateTime from, DateTime to)
        {
            ObservableCollection<DateTime> result = new ObservableCollection<DateTime>();
            DateTime current = from;
            while (current < to)
            {
                result.Add(current);
                current = current.AddDays(1);
            }

            return result;
        }

        private void Resume()
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private void DatesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedDate == null) return;
            CurrenciesGrid.DataContext = NbpApi.GetAllCurrencyExchangeRatesForDay((DateTime) SelectedDate);
            //CurrenciesGrid.SelectedItem = null;
        }

        private void CurrenciesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Frame.Navigate(typeof(CurrencyPage), Currency );
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}