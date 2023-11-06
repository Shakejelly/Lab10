using Lab10.Data;
using Lab10.Models;
using Microsoft.EntityFrameworkCore;
using System.CodeDom.Compiler;

namespace Lab10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool menu = true;
            while (menu == true) { 
            Console.WriteLine("Welcome! You have three choices. 1: Get all customers. 2: Make a new customer. 3. Quit");
            string GREEN = Console.IsOutputRedirected ? "" : "\x1b[92m";
            string NORMAL = Console.IsOutputRedirected ? "" : "\x1b[39m";
            using (NorthwindContext context = new NorthwindContext())
            {
                int menuChoice = Convert.ToInt32(Console.ReadLine());
                if ( menuChoice == 1)
                {
                    Console.WriteLine("How do you want to sort the company names given?");
                    Console.Write($"{GREEN}A.{NORMAL} A-Ö.   ");
                    Console.WriteLine($"{GREEN}B.{NORMAL} Ö-A.");
                    string input = Console.ReadLine();

                    var getCustomer = context.Customers
                        .Include(c => c.Orders)
                        .Select(c => new
                        {
                            c.CustomerId,
                            c.CompanyName,
                            c.Country,
                            c.Region,
                            c.Phone,
                            TotalOrders = c.Orders.Count
                        });

                    if (input.Equals("A", StringComparison.OrdinalIgnoreCase))
                    {
                        getCustomer = getCustomer.OrderBy(c => c.CompanyName);
                    }
                    else if (input.Equals("B", StringComparison.OrdinalIgnoreCase))
                    {
                        getCustomer = getCustomer.OrderByDescending(c => c.CompanyName);
                    }
                    else
                    {
                        Console.WriteLine($"Wrong choice, {GREEN}A{NORMAL} will automatically be chosen. ");
                        getCustomer = getCustomer.OrderBy(c => c.CompanyName);
                        Console.ReadKey();
                    }
                    var getCustomers = getCustomer.ToList();

                    for (int i = 0; i < getCustomers.Count; i++)
                    {
                        var customer = getCustomers[i];
                        Console.WriteLine($"{i}. {GREEN}Company:{NORMAL} {customer.CompanyName}. {GREEN}Country:{NORMAL} {customer.Country}. {GREEN}Region:{NORMAL}. {customer.Region}. {GREEN}Phonenumber:{NORMAL}{customer.Phone}.{GREEN}Total Orders:{NORMAL} {customer.TotalOrders}");
                    }

                    if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0)
                    {
                        var chosenCustomer = getCustomers[choice];

                        Console.WriteLine($"You chose {choice}: {chosenCustomer.CompanyName}");

                        var companyInfo = context.Customers
                            .Where(ao => ao.CustomerId == chosenCustomer.CustomerId)
                            .Select(ao => new
                            {
                                ao.CustomerId,
                                ao.CompanyName,
                                ao.ContactName,
                                ao.ContactTitle,
                                ao.Address,
                                ao.City,
                                ao.PostalCode,
                                ao.Country,
                                ao.Phone,
                            })
                            .ToList();

                        foreach (var ao in companyInfo)
                        {
                            Console.WriteLine($"Company: {ao.CompanyName}. Contact Name: {ao.ContactName} ,  {ao.ContactTitle}. Location: {ao.Country}, {ao.City}: {ao.Address}({ao.PostalCode}) Phone: {ao.Phone}");
                        }

                        var customerOrders = context.Orders
                            .Where(o => o.CustomerId == chosenCustomer.CustomerId)
                            .Select(o => o.OrderId);

                        var products = context.OrderDetails
                            .Where(od => customerOrders.Contains(od.OrderId))
                            .Select(od => new { od.Product.ProductName })
                            .ToList();


                        foreach (var product in products)
                        {
                            Console.WriteLine($"{product.ProductName}");
                        }
                    }
                }
                else if (menuChoice == 2)
                {
                    AddCustomer(context);
                }
                else if (menuChoice == 3)
                {
                    Console.WriteLine("Bye!");
                    Console.ReadKey();
                        menu = false;
                }
                else Console.WriteLine("Invalid input, going back to menu.");
                    Console.ReadKey();
                    Console.Clear();
                        

                }
            }
            
        } 
        
        static void AddCustomer(NorthwindContext context)
        {
            Console.WriteLine("Add a new customer:");
            string customerId = null;
           if (string.IsNullOrEmpty(customerId))
            {
                customerId = RandomCustomerID();
            }
            Console.WriteLine("What is the name of the company?");
            string companyName = Console.ReadLine();
            Console.Write("What is the contact name?");
            string contactName = Console.ReadLine();
            Console.Write($"What is the title of {contactName}?");
            string title = Console.ReadLine();
            Console.Clear();
            Console.Write("Insert country: ");
            string country = Console.ReadLine();
            Console.Write("Insert city: ");
            string city = Console.ReadLine();
            Console.Write("Insert adress: ");
            string adress = Console.ReadLine();
            Console.Write("Insert Postal Code: ");
            string postalCode = Console.ReadLine();
            Console.Write("Insert your phone number: ");
            string phone = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Your input was successful and will now be saved.");


            var newCustomer = new Customer
            {
                CustomerId = customerId,
                CompanyName = companyName,
                ContactName = contactName,
                ContactTitle = title,
                Country = country,
                City = city,
                PostalCode = postalCode,
                Phone = phone

            };

            context.Customers.Add(newCustomer);
            context.SaveChanges();

           
        }
        
        
        static string RandomCustomerID()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            var customerId = new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return customerId;
                
        }

        
        
    }
    
}