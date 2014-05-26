/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:WebApiSample.Mvvm.WpfClient"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// Toolkit namespace
using SimpleMvvmToolkit;

namespace WebApiSample.Mvvm.WpfClient
{
    /// <summary>
    /// This class creates ViewModels on demand for Views, supplying a
    /// ServiceAgent to the ViewModel if required.
    /// <para>
    /// Place the ViewModelLocator in the App.xaml resources:
    /// </para>
    /// <code>
    /// &lt;Application.Resources&gt;
    ///     &lt;vm:ViewModelLocator xmlns:vm="clr-namespace:WebApiSample.Mvvm.WpfClient"
    ///                                  x:Key="Locator" /&gt;
    /// &lt;/Application.Resources&gt;
    /// </code>
    /// <para>
    /// Then use:
    /// </para>
    /// <code>
    /// DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
    /// </code>
    /// <para>
    /// Use the <strong>mvvmlocator</strong> or <strong>mvvmlocatornosa</strong>
    /// code snippets to add ViewModels to this locator.
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        // Create MainPageViewModel on demand
        public MainPageViewModel MainPageViewModel
        {
            get { return new MainPageViewModel(); }
        }

        // Create CustomerViewModel on demand
        public CustomerOrdersViewModel CustomerOrdersViewModel
        {
            get
            {
                ICustomerServiceAgent customerServiceAgent = new CustomerServiceAgent();
                IOrderServiceAgent orderServiceAgent = new OrderServiceAgent();
                return new CustomerOrdersViewModel(customerServiceAgent, orderServiceAgent);
            }
        }

        // Create OrderViewModelDetail on demand
        public OrderViewModelDetail OrderViewModelDetail
        {
            get
            {
                IProductServiceAgent productServiceAgent = new ProductServiceAgent();
                IOrderServiceAgent orderServiceAgent = new OrderServiceAgent();
                return new OrderViewModelDetail(productServiceAgent, orderServiceAgent);
            }
        }
    }
}