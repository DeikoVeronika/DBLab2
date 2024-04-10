using BeautySpaceDomain.Model;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BeautySpaceInfrastructure.Services
{
    public class CategoryImportService : IImportService<Category>
    {
        private readonly DbbeautySpaceContext _context;
        public CategoryImportService(DbbeautySpaceContext context)
        {
            _context = context;
        }
        public async Task<bool> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Дані не можуть бути прочитані", nameof(stream));
            }

            using (XLWorkbook workBook = new XLWorkbook(stream))
            {
                ValidateWorkbook(workBook);

                bool isNewServiceCreated = await ProcessWorksheets(workBook, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return isNewServiceCreated;
            }
        }
        private void ValidateWorkbook(XLWorkbook workBook)
        {
            if (!workBook.Worksheets.Any() || workBook.Worksheets.All(ws => ws.RowsUsed().Count() <= 1))
            {
                throw new ArgumentException("Excel файл пустий або містить лише заголовки без даних");
            }

        }
        private async Task<bool> ProcessWorksheets(XLWorkbook workBook, CancellationToken cancellationToken)
        {
            bool isNewServiceCreated = false;

            foreach (IXLWorksheet worksheet in workBook.Worksheets)
            {
                var categoryName = worksheet.Name;
                var categoryService = await GetCategory(categoryName, cancellationToken);

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    bool isServiceAdded = await AddServiceAsync(row, cancellationToken, categoryService);
                    isNewServiceCreated = isNewServiceCreated || isServiceAdded;
                }
            }

            return isNewServiceCreated;
        }

        private async Task<Category> GetCategory(string categoryName, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(category => category.Name == categoryName, cancellationToken);

            if (category == null)
            {
                category = new Category { Name = categoryName };
                _context.Categories.Add(category);
            }

            return category;
        }

        private async Task<bool> AddServiceAsync(IXLRow row, CancellationToken cancellationToken, Category category)
        {
            ValidateHeader(row);

            Service service = CreateService(row);

            var existingService = await _context.Services
                .FirstOrDefaultAsync(s => EF.Functions.Like(s.Name, service.Name) && EF.Functions.Like(s.Description, service.Description), cancellationToken);

            if (existingService != null)
            {
                return false;
            }

            ValidateService(service, row, category);

            service.Category = category;
            _context.Services.Add(service);
            return true;
        }

        private static void ValidateHeader(IXLRow row)
        {
            var cells = row.Cells().Take(3).ToList();
            if (cells.Any(cell => string.IsNullOrEmpty(cell.Value.ToString())))
            {
                throw new ArgumentException($"Невірна структура заголовка. Потрібно мати 3 стовпці в такому порядку: Назва, Опис, Ціна  ");
            }
        }

        private static Service CreateService(IXLRow row)
        {
            return new Service
            {
                Name = GetServiceName(row),
                Description = GetServiceDescription(row),
                Price = GetServicePrice(row)
            };
        }

        private static string GetServiceName(IXLRow row)
        {
            var nameCell = row.Cell(1).Value.ToString();
            if (string.IsNullOrWhiteSpace(nameCell))
            {
                throw new ArgumentException($"Назва не може бути пустою. Помилка в рядку {row.RowNumber()}");
            }
            return nameCell;
        }
        private static string GetServiceDescription(IXLRow row)
        {
            return row.Cell(2).Value.ToString();
        }

        private static decimal GetServicePrice(IXLRow row)
        {
            var priceCell = row.Cell(3);

            if (priceCell.IsEmpty() || string.IsNullOrWhiteSpace(priceCell.Value.ToString()))
            {
                throw new ArgumentException($"Ціна послуги не може бути порожньою. Помилка в рядку {row.RowNumber()}");
            }

            if (!decimal.TryParse(priceCell.Value.ToString(), out decimal price))
            {
                throw new ArgumentException($"Ціна має бути числом. Помилка в рядку {row.RowNumber()}: \"{priceCell.Value}\"");
            }
            return price;
        }

        private static void ValidateService(Service service, IXLRow row, Category category)
        {
            List<string> errors = new List<string>();

            if (!IsValidName(service.Name))
            {
                errors.Add($"Неправильний формат назви в рядку {row.RowNumber()} на сторінці {category.Name}: \"{service.Name}\". Назва має містити пронаймні дві літери.");
            }

            if (!IsValidPrice(service.Price))
            {
                errors.Add($"Неправильний формат ціни в рядку {row.RowNumber()} на сторінці {category.Name}: \"{service.Price}\"");
            }

            if (errors.Any())
            {
                throw new ArgumentException(string.Join("\n", errors));
            }
        }

        public static bool IsValidName(string name)
        {
            Regex regexLength = new Regex(@"^.{2,50}$");
            Regex regexLetters = new Regex(@"^.*[а-яА-Яa-zA-Z].*[а-яА-Яa-zA-Z].*$");
            return regexLength.IsMatch(name) && regexLetters.IsMatch(name);
        }



        private static bool IsValidPrice(decimal price)
        {
            return price > 0;
        }
    }
}
