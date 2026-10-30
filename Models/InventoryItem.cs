using System;
using System.ComponentModel.DataAnnotations;

namespace prac8.Models
{
    public class InventoryItem
    {
        [Display(Name = "Item ID")]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Item name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Item name must be between 2 and 100 characters.")]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, 100000, ErrorMessage = "Quantity must be a non-negative number.")]
        [Display(Name = "Stock Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit price is required.")]
        [Range(0.01, 10000000.00, ErrorMessage = "Unit price must be greater than 0.")]
        [Display(Name = "Unit Price (Rs.)")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal UnitPrice { get; set; }

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Date Added")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime CreatedDate { get; set; }
    }
}
