using System;
using System.Windows;
using System.Windows.Controls;
using SimpleMvvmToolkit;
using WebApiSample.Mvvm.Client.Entities.Models;

namespace WebApiSample.Mvvm.WpfClient
{
    /// <summary>
    /// Interaction logic for CustomerView.xaml
    /// </summary>
    public partial class CustomerView : UserControl
    {
        private readonly CustomerOrdersViewModel _viewModel;

        public CustomerView()
        {
            InitializeComponent();
            _viewModel = (CustomerOrdersViewModel)LayoutRoot.DataContext;
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            _viewModel.ErrorNotice += OnErrorNotice;
            _viewModel.CreateOrderNotice += OnCreateOrderNotice;
            _viewModel.ModifyOrderNotice += OnModifyOrderNotice;
            _viewModel.DeleteOrderNotice += OnDeleteOrderNotice;
            _viewModel.DeleteVerifiedNotice += OnDeleteVerifiedNotice;
        }

        private void OnDeleteVerifiedNotice(object sender, NotificationEventArgs eventArgs)
        {
            MessageBox.Show("Order has been deleted", "Order Deleted",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnCreateOrderNotice(object sender, NotificationEventArgs<Order> eventArgs)
        {
            // Create order view model and view
            var orderDetail = new OrderDetailView(eventArgs.Data);
            orderDetail.Owner = Window.GetWindow(this);

            // Show order detail dialog
            if (orderDetail.ShowDialog() == false) return;

            // Add new order
            _viewModel.AddNewOrder(orderDetail.ViewModel.Model);
        }

        private void OnModifyOrderNotice(object sender, NotificationEventArgs<Order> eventArgs)
        {
            // Create order view model and view
            var orderDetail = new OrderDetailView(eventArgs.Data);
            orderDetail.Owner = Window.GetWindow(this);

            // Show order detail dialog
            orderDetail.ShowDialog();
        }

        private void OnDeleteOrderNotice(object sender, NotificationEventArgs<Order, bool> eventArgs)
        {
            // Prompt user to delete order
            if (MessageBox.Show("Do you wish to delete this order?", "Delete Order",
                MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                eventArgs.Completed(true);
            }
        }

        private void OnErrorNotice(object sender, NotificationEventArgs<Exception> eventArgs)
        {
            MessageBox.Show(eventArgs.Data.Message, "Error");
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.ErrorNotice -= OnErrorNotice;
            _viewModel.CreateOrderNotice -= OnCreateOrderNotice;
            _viewModel.ModifyOrderNotice -= OnModifyOrderNotice;
            _viewModel.DeleteOrderNotice -= OnDeleteOrderNotice;
        }
    }
}
