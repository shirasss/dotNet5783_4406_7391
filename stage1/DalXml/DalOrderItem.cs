﻿using Dal.DO;
using DalApi;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Dal;

internal class DalOrderItem : IorderItem
{
    //  getting the id from the xml and updating it
    public int getIDAndUpdateXml()
    {
        XmlRootAttribute IDSRoot = new XmlRootAttribute();
        IDSRoot.ElementName = "IDS";
        IDSRoot.IsNullable = true;
        StreamReader read = new("../../xml/ConfigData.xml");
        XmlSerializer serID = new XmlSerializer(typeof(IDSConfig), IDSRoot);
        IDSConfig allIDS = ((IDSConfig)serID.Deserialize(read));
        int orderID = allIDS.OrderItemId;
        allIDS.OrderItemId++;
        read.Close();
        StreamWriter write = new("../../xml/ConfigData.xml");
        serID.Serialize(write, allIDS);
        write.Close();
        return orderID;
    }

    /// <summary>
    /// creatnig a new orderItem
    /// </summary>
    /// <param name="orderItem"></param>
    /// <returns></returns>
    public int Create(OrderItem orderItem)
    {

        orderItem.OrderItem_ID= getIDAndUpdateXml();   
        XmlRootAttribute xRoot = new XmlRootAttribute();
        xRoot.ElementName = "OrderItems";
        xRoot.IsNullable = true;
        StreamReader sread = new StreamReader("../../xml/OrderItem.xml");
        XmlSerializer ser = new XmlSerializer(typeof(List<OrderItem>), xRoot);
        List<OrderItem> orderItemsList = (List<OrderItem>)ser.Deserialize(sread);
        sread.Close();
        orderItemsList.Add(orderItem);
        StreamWriter swrite = new StreamWriter("../../xml/OrderItem.xml"); 
        ser.Serialize(swrite, orderItemsList);
        swrite.Close();
        return orderItem.OrderItem_ID;

    }
    /// <summary>
    /// Deleting a certain orderItem
    /// </summary>
    /// <param name="id"></param>
    public void Delete(int id)
    {
        XmlRootAttribute xRoot = new XmlRootAttribute();
        xRoot.ElementName = "OrderItems";
        xRoot.IsNullable = true;
        StreamReader sread = new StreamReader("../../xml/OrderItem.xml");
        XmlSerializer ser = new XmlSerializer(typeof(List<OrderItem>), xRoot);
        List<OrderItem> OrderItemsList = (List<OrderItem>)ser.Deserialize(sread);
        sread.Close();
        OrderItem order = OrderItemsList.Where(o => o.OrderItem_ID == id).First();
        OrderItemsList.Remove(order);
        StreamWriter swrite = new("../../xml/OrderItem.xml");
        ser.Serialize(swrite, OrderItemsList);
        swrite.Close();
    }
    /// <summary>
    /// reading the orderItems list by an optional given filter
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public IEnumerable<OrderItem> ReadByFilter(Func<OrderItem, bool> f = null)
    {
        XmlRootAttribute xRoot = new XmlRootAttribute();
        xRoot.ElementName = "OrderItems";
        xRoot.IsNullable = true;
        StreamReader sread = new StreamReader("../../xml/OrderItem.xml");
        XmlSerializer ser = new XmlSerializer(typeof(List<OrderItem>), xRoot);
        List<OrderItem> OrderItemsList = (List<OrderItem>)ser.Deserialize(sread);
        sread.Close();
        return f==null?OrderItemsList:OrderItemsList.Where(f);
    }
    /// <summary>
    /// reading a certain orderItem by a given condition
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public OrderItem ReadSingle(Func<OrderItem, bool> f)
    {
        XmlRootAttribute xRoot = new XmlRootAttribute();
        xRoot.ElementName = "OrderItems";
        xRoot.IsNullable = true;
        StreamReader sread = new StreamReader("../../xml/OrderItem.xml");
        XmlSerializer ser = new XmlSerializer(typeof(List<OrderItem>), xRoot);
        List<OrderItem> OrderItemsList = (List<OrderItem>)ser.Deserialize(sread);
        sread.Close();
        return OrderItemsList.Where(f).First();
    }
    /// <summary>
    /// read orderItem by product's id and order's id
    /// </summary>
    /// <param name="order_id"></param>
    /// <param name="product_id"></param>
    /// <returns></returns>
    public OrderItem Read_item_by_product_order(int order_id, int product_id)
    {
        XmlRootAttribute xRoot = new XmlRootAttribute();
        xRoot.ElementName = "OrderItems";
        xRoot.IsNullable = true;
        StreamReader sread = new StreamReader("../../xml/OrderItem.xml");
        XmlSerializer ser = new XmlSerializer(typeof(List<OrderItem>), xRoot);
        List<OrderItem> OrderItemsList = (List<OrderItem>)ser.Deserialize(sread);
        sread.Close();
        return OrderItemsList.Where(oi=> oi.Order_ID==order_id && oi.Product_ID== product_id).First();
    }
    /// <summary>
    /// updating a certain orderItem
    /// </summary>
    /// <param name="orderItem"></param>
    /// <returns></returns>
    /// <exception cref="NotExistExceptions"></exception>

    public bool Update(OrderItem orderItem)
    {
        

        XmlRootAttribute xRoot = new XmlRootAttribute();
        xRoot.ElementName = "OrderItems";
        xRoot.IsNullable = true;
        StreamReader sread = new StreamReader("../../xml/OrderItem.xml");
        XmlSerializer ser = new XmlSerializer(typeof(List<OrderItem>), xRoot);
        List<OrderItem> orderItemsList = (List<OrderItem>)ser.Deserialize(sread);
        sread.Close();
        int index = orderItemsList.FindIndex(oi => orderItem.OrderItem_ID == oi.OrderItem_ID);
        if(index == -1 ) throw new NotExistExceptions();
        orderItemsList[index]=orderItem;
        StreamWriter swrite = new StreamWriter("../../xml/OrderItem.xml");
        ser.Serialize(swrite, orderItemsList);
        swrite.Close();
        return true;
    }
}

