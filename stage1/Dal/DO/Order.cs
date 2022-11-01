﻿


namespace Dal.DO;

public struct Order
{
    public int Order_ID { get; set; }
    public string Customer_Name { get; set; }
    public string Customer_Email { get; set; }
    public string Customer_Address { get; set; }
    public DateTime Order_Date { get; set; }
    public DateTime Ship_Date { get; set; }
    public DateTime Delivery_Date { get; set; }
}
