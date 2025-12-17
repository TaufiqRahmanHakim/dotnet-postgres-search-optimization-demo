using Bogus;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;

// 1. Definisi Entity
public class Customer {
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = "";
    public string City { get; set; } = "";
    public bool IsActive { get; set; }
    public int LoyaltyPoints { get; set; }
}

// 2. DbContext
public class AppDbContext : DbContext {
    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        // Ganti connection string sesuai database lokal Anda
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=asembaris;Database=CustomerDB");
    }
}

// 3. Program Utama
class Program {
    static void Main(string[] args) {
        using var context = new AppDbContext();

        // Pastikan database fresh (Hati-hati, ini menghapus DB lama!)
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        Console.WriteLine("Database dibuat. Mulai generate data...");

        // Konfigurasi BOGUS untuk data palsu
        var faker = new Faker<Customer>("id_ID") 
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
            .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(c => c.DateOfBirth, f => f.Date.Past(40).ToUniversalTime()) 
            .RuleFor(c => c.Address, f => f.Address.StreetAddress())
            .RuleFor(c => c.City, f => f.Address.City())
            .RuleFor(c => c.IsActive, f => f.Random.Bool())
            .RuleFor(c => c.LoyaltyPoints, f => f.Random.Int(0, 1000));

        int totalData = 500_000;
        int batchSize = 5_000;

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < totalData; i += batchSize) {
            var batch = faker.Generate(batchSize);
            context.Customers.AddRange(batch);
            context.SaveChanges();
            context.ChangeTracker.Clear();
            Console.WriteLine($"Progress: {i + batchSize} / {totalData} data tersimpan...");
        }

        stopwatch.Stop();
        Console.WriteLine($"SELESAI! Waktu total: {stopwatch.Elapsed.TotalSeconds} detik.");
        Console.WriteLine("Sekarang silakan mainkan query Anda!");
    }
}