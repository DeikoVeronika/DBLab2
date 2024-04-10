using BeautySpaceDomain.Model;

namespace BeautySpaceInfrastructure.Services
{
    public class CategoryDataPortServiceFactory
    : IDataPortServiceFactory<Category>
    {
        private readonly DbbeautySpaceContext _context;
        public CategoryDataPortServiceFactory(DbbeautySpaceContext context)
        {
            _context = context;
        }
        public IImportService<Category> GetImportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new CategoryImportService(_context);
            }
            throw new NotImplementedException($"Під час завантаження файлу виникла помилка. Оберіть інший файл.");
        }
        public IExportService<Category> GetExportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                // Поверніть відповідний експортний сервіс тут
                return new CategoryExportService(_context);

            }

            throw new NotImplementedException($"No export service implemented for Category with content type {contentType}");

        }
    }
}
