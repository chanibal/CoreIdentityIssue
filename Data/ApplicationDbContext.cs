using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace ASPNetCore_CoreIdentity_user_data_issue.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
			{
		}

		public DbSet<Bar> Bars { get; set; }

		public DbSet<Foo> Foos { get; set; }

		public DbSet<ApplicationUser> Users { get; set; }
	}

	
	// All models in the same file, for brevity 


	public class ApplicationUser : IdentityUser
	{
		public string CustomTag { get; set; }
		public Bar Bar { get; set; }
	}


	public class Bar
	{
		public int Id { get; set; }
		public int Value { get; set; }
	}


	public class Foo
	{
		public int Id { get; set; }
		public ApplicationUser User { get; set; }
	}
}
