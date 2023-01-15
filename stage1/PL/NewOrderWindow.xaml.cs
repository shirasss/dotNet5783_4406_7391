﻿using BlApi;
using BO;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for NewOrderWindow.xaml
    /// </summary>

    public partial class NewOrderWindow : Window
    {
        public IBl bl;
        public BO.Cart cart { get; set; }
        public NewOrderWindow(IBl BL,Cart cart_)
        {
            InitializeComponent();
            bl = BL;
            cart = cart_;
            CatalogView.ItemsSource = bl.iProduct.ReadCatalog();
        }

        

        private void CatalogView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CatalogView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           new ProductWindow(bl, ((BO.ProductItem)CatalogView.SelectedItem).ID, cart).Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new CartWindow(bl, cart).Show();
            this.Close();
        }
    }
}