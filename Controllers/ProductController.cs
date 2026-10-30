using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Practical8.Models;

namespace Practical8.Controllers
{
    public class ProductController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["InventoryDbConn"].ConnectionString;

        // GET: Product
        public ActionResult Index()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT ProductId, ProductName, Quantity, Price FROM Products", con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    products.Add(new Product
                    {
                        ProductId = Convert.ToInt32(rdr["ProductId"]),
                        ProductName = rdr["ProductName"].ToString(),
                        Quantity = Convert.ToInt32(rdr["Quantity"]),
                        Price = Convert.ToDecimal(rdr["Price"])
                    });
                }
            }

            return View(products);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(Product p)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "INSERT INTO Products (ProductName, Quantity, Price) VALUES (@name, @qty, @price)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", p.ProductName);
                cmd.Parameters.AddWithValue("@qty", p.Quantity);
                cmd.Parameters.AddWithValue("@price", p.Price);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int id)
        {
            Product p = new Product();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT ProductId, ProductName, Quantity, Price FROM Products WHERE ProductId = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    p.ProductId = Convert.ToInt32(rdr["ProductId"]);
                    p.ProductName = rdr["ProductName"].ToString();
                    p.Quantity = Convert.ToInt32(rdr["Quantity"]);
                    p.Price = Convert.ToDecimal(rdr["Price"]);
                }
            }

            return View(p);
        }

        // POST: Product/Edit/5
        [HttpPost]
        public ActionResult Edit(Product p)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "UPDATE Products SET ProductName = @name, Quantity = @qty, Price = @price WHERE ProductId = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", p.ProductId);
                cmd.Parameters.AddWithValue("@name", p.ProductName);
                cmd.Parameters.AddWithValue("@qty", p.Quantity);
                cmd.Parameters.AddWithValue("@price", p.Price);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // POST: Product/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "DELETE FROM Products WHERE ProductId = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
