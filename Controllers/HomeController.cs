using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ASPNetCore_CoreIdentity_user_data_issue.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ASPNetCore_CoreIdentity_user_data_issue.Data;

namespace ASPNetCore_CoreIdentity_user_data_issue.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
		{
			_logger = logger;
			_userManager = userManager;
			_context = context;
			_signInManager = signInManager;
		}

		// Here, for brevity
		public class IndexDataModel
		{
			public ApplicationUser user;
			public Foo[] foos;
		}

		[Authorize]
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			var foos = _context.Foos.Where(f => f.User == user).ToArray();
			return View(new IndexDataModel() { user = user, foos = foos });
		}

		[Authorize]
		public async Task<IActionResult> IncrementBar()
		{
			// This DOES NOT work
			var user = await _userManager.GetUserAsync(HttpContext.User);
			if (user.Bar == null)	//< this is always null
			{
				user.Bar = new Bar() { Value = 0 };
				_context.Add(user.Bar); // doesn't help
			}
			user.Bar.Value++;
			await _userManager.UpdateAsync(user); // doesn't help
			await _signInManager.RefreshSignInAsync(user);  // doesn't help, starting to get desperate
			await _context.SaveChangesAsync(); // doesn't help

			return RedirectToAction(nameof(Index));
		}


		[Authorize]
		public async Task<IActionResult> AddFoo()
		{
			// This works
			var user = await _userManager.GetUserAsync(HttpContext.User);
			var foo = new Foo() { User = user };
			_context.Add(foo);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}


		[Authorize]
		public async Task<IActionResult> ResetCustomTag()
		{
			// This works
			var user = await _userManager.GetUserAsync(HttpContext.User);
			user.CustomTag = Guid.NewGuid().ToString();
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
