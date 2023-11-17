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

            // Отримання фізичного шляху до папки Files
            var filesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Files");

            // Перевірка існування папки Files
            if (!Directory.Exists(filesFolderPath))
            {
                Directory.CreateDirectory(filesFolderPath); // Створення папки, якщо вона не існує
            }

            // Шлях до файлу JSON у папці Files
            var filePath = Path.Combine(filesFolderPath, "folders.json");

            // Запис у файл JSON у папці Files
            System.IO.File.WriteAllText(filePath, json);

            // Повернення повідомлення користувачеві
            return Content("Structure exported to file");
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            _db.Folders.RemoveRange(_db.Folders); 

            string jsonContent;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                jsonContent = await reader.ReadToEndAsync();
            }

            var folders = JsonConvert.DeserializeObject<List<Folder>>(jsonContent);

            _db.Folders.AddRange(folders);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public IActionResult ImportAndExportOfFiles()
        {
            return View();
        }
    }
}
