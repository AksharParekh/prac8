using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace prac8.Models
{
    public class InventoryDAL
    {
        private readonly string _connectionString;

        public InventoryDAL()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["InventoryDbConn"].ConnectionString;
        }

        // CREATE: Insert a new inventory item using SqlCommand & Parameters
        public bool InsertItem(InventoryItem item)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Inventory (ItemName, Category, Quantity, UnitPrice, Description, CreatedDate)
                                 VALUES (@ItemName, @Category, @Quantity, @UnitPrice, @Description, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                    cmd.Parameters.AddWithValue("@Category", item.Category);
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                    cmd.Parameters.AddWithValue("@Description", (object)item.Description ?? DBNull.Value);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // READ: Retrieve all inventory items with optional search and category filters
        public List<InventoryItem> GetAllItems(string searchTerm = null, string categoryFilter = null)
        {
            List<InventoryItem> items = new List<InventoryItem>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT ItemId, ItemName, Category, Quantity, UnitPrice, Description, CreatedDate FROM Inventory WHERE 1=1";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND (ItemName LIKE @Search OR Description LIKE @Search)";
                }
                if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter != "All")
                {
                    query += " AND Category = @Category";
                }

                query += " ORDER BY ItemId DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@Search", "%" + searchTerm + "%");
                    }
                    if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter != "All")
                    {
                        cmd.Parameters.AddWithValue("@Category", categoryFilter);
                    }

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new InventoryItem
                            {
                                ItemId = Convert.ToInt32(reader["ItemId"]),
                                ItemName = reader["ItemName"].ToString(),
                                Category = reader["Category"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : string.Empty,
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            });
                        }
                    }
                }
            }

            return items;
        }

        // READ: Retrieve a single inventory item by ItemId
        public InventoryItem GetItemById(int id)
        {
            InventoryItem item = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT ItemId, ItemName, Category, Quantity, UnitPrice, Description, CreatedDate FROM Inventory WHERE ItemId = @ItemId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ItemId", id);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            item = new InventoryItem
                            {
                                ItemId = Convert.ToInt32(reader["ItemId"]),
                                ItemName = reader["ItemName"].ToString(),
                                Category = reader["Category"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : string.Empty,
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            };
                        }
                    }
                }
            }

            return item;
        }

        // UPDATE: Modify existing inventory item by ItemId
        public bool UpdateItem(InventoryItem item)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Inventory 
                                 SET ItemName = @ItemName, 
                                     Category = @Category, 
                                     Quantity = @Quantity, 
                                     UnitPrice = @UnitPrice, 
                                     Description = @Description 
                                 WHERE ItemId = @ItemId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ItemId", item.ItemId);
                    cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                    cmd.Parameters.AddWithValue("@Category", item.Category);
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                    cmd.Parameters.AddWithValue("@Description", (object)item.Description ?? DBNull.Value);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // DELETE: Remove inventory item by ItemId
        public bool DeleteItem(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Inventory WHERE ItemId = @ItemId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ItemId", id);
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // Distinct category listing for filtering
        public List<string> GetCategories()
        {
            List<string> categories = new List<string>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT DISTINCT Category FROM Inventory ORDER BY Category";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(reader["Category"].ToString());
                        }
                    }
                }
            }

            return categories;
        }
    }
}
