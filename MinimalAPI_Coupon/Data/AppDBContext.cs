﻿using Microsoft.EntityFrameworkCore;
using MinimalAPI_Coupon.Models;

namespace MinimalAPI_Coupon.Data {
    public class AppDBContext : DbContext {

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) {
             
        }

        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupon>().HasData(
                new Coupon() {
                    Id = 1,
                    Name = "10OFFA",
                    Percent = 10,
                    IsActive = true,
                },
                new Coupon() {
                    Id = 2,
                    Name = "20OFF",
                    Percent = 20,
                    IsActive = false,
                },
                new Coupon() {
                    Id = 3,
                    Name = "30OFF",
                    Percent = 30,
                    IsActive = false,
                }
            );
        }

    }
}
