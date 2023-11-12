using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OnlineShopCMS.Data;
using OnlineShopCMS.Models;

namespace OnlineShopCMS.Controllers
{
    public class ProductsController : Controller
    {
        private readonly OnlineShopCMSContext _context;

        public ProductsController(OnlineShopCMSContext context)
        {
            _context = context;
        }

        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            _context.Category.Add(category);
            await _context.SaveChangesAsync();
            return View();
        }

        // GET: Products
            public async Task<IActionResult> Index(string searchString, string currentFilter, int? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;  // 儲存當前搜尋狀態

            var result = from m in _context.Product select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(s => s.Name.Contains(searchString));
            }

            int pageSize = 5;  //一頁顯示幾項
            var res = new IndexViewModel
            {
                Data= result.Include(p => p.Category).AsNoTracking().ToList(),
                PageInfo= await PaginatedList<Product>.CreateAsync(
                result.Include(p => p.Category).AsNoTracking(), pageNumber ?? 1, pageSize)
            };//為什麼要這樣改
            return View(res);
        }


        //{

        //      return _context.Product != null ? 
        //                  View(await _context.Product.Include(c=>c.Category).ToListAsync()) :
        //                  Problem("Entity set 'OnlineShopCMSContext.Product'  is null.");
        //}

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DetailViewModel dvm = new DetailViewModel();  //建立一個 ViewModel

            var product = await _context.Product
                        .Include(p => p.Category)
                        .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                dvm.product = product;
                if (product.Image != null)
                {
                    dvm.imgsrc = ViewImage(product.Image);
                }
            }
            return View(dvm); //回傳 ViewModel
        }

        private string ViewImage(byte[] arrayImage)
        {
            // 二進位圖檔轉字串
            string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);
            return "data:image/png;base64," + base64String;
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(_context.Set<Category>(), "Id", "Name");
            return View();
        }


        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile myimg)
        {
            if (ModelState.IsValid)
            {
                if (myimg != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        myimg.CopyTo(ms);
                        product.Image = ms.ToArray();
                    }
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categories"] = new SelectList(
                _context.Set<Category>(), "Id", "Name", product.CategoryId);
            return View(product);
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Categories"] = new SelectList(_context.Set<Category>(), "Id", "Name");
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,CategoryId")] Product product)
        {
            
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var model = _context.Product.FirstOrDefault(m => m.Id == id);
                    model.Name = product.Name;
                    model.Price = product.Price;
                    model.CategoryId = product.CategoryId;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
               .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);




        }


        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'OnlineShopCMSContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
