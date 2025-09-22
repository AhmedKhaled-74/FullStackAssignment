using FullStackAssignment.Application.DTOs.ProductDTOs;
using FullStackAssignment.Application.DTOs.UserDTOs;
using FullStackAssignment.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.Mappers
{
    public static class ProductMappers
    {
        public static Product ToProductEntity(this ProductRequestAdd productRequestAdd) 
        { 
           return new Product()
            {
                Name = productRequestAdd.Name,
                Price = productRequestAdd.Price,
                DiscountRate = productRequestAdd.DiscountRate,
                CategoryId = productRequestAdd.CategoryId,
                MinimumQuantity = productRequestAdd.MinimumQuantity,     
            };
        }
        public static Product ToProductEntityByUpdate(this ProductRequestUpdate productRequestUpdate)
        {
            return new Product()
            {
                ProductCode = productRequestUpdate.ProductCode,
                Name = productRequestUpdate.Name,
                Price = productRequestUpdate.Price,
                DiscountRate = productRequestUpdate.DiscountRate,
                CategoryId = productRequestUpdate.CategoryId,
                MinimumQuantity = productRequestUpdate.MinimumQuantity,
            };
        }
        public static ProductResult ToProductResultEntity(this Product product)
        {
            return new ProductResult()
            {
                ProductCode = product.ProductCode,
                Name = product.Name,
                Price = product.Price,
                DiscountRate = product.DiscountRate,
                Category = product.Category.CategoryName,
                Image = product.Image,
                MinimumQuantity = product.MinimumQuantity,
            };
        }
    }
}
