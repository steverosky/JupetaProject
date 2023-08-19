using Amazon.Auth.AccessControlPolicy;
using Jupeta.Models.DBModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Jupeta.Models.RequestModels
{
    public class AddProductModel
    {
        [MaxLength(100), MinLength(6)]
        public required string ProductName { get; set; }
        public required string Description { get; set; }
        public required string Summary { get; set; }
        public required double Price { get; set; }
        public required bool IsAvailable { get; set; }
        public required Category Category { get; set; }
        public required SellingType SellingType { get; set; }
        public required Condition Condition { get; set; }
        public required int Quantity { get; set; }
        public required IFormFile ImageFile { get; set; }
               
    }

    public enum Category
    {
        MobilePhones = 1,
        HomeDecor,
        Electronics,
        Clothing,
        Beauty,
        Books,
        Sports,
        Automotive,
        Toys

    }
    public enum SellingType
    {
        BuyNow = 1,
        Auction

    }
    public enum Condition
    {
        New = 1,
        Used,
        Refurbished

    }
}

//private static readonly string[] AllowedConditions = { "Used", "New", "Refurbished" };

//private string? condition;

//public required string Condition
//{
//    get => condition!;
//    set
//    {
//        if (AllowedConditions.Contains(value))
//        {
//            condition = value;
//        }
//        else
//        {
//            throw new ArgumentException("Invalid condition value. Allowed conditions are: Used, New, Refurbished");
//        }
//    }
//}