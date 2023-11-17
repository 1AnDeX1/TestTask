using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestTask.Data;
using TestTask.Models;

namespace TestTask.Controllers
{
    public class FoldersController : Controller
    {
        private readonly FolderDbContext _db;

        public FoldersController(FolderDbContext db) {  _db = db; }

        public IActionResult Index(int? id = null)
        {
            var childCatalogs = _db.Folders.Where(c => c.ParentFolderId == id).ToList();
            if (childCatalogs == null)
            {
                return NotFound();
            }

            var parentFolder = _db.Folders.FirstOrDefault(c => c.Id == id);
            if (id != null)
            {
                ViewBag.ParentFolderName = parentFolder.Name;
            }
            else
            {
                ViewBag.ParentFolderName = null;
            }
            
            return View(childCatalogs);
        }
        [HttpGet]
        public IActionResult ExportToFile()
        {
            var folders = _db.Folders.ToList(); // Отримання всіх папок з бази даних

            // Серіалізація в JSON
            var json = JsonConvert.SerializeObject(folders, Formatting.Indented);

            // Збереження у файл JSON
            var fileName = "folders.json"; // Назва файлу
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName); // Шлях до файлу
            System.IO.File.WriteAllText(filePath, json); // Запис у файл

            // Повернення повідомлення користувачеві
            return Content("Structure exported to file");
        }
        [HttpPost]
        public async Task<IActionResult> ImportFromFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            // Видалення наявних даних перед імпортом нових
            _db.Folders.RemoveRange(_db.Folders); // Видалення всіх папок

            // Отримання тексту з файлу
            string jsonContent;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                jsonContent = await reader.ReadToEndAsync();
            }

            // Десеріалізація JSON у список об'єктів
            var folders = JsonConvert.DeserializeObject<List<Folder>>(jsonContent);

            // Додавання нових даних до бази даних
            _db.Folders.AddRange(folders);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
