using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestTask.Data;

namespace TestTask.Controllers
{
    public class FoldersController : Controller
    {
        private readonly FolderDbContext _db;

        public FoldersController(FolderDbContext db) {  _db = db; }

        public IActionResult Index(int id)
        {
            var catalog = _db.Folders.Find(id);
            if (catalog == null)
            {
                return NotFound();
            }

            return View(catalog);
        }
    }
}
