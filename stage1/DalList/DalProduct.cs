﻿using Dal.DO;
using DalApi;

namespace Dal;

internal class DalProduct: Iproduct
{
    /// <summary>
    /// creating a new product
    /// </summary>
    /// <param name="product"></param>
    /// <returns>return the order id</returns>
    public int Create(Product product)
    {
       
        product.ID = DataSource.Config.Product_ID;
        DataSource.ProductsList.Add(product);
        return product.ID;
    }
    /// <summary>
    /// Updating a certain product
    /// </summary>
    /// <param name="product"></param>
    /// <returns>1 in case of succeed</returns>
    /// <exception cref="NotExistExceptions"></exception>
    public bool Update(Product product)
    {
        for (int i = 0; i < DataSource.ProductsList.Count(); i++)
        {
            if (DataSource.ProductsList[i].ID == product.ID)
            {
                DataSource.ProductsList[i] = product;
                return true;
            }
        }
            throw new NotExistExceptions();
    }
    /// <summary>
    /// Deleting a certain product
    /// </summary>
    /// <param name="ID"></param>
    /// <exception cref="NotExistExceptions"></exception>
    public void Delete(int ID)
    {
        foreach(Product p in DataSource.ProductsList)
        {
            if (p.ID == ID)
            {
                DataSource.ProductsList.Remove(p);
                return;
            }
        }
        throw new NotExistExceptions();
    }
    
    /// <summary>
    /// Reading  the product list by an optional given filter 
    /// </summary>
    /// <returns>the product list</returns>
    public IEnumerable<Product> ReadByFilter(Func<Product, bool>? f = null)
    {
        if (f == null)
        {
            return DataSource.ProductsList;
        }
        IEnumerable<Product> products = DataSource.ProductsList;
        return products.Where(f);
    }
    /// <summary>
    /// Reading a certain product by a given condition
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    /// <exception cref="NotExistExceptions"></exception>
    public Product ReadSingle(Func<Product, bool> f)
    {
        IEnumerable<Product> products = DataSource.ProductsList;
        Product product;
        try
        {
            product = products.Where(f).First();
            return product;
        }
        catch (Exception ex)
        {
           throw new NotExistExceptions();
        }
    }
}

