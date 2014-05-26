using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SimpleMvvmToolkit;
using TrackableEntities.Client;
using WebApiSample.Mvvm.Client.Entities.Models;

namespace WebApiSample.Mvvm.WpfClient
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class CustomerOrdersViewModel : ViewModelBase<CustomerOrdersViewModel>
    {
        private readonly ICustomerServiceAgent _customerServiceAgent;
        private readonly IOrderServiceAgent _orderServiceAgent;

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs<Order>> CreateOrderNotice;
        public event EventHandler<NotificationEventArgs<Order>> ModifyOrderNotice;
        public event EventHandler<NotificationEventArgs<Order, bool>> DeleteOrderNotice;
        public event EventHandler<NotificationEventArgs> DeleteVerifiedNotice;

        public CustomerOrdersViewModel(
            ICustomerServiceAgent customerServiceAgent,
            IOrderServiceAgent orderServiceAgent)
        {   
            SelectedCustomerIndex = -1;
            _customerServiceAgent = customerServiceAgent;
            _orderServiceAgent = orderServiceAgent;
        }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                NotifyPropertyChanged(m => m.SelectedCustomer);
            }
        }

        private Order _selectedOrder;
        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                _selectedOrder = value;
                NotifyPropertyChanged(m => m.SelectedOrder);
            }
        }

        private List<Customer> _customers;
        public List<Customer> Customers
        {
            get { return _customers; }
            set
            {
                _customers = value;
                NotifyPropertyChanged(m => m.Customers);
            }
        }

        private int _selectedCustomerIndex;
        public int SelectedCustomerIndex
        {
            get { return _selectedCustomerIndex; }
            set
            {
                _selectedCustomerIndex = value;
                NotifyPropertyChanged(m => m.SelectedCustomerIndex);
            }
        }

        private int _selectedOrderIndex;
        public int SelectedOrderIndex
        {
            get { return _selectedOrderIndex; }
            set
            {
                _selectedOrderIndex = value;
                NotifyPropertyChanged(m => m.SelectedOrderIndex);
            }
        }

        private ChangeTrackingCollection<Order> _customerOrders;
        public ChangeTrackingCollection<Order> CustomerOrders
        {
            get { return _customerOrders; }
            set
            {
                _customerOrders = value;
                NotifyPropertyChanged(m => m.CustomerOrders);
            }
        }

        public async void LoadCustomers()
        {
            try
            {
                var customers = await _customerServiceAgent.GetCustomers();
                Customers = new List<Customer>(customers);
                if (Customers.Count > 0)
                    SelectedCustomerIndex = 0;
            }
            catch (Exception ex)
            {
                NotifyError(null, ex);
            }
        }

        public async void LoadCustomerOrders()
        {
            if (SelectedCustomer == null) return;
            try
            {
                var orders = await _orderServiceAgent.GetCustomerOrders(SelectedCustomer.CustomerId);
                CustomerOrders = new ChangeTrackingCollection<Order>(orders);
                if (CustomerOrders.Count > 0)
                    SelectedOrderIndex = 0;
            }
            catch (Exception ex)
            {
                NotifyError(null, ex);
            }
        }

        public void CreateNewOrder()
        {
            if (SelectedCustomer == null) return;
            var order = new Order
            {
                Customer = SelectedCustomer,
                CustomerId = SelectedCustomer.CustomerId
            };
            Notify(CreateOrderNotice, new NotificationEventArgs<Order>(null, order));
        }

        public void ModifyOrder()
        {
            if (SelectedOrder == null) return;
            Notify(ModifyOrderNotice, new NotificationEventArgs<Order>(null, SelectedOrder));
        }

        public void DeleteOrder()
        {
            if (SelectedOrder == null) return;
            Notify(DeleteOrderNotice, new NotificationEventArgs<Order, bool>(null,
                SelectedOrder, OnDeleteNoticeCompleted));
        }

        private async void OnDeleteNoticeCompleted(bool confirmed)
        {
            if (confirmed)
            {
                try
                {
                    // Delete order
                    await _orderServiceAgent.DeleteOrder(SelectedOrder.OrderId);

                    // Verify order deletion
                    bool deleted = await _orderServiceAgent.VerifyOrderDeleted(SelectedOrder.OrderId);
                    if (deleted)
                    {
                        CustomerOrders.Remove(SelectedOrder);
                        Notify(DeleteVerifiedNotice, new NotificationEventArgs());
                    }
                }
                catch (Exception ex)
                {
                    NotifyError(null, ex);
                }
            }
        }

        public void AddNewOrder(Order order)
        {
            if (order == null) return;
            CustomerOrders.Tracking = false;
            CustomerOrders.Add(order);
            CustomerOrders.Tracking = true;
            SelectedOrderIndex = CustomerOrders.Count - 1;
        }

        private void NotifyError(string message, Exception error)
        {
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }
    }
}