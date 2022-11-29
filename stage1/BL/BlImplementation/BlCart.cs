﻿
using BlApi;
using Dal;
using DalApi;
namespace BlImplementation;
internal class BlCart:ICart
{
    IDal dal = new DalList();
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
    public BO.Cart UpdateOrderItem(BO.Cart cart, int id, int quantity)
    {
            BO.OrderItem OItem = cart.Items.Find(OItem => OItem.ID == id);
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
    public void SubmitOrder(BO.Cart cart, string CustomerName, string CustomerEmail, string CustomerAddress)
    {
        // catch all the ex in IsValidCart ?????????????
        IsValidCart(cart, CustomerName, CustomerEmail, CustomerAddress); //check valiation of cart and customer details
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

        }

    }
    public void IsValidCart(BO.Cart cart, string CustomerName, string CustomerEmail, string CustomerAddress)
    {
        foreach (BO.OrderItem OItem in cart.Items)
        {
            try
            {
                Dal.DO.Product product = dal.iproduct.Read(OItem.ProductID);
                if (OItem.Amount > 0)
                {
                    throw new BO.PropertyInValidException("Amount");
                }
                if (OItem.Amount > product.InStock)
                {
                    throw new BO.NotInStockException(product.Name);
                }
            }
            catch (Dal.DO.NotExistExceptions)
            {
                throw new BO.NotExistExceptions();
            }
        }

        if (!IsValidEmail(CustomerEmail))
        {
            throw new BO.EmailIsNotValidException();
        }
        if (CustomerAddress == "") 
            throw new BO.CustomerDetailsAreUnknown("address");
        if (CustomerName == "")
            throw new BO.CustomerDetailsAreUnknown("name");
        if (CustomerEmail == "")
            throw new BO.CustomerDetailsAreUnknown("Email");
    } 
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