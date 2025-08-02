using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace NencerApi.Helpers
{
    public class EfPatchHelper
    {
        public static void PatchEntityFromDto<TDto, TEntity>(
        TDto dto,
        TEntity entity,
        DbContext context,
        IMapper mapper)
        where TDto : class
        where TEntity : class
        {
            // Dùng AutoMapper để gán dữ liệu từ DTO vào Entity
            mapper.Map(dto, entity);

            // Duyệt các property trong DTO
            foreach (var prop in typeof(TDto).GetProperties())
            {
                var value = prop.GetValue(dto);
                if (value != null)
                {
                    context.Entry(entity).Property(prop.Name).IsModified = true;
                }
            }
        }
    }
}
