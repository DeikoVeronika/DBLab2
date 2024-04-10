using BeautySpaceDomain.Model;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BeautySpaceInfrastructure.Services
{
    public class CategoryExportService : IExportService<Category>
    {
        private const string RootWorksheetName = "";

        private static readonly IReadOnlyList<string> HeaderNames =
            new string[]
            {
                "Назва",
                "Опис",
                "Ціна",
                "Категорія",
            };

        private readonly DbbeautySpaceContext _context;

        private static void WriteHeader(IXLWorksheet worksheet)
        {
            for (int columnIndex = 0; columnIndex < HeaderNames.Count; columnIndex++)
            {
                worksheet.Cell(1, columnIndex + 1).Value = HeaderNames[columnIndex];
            }
            worksheet.Row(1).Style.Font.Bold = true;
        }

        private void WriteService(IXLWorksheet worksheet, Service service, int rowIndex)
        {
            var columnIndex = 1;
            worksheet.Cell(rowIndex, columnIndex++).Value = service.Name;
            worksheet.Cell(rowIndex, columnIndex++).Value = service.Description;
            worksheet.Cell(rowIndex, columnIndex++).Value = service.Price;
            worksheet.Cell(rowIndex, columnIndex++).Value = service.Category.Name;
        }

        private void WriteServices(IXLWorksheet worksheet, ICollection<Service> services)
        {
            WriteHeader(worksheet);
            int rowIndex = 2;
            foreach (var service in services)
            {
                WriteService(worksheet, service, rowIndex);
                rowIndex++;
            }
        }

        private void WriteCategories(XLWorkbook workbook, ICollection<Category> categories)
        {
            foreach (var category in categories)
            {
                if (category is not null)
                {
                    var worksheet = workbook.Worksheets.Add(category.Name);
                    WriteServices(worksheet, category.Services.ToList());
                }
            }
        }


        public CategoryExportService(DbbeautySpaceContext context)
        {
            _context = context;
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanWrite)
            {
                throw new ArgumentException("Потік не дозволяє запис", nameof(stream));
            }

            var categories = await _context.Categories
                .Include(category => category.Services)
                .ToListAsync(cancellationToken);

            var workbook = new XLWorkbook();

            WriteCategories(workbook, categories);
            workbook.SaveAs(stream);
        }
    }
}
