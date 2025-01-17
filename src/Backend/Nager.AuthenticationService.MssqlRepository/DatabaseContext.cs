﻿using Microsoft.EntityFrameworkCore;
using Nager.AuthenticationService.Abstraction.Entities;

namespace Nager.AuthenticationService.MssqlRepository
{
    public class DatabaseContext : DbContext
    {
        private readonly DbContextOptions<DatabaseContext> _options;

        public DbSet<UserEntity> Users { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
           : base(options)
        {
            this._options = options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().HasIndex(entity => new { entity.EmailAddress });
        }
    }
}
