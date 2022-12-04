﻿
using BlApi;
using Dal;
using DalApi;
namespace BlImplementation;
internal class BlCart:ICart
{
    IDal dal = new DalList();
    /// <summary>
    /// add order item to the cart
    /// </summary>
    /// <param name="cart">the customer cart</param>
    /// <param name="id">the product id</param>
    /// <returns>returns the cart whith to new order item</returns>
    /// <exception cref="BO.NotInStockException"></exception>
    /// <exception cref="BO.DataError"></exception>
    public BO.Cart addOrderItem(BO.Cart cart, int id)
    {
        try
        {
            Dal.DO.Product p = dal.iproduct.Read(id);
            if (p.InStock <= 0)
                throw new BO.NotInStockException(p.Name);
            foreach (BO.OrderItem item in cart.Items)
            {
                if (item.ProductID == id)
                {
                    //Updating the existing OrderItem
                    item.Amount++;
                    item.TotalPrice += p.Price;
                    cart.TotalPrice += p.Price;
                    return cart;
                }
            };
            //creating a new OrderItem:
            BO.OrderItem OItem = new BO.OrderItem();
            OItem.ID = DataSource.Config.OrderItem_ID;
            OItem.ProductID = p.ID;
            OItem.Name = p.Name;
            OItem.Price = p.Price;
            OItem.Amount = 1;
            OItem.TotalPrice = p.Price;
            cart.TotalPrice += p.Price;
            cart.Items.Add(OItem);
            return cart;
        }
        catch (Dal.DO.NotExistExceptions ex)
        {
            throw new BO.DataError(ex);
        }
    }
    /// <summary>
    /// updating the quantity in a order item in the given cart
    /// </summary>
    /// <param name="cart">the customer cart</param>
    /// <param name="id">the product id</param>
    /// <param name="quantity">the new quantity</param>
    /// <returns></returns>
    /// <exception cref="BO.NotExistExceptions"></exception>
    public BO.Cart UpdateOrderItem(BO.Cart cart, int id, int quantity)
    {
            BO.OrderItem OItem = cart.Items.Find(OItem => OItem.ProductID == id);
            if(OItem == null)
                throw new BO.NotExistExceptions();
            if (quantity == 0)
            {
                cart.TotalPrice -= OItem.TotalPrice;
                cart.Items.Remove(OItem);
            }
            else if (OItem.Amount > quantity || OItem.Amount < quantity)
            {
                double lastTotalPrice = OItem.TotalPrice;
                OItem.Amount = quantity;
                OItem.TotalPrice = quantity * OItem.Price;
                cart.TotalPrice = cart.TotalPrice - lastTotalPrice + OItem.TotalPrice;
            }
            return cart;
    }
    /// <summary>
    /// submitting the cart and creating a new order
    /// </summary>
    /// <param name="cart">the customer cart</param>
    /// <param name="CustomerName">the customer name</param>
    /// <param name="CustomerEmail">the customer email</param>
    /// <param name="CustomerAddress">the customer address</param>
    /// <exception cref="BO.DataError">error from the database=dal</exception>
    public void SubmitOrder(BO.Cart cart, string CustomerName, string CustomerEmail, string CustomerAddress)
    {

        IsValidCart(cart, CustomerName, CustomerEmail, CustomerAddress); //check validation of cart and customer details
        Dal.DO.Order newOrder = new Dal.DO.Order();
        newOrder.Customer_Address = CustomerAddress;
        newOrder.Customer_Name = CustomerName;
        newOrder.Customer_Email = CustomerEmail;
        newOrder.Order_Date= DateTime.Now;
        newOrder.Ship_Date = DateTime.MinValue;
        newOrder.Delivery_Date = DateTime.MinValue;
        int id = dal.iorder.Create(newOrder);
        foreach(BO.OrderItem BOoi in cart.Items)
            {
            try
            {
                Dal.DO.OrderItem DOoi = new Dal.DO.OrderItem();
                DOoi.Order_ID = id;
                DOoi.Product_Price = BOoi.Price;
                DOoi.Product_ID = BOoi.ProductID;
                DOoi.Product_Amount= BOoi.Amount;
                dal.iorderItem.Create(DOoi);
                Dal.DO.Product product= dal.iproduct.Read(DOoi.Product_ID);
                product.InStock -= DOoi.Product_Amount;
                dal.iproduct.Update(product);
            }
            catch (Dal.DO.NotExistExceptions ex)
            {
                throw new BO.DataError(ex);
            }
            catch (BO.NotInStockException ex)
            {
                throw ex;
            }
            catch (BO.PropertyInValidException ex)
            {
                throw ex;
            }

        }

    }
    /// <summary>
    /// checks if the cart has valid values
    /// </summary>
    /// <param name="cart">the customer cart</param>
    /// <param name="CustomerName">the customer name</param>
    /// <param name="CustomerEmail">the customer email</param>
    /// <param name="CustomerAddress">the customer address</param>
    /// <exception cref="BO.PropertyInValidException">in valid property</exception>
    /// <exception cref="BO.NotInStockException"></exception>
    /// <exception cref="BO.DataError"></exception>
    public void IsValidCart(BO.Cart cart, string CustomerName, string CustomerEmail, string CustomerAddress)
    {
        foreach (BO.OrderItem OItem in cart.Items)
        {
            try
            {
                Dal.DO.Product product = dal.iproduct.Read(OItem.ProductID);
                if (OItem.Amount < 0)
                {
                    throw new BO.PropertyInValidException("Amount");
                }
                if (OItem.Amount > product.InStock)
                {
                    throw new BO.NotInStockException(product.Name);
                }
            }
            catch (Dal.DO.NotExistExceptions ex)
            {
                throw new BO.DataError(ex);
            }
        }

        if (!IsValidEmail(CustomerEmail))
        {
            throw new BO.PropertyInValidException("Email");
        }
        if (CustomerAddress == "") 
            throw new BO.PropertyInValidException("address");
        if (CustomerName == "")
            throw new BO.PropertyInValidException("name");
        if (CustomerEmail == "")
            throw new BO.PropertyInValidException("Email");
    }
    /// <summary>
    /// checks if the email is valid
    /// </summary>
    /// <param name="email">the customer email</param>
    /// <returns></returns>
    bool IsValidEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith("."))
        {
            return false; // suggested by @TK-421
        }
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }

    }
}