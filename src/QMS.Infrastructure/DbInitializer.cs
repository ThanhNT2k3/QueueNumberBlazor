using QMS.Domain.Entities;
using QMS.Infrastructure.Persistence;

namespace QMS.Infrastructure;

public static class DbInitializer
{
    public static async Task InitializeAsync(QmsDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (context.Branches.Any())
        {
            return; // DB has been seeded
        }

        var branches = new Branch[]
        {
            new Branch { Name = "Saigon Centre Branch", Code = "SGN01", Address = "65 Le Loi, District 1, HCMC" },
            new Branch { Name = "Hanoi Tower Branch", Code = "HAN01", Address = "49 Hai Ba Trung, Hanoi" }
        };
        await context.Branches.AddRangeAsync(branches);
        await context.SaveChangesAsync();

        var serviceTypes = new ServiceType[]
        {
            new ServiceType { Name = "Deposit & Withdrawal", Code = "DEP", Description = "Cash transactions" },
            new ServiceType { Name = "Loans & Credit", Code = "LOAN", Description = "Personal and business loans" },
            new ServiceType { Name = "Advisory Services", Code = "ADV", Description = "Investment and insurance advice" },
            new ServiceType { Name = "VIP Priority", Code = "VIP", Description = "Priority service for VIP customers" }
        };
        await context.ServiceTypes.AddRangeAsync(serviceTypes);
        await context.SaveChangesAsync();

        var counters = new Counter[]
        {
            new Counter { Name = "Counter 1", Prefix = "A", BranchId = branches[0].Id, IsActive = true },
            new Counter { Name = "Counter 2", Prefix = "B", BranchId = branches[0].Id, IsActive = true },
            new Counter { Name = "VIP Counter", Prefix = "V", BranchId = branches[0].Id, IsActive = true },
            new Counter { Name = "Counter A", Prefix = "C", BranchId = branches[1].Id, IsActive = true },
            new Counter { Name = "Counter B", Prefix = "D", BranchId = branches[1].Id, IsActive = true }
        };
        await context.Counters.AddRangeAsync(counters);
        await context.SaveChangesAsync();

        var users = new User[]
        {
            new User { ExternalId = "EMP001", FullName = "Nguyen Van A", Email = "nguyenvana@scb.com", PhoneNumber = "0901234567", Role = "Teller", CurrentBranchId = branches[0].Id, CurrentCounterId = counters[0].Id },
            new User { ExternalId = "EMP002", FullName = "Tran Thi B", Email = "tranthib@scb.com", PhoneNumber = "0902345678", Role = "Teller", CurrentBranchId = branches[0].Id, CurrentCounterId = counters[1].Id },
            new User { ExternalId = "EMP003", FullName = "Le Van C", Email = "levanc@scb.com", PhoneNumber = "0903456789", Role = "Teller", CurrentBranchId = branches[0].Id, CurrentCounterId = counters[2].Id },
            new User { ExternalId = "EMP004", FullName = "Pham Thi D", Email = "phamthid@scb.com", PhoneNumber = "0904567890", Role = "Teller", CurrentBranchId = branches[1].Id, CurrentCounterId = counters[3].Id },
            new User { ExternalId = "EMP005", FullName = "Hoang Van E", Email = "hoangvane@scb.com", PhoneNumber = "0905678901", Role = "Teller", CurrentBranchId = branches[1].Id, CurrentCounterId = counters[4].Id },
            new User { ExternalId = "MGR001", FullName = "Vo Thi Manager", Email = "vothimgr@scb.com", PhoneNumber = "0906789012", Role = "Manager", CurrentBranchId = branches[0].Id },
            new User { ExternalId = "MGR002", FullName = "Do Van Manager", Email = "dovanmgr@scb.com", PhoneNumber = "0907890123", Role = "Manager", CurrentBranchId = branches[1].Id },
            new User { ExternalId = "TM001", FullName = "Tran Van TM", Email = "tranvantm@scb.com", PhoneNumber = "0908901234", Role = "TM", CurrentBranchId = branches[0].Id },
            new User { ExternalId = "TM002", FullName = "Le Thi TM", Email = "lethitm@scb.com", PhoneNumber = "0909012345", Role = "TM", CurrentBranchId = branches[1].Id }
        };
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // Seed Counter-ServiceType mappings
        var counterServiceMappings = new CounterServiceType[]
        {
            // Counter 1 (A) - General services
            new CounterServiceType { CounterId = counters[0].Id, ServiceTypeId = serviceTypes[0].Id, Priority = 1, IsPrimary = true }, // Deposit & Withdrawal
            new CounterServiceType { CounterId = counters[0].Id, ServiceTypeId = serviceTypes[1].Id, Priority = 2, IsPrimary = false }, // Loans (backup)
            
            // Counter 2 (B) - Loans specialist
            new CounterServiceType { CounterId = counters[1].Id, ServiceTypeId = serviceTypes[1].Id, Priority = 1, IsPrimary = true }, // Loans & Credit
            new CounterServiceType { CounterId = counters[1].Id, ServiceTypeId = serviceTypes[2].Id, Priority = 2, IsPrimary = false }, // Advisory (backup)
            
            // VIP Counter (V) - All services (Backup counter)
            new CounterServiceType { CounterId = counters[2].Id, ServiceTypeId = serviceTypes[3].Id, Priority = 1, IsPrimary = true }, // VIP Priority
            new CounterServiceType { CounterId = counters[2].Id, ServiceTypeId = serviceTypes[0].Id, Priority = 2, IsPrimary = false }, // Deposit (backup)
            new CounterServiceType { CounterId = counters[2].Id, ServiceTypeId = serviceTypes[1].Id, Priority = 3, IsPrimary = false }, // Loans (backup)
            new CounterServiceType { CounterId = counters[2].Id, ServiceTypeId = serviceTypes[2].Id, Priority = 4, IsPrimary = false }, // Advisory (backup)
            
            // Hanoi counters
            new CounterServiceType { CounterId = counters[3].Id, ServiceTypeId = serviceTypes[0].Id, Priority = 1, IsPrimary = true },
            new CounterServiceType { CounterId = counters[4].Id, ServiceTypeId = serviceTypes[1].Id, Priority = 1, IsPrimary = true }
        };
        await context.CounterServiceTypes.AddRangeAsync(counterServiceMappings);
        await context.SaveChangesAsync();
    }
}
