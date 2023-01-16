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
using System.Xml.Linq;

namespace PL;

/// <summary>
/// Interaction logic for CartWindow.xaml
/// </summary>
public partial class CartWindow : Window
{
    public IBl bl { get; set; }
    public Cart cart { get; set; }
    public CartWindow(IBl BL,Cart cart_)
    {
        InitializeComponent();
        bl= BL; 
        cart= cart_;
        ProductsView.ItemsSource = cart_.Items;
    }

    private void SubmitOrderBTN_Click(object sender, RoutedEventArgs e)
    {
        bl.iCart.SubmitOrder(cart, CustomerNameTXT.Text, CustomerEmailTXT.Text, CustomerAddressTXT.Text);
        new MainWindow().Show();
        this.Close();
    }
    
    private void ProductsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void DeleteBTN_Click(object sender, RoutedEventArgs e)
    {
        ///
        try
        {
            BO.OrderItem obj = ((FrameworkElement)sender).DataContext as BO.OrderItem;
            int id = obj.ProductID;
            cart = bl.iCart.UpdateOrderItem(cart, id, 0);
            MessageBox.Show("The product has been successfully removed from the cart");

        }
        catch  (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

    }

    private void back_Click(object sender, RoutedEventArgs e)
    {
        new NewOrderWindow(bl, cart).Show();
        this.Close();
    }

    //  <GridViewColumn Header = "Amount" Width="80" x:Name="newAmountColounm">
    //<GridViewColumn.CellTemplate >
    //    <DataTemplate>
    //        <TextBox Text = "{Binding Path=Amount, Mode=TwoWay}" x:Name="newAmountTxt" Width="80"/>
    //    </DataTemplate>
    //</GridViewColumn.CellTemplate>
    //</GridViewColumn>


    //<GridViewColumn Header = "update new amount" Width="150">
    //    <GridViewColumn.CellTemplate >
    //        <DataTemplate>
    //            <Button x:Name="updateNewAmount" Click="updateAmount_Button_Click" Width="150" ></Button>
    //        </DataTemplate>
    //    </GridViewColumn.CellTemplate>
    //</GridViewColumn>
}
