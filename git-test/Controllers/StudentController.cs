using Elasticsearch.Net;
using git_test.Data;
using git_test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using System.ComponentModel.DataAnnotations;

namespace git_test.Controllers
{
    public class StudentController : Controller       
    {
        private readonly AppDbContext _dbContext;
        private readonly IElasticClient _elasticClient;
        public StudentController(AppDbContext dbContext, IElasticClient elasticClient) 
        {
            _dbContext = dbContext;
            _elasticClient = elasticClient;
           
        }

        
        public async Task<IActionResult> List(string SearchString)
        {
            List<Student> students = _dbContext.Students.ToList();

            foreach (var item in students)
            {
                var Indexing = _elasticClient.IndexDocument(item);
            }


            if (!string.IsNullOrEmpty(SearchString))
            {

                
                var response = _elasticClient.Search<Student>(s => s
           .Query(q => q
               .Fuzzy(fz => fz
                   .Field(f => f.Name)
                       .Value(SearchString)
                           .Fuzziness(Fuzziness.Auto)

               )
           )
       );
                List<Student> searchResults = response.Documents.ToList();
                return View(searchResults);
            }


            return View(students);
        }


        
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Student obj)
        {
            var IsNameExist = _dbContext.Students.Any(x => x.Name == obj.Name);

            if (IsNameExist)
            {
                ModelState.AddModelError("Name", "user name is taken");
                return View("Add");
            }

            var IsNumberExist = _dbContext.Students.Any(x => x.Number == obj.Number);

            if (IsNumberExist)
            {
                ModelState.AddModelError("Nmuber", "number is taken");
                return View("Add");
            }

            var IsEmailExist = _dbContext.Students.Any(x => x.Email == obj.Email);

            if (IsEmailExist)
            {
                ModelState.AddModelError("Email", "Email is used");
                return View("Add");
            }


            if (ModelState.IsValid)
            {
                _dbContext.Students.Add(obj);
                _dbContext.SaveChanges();
                TempData["success"] = "student addeed seccess";
                return RedirectToAction("List", "Student");
            }

            return View();
        }


        
        public async Task<IActionResult> Edit(Guid id)
        {
            Student Student = _dbContext.Students.Find(id);
            if (Student == null)
            {
                return NotFound();
            }

            return View(Student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Student obj)
        {
            if(ModelState.IsValid)
            {
                _dbContext.Students.Update(obj);
                _dbContext.SaveChanges();
                TempData["success"] = "student updated seccess";
                return RedirectToAction("List");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Student obj)
        {
            var student = await _dbContext.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == obj.Id);

            if(student is not null)
            {
                _dbContext.Students.Remove(obj);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List");
        }
    }
}
