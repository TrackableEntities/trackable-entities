using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SimpleMvvmToolkit;
using WebApiSample.Mvvm.Client.Entities.Models;

namespace WebApiSample.Mvvm.WpfClient
{
    /// <summary>
    /// Interaction logic for OrderDetailView.xaml
    /// </summary>
    public partial class OrderDetailView : Window
    {
        public OrderDetailView(Order order)
        {
            InitializeComponent();

            // Initialize view model
            ViewModel = (OrderViewModelDetail)DataContext;
            ViewModel.Initialize(order);
            ViewModel.ErrorNotice += OnErrorNotice;
            ViewModel.ResultNotice += OnResultNotice;
        }

        public OrderViewModelDetail ViewModel { get; private set; }

        private void OnResultNotice(object sender, NotificationEventArgs<bool> eventArgs)
        {
            DialogResult = eventArgs.Data;
        }

        private void OnErrorNotice(object sender, NotificationEventArgs<Exception> eventArgs)
        {
            MessageBox.Show(eventArgs.Data.Message, "Error");
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.ErrorNotice -= OnErrorNotice;
            ViewModel.ResultNotice -= OnResultNotice;
        }
    }
}
