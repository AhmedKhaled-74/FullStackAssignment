using FullStackAssignment.Application.DTOs.CategoryDTOs;
using FullStackAssignment.Domain.Entites;

namespace FullStackAssignment.Application.Mappers
{
    public static class CategoryMappers
    {
        public static Category ToCategoryEntity(this CategoryRequestAdd categoryRequestAdd)
        {
            return new Category()
            {
                CategoryName = categoryRequestAdd.CategoryName,
                CategoryId = Guid.NewGuid(),
            };
        }
        public static Category ToCategoryEntityByUpdate(this CategoryRequestUpdate categoryRequestUpdate)
        {
            return new Category()
            {
                CategoryName = categoryRequestUpdate.CategoryName,
                CategoryId = categoryRequestUpdate.CategoryId,
            };
        }
        public static CategoryResult ToCategoryResultEntity(this Category category)
        {
            return new CategoryResult()
            {
                CategoryName = category.CategoryName,
                CategoryId = category.CategoryId,
            };
        }
    }
}
