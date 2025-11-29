using Microsoft.EntityFrameworkCore;
using QMS.Domain.Entities;
using QMS.Infrastructure.Persistence;

namespace QMS.Web.Data;

public static class ServiceTypeSeedData
{
    public static async Task SeedServicesAsync(QmsDbContext context)
    {
        var existingServices = await context.ServiceTypes.ToListAsync();
        
        if (existingServices.Any())
        {
            Console.WriteLine($"Updating {existingServices.Count} existing services with icons and colors...");
            
            foreach (var service in existingServices)
            {
                // Update icon and color for all services
                service.IconClass = GetIconForCode(service.Code);
                service.ColorCode = GetColorForCode(service.Code);
                Console.WriteLine($"  - {service.Name}: {service.IconClass} / {service.ColorCode}");
            }
            
            await context.SaveChangesAsync();
            Console.WriteLine("✓ All services updated successfully!");
            return;
        }

        Console.WriteLine("No existing services found. Creating sample services...");
        await CreateSampleServices(context);
    }

    private static async Task CreateSampleServices(QmsDbContext context)
    {

        var services = new List<ServiceType>
        {
            new ServiceType
            {
                Name = "Deposit & Withdrawal",
                Code = "DEPOSIT_WITHDRAW",
                Description = "Cash transactions and account deposits",
                IconClass = "bi-cash-coin",
                ColorCode = "#4facfe",
                IsActive = true
            },
            new ServiceType
            {
                Name = "Loans & Credit",
                Code = "LOAN",
                Description = "Personal and business loans",
                IconClass = "bi-bank",
                ColorCode = "#43e97b",
                IsActive = true
            },
            new ServiceType
            {
                Name = "Credit Card Services",
                Code = "CREDIT_CARD",
                Description = "Credit card applications and support",
                IconClass = "bi-credit-card",
                ColorCode = "#f093fb",
                IsActive = true
            },
            new ServiceType
            {
                Name = "Account Opening",
                Code = "ACCOUNT_OPEN",
                Description = "Open new savings or checking accounts",
                IconClass = "bi-person-badge",
                ColorCode = "#667eea",
                IsActive = true
            },
            new ServiceType
            {
                Name = "Money Transfer",
                Code = "TRANSFER",
                Description = "Domestic and international transfers",
                IconClass = "bi-arrow-left-right",
                ColorCode = "#30cfd0",
                IsActive = true
            },
            new ServiceType
            {
                Name = "Investment Advisory",
                Code = "INVESTMENT",
                Description = "Investment and wealth management advice",
                IconClass = "bi-graph-up-arrow",
                ColorCode = "#fa709a",
                IsActive = true
            },
            new ServiceType
            {
                Name = "Insurance Services",
                Code = "INSURANCE",
                Description = "Life, health, and property insurance",
                IconClass = "bi-shield-check",
                ColorCode = "#8b5cf6",
                IsActive = true
            },
            new ServiceType
            {
                Name = "VIP Priority",
                Code = "VIP",
                Description = "Priority service for VIP customers",
                IconClass = "bi-star-fill",
                ColorCode = "#f59e0b",
                IsActive = true
            },
            new ServiceType
            {
                Name = "Business Banking",
                Code = "BUSINESS",
                Description = "Corporate and business account services",
                IconClass = "bi-briefcase",
                ColorCode = "#10b981",
                IsActive = true
            },
            new ServiceType
            {
                Name = "Mortgage Services",
                Code = "MORTGAGE",
                Description = "Home loans and mortgage refinancing",
                IconClass = "bi-house-door",
                ColorCode = "#ff6b6b",
                IsActive = true
            }
        };

        await context.ServiceTypes.AddRangeAsync(services);
        await context.SaveChangesAsync();

        Console.WriteLine($"Successfully seeded {services.Count} services!");
    }


    private static string GetIconForCode(string code)
    {
        return code.ToUpper() switch
        {
            var c when c.Contains("DEPOSIT") || c.Contains("WITHDRAW") => "bi-cash-coin",
            var c when c.Contains("LOAN") => "bi-bank",
            var c when c.Contains("CARD") || c.Contains("CREDIT") => "bi-credit-card",
            var c when c.Contains("ACCOUNT") => "bi-person-badge",
            var c when c.Contains("TRANSFER") => "bi-arrow-left-right",
            var c when c.Contains("INVEST") => "bi-graph-up-arrow",
            var c when c.Contains("INSURANCE") => "bi-shield-check",
            var c when c.Contains("VIP") => "bi-star-fill",
            var c when c.Contains("BUSINESS") => "bi-briefcase",
            var c when c.Contains("MORTGAGE") || c.Contains("HOME") => "bi-house-door",
            _ => "bi-gear"
        };
    }

    private static string GetColorForCode(string code)
    {
        return code.ToUpper() switch
        {
            var c when c.Contains("DEPOSIT") || c.Contains("WITHDRAW") => "#4facfe",
            var c when c.Contains("LOAN") => "#43e97b",
            var c when c.Contains("CARD") || c.Contains("CREDIT") => "#f093fb",
            var c when c.Contains("ACCOUNT") => "#667eea",
            var c when c.Contains("TRANSFER") => "#30cfd0",
            var c when c.Contains("INVEST") => "#fa709a",
            var c when c.Contains("INSURANCE") => "#8b5cf6",
            var c when c.Contains("VIP") => "#f59e0b",
            var c when c.Contains("BUSINESS") => "#10b981",
            var c when c.Contains("MORTGAGE") || c.Contains("HOME") => "#ff6b6b",
            _ => "#667eea"
        };
    }
}
