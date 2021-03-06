//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebEcom.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Product
    {
        public int Product_Id { get; set; }
        [Display(Name ="Product Name")]
        public string Product_Name { get; set; }
        [Display(Name = "Product Description")]
        public string Product_Descr { get; set; }
        [Display(Name = "Product Price")]
        public decimal Product_price { get; set; }
        [Display(Name = "Select Category")]
        public Nullable<int> Product_CategoryID { get; set; }
        [Display(Name = "Select Image")]
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
        public int Id_USER { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual User User { get; set; }
    }
}
