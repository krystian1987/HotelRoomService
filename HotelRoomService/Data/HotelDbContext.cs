using HotelRoomService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomService.Data
{
  public class HotelDbContext : DbContext
  {
    public DbSet<Room> Rooms { get; set; }

    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Room>().HasKey(r => r.Id);
    }
  }
}
