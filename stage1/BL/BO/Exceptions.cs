﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;
public class NotExistExceptions : Exception
{
    public override string Message => "object does not exist";

}
public class IDNotValidException : Exception
{
    public override string Message => "the id is not valid";

}
public class ProductExistsInOrderException : Exception
{
    public override string Message => "the product exists in one or more orders";

}
public class PropertyInValidException : Exception
{
    public string Property { get; }
    public PropertyInValidException(string msg)
    {
        Property = msg;
    }
    public override string Message => $"Invalid property- {Property}";

}
//------------------------------------- ?
public class DataError : Exception
{
    public DataError(Exception ERR) : base("", ERR) { }
    public override string Message => $" data error: {InnerException.Message}";
}