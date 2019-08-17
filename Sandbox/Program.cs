using Extensions.IQueryable.Filtering;
using Extensions.IQueryable;
using Extensions.IEnumerable;
using System.Collections.Generic;
using System.Linq;
using Extensions.IQueryable.Pagination;
using System;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            IQueryable<Car> cars = new List<Car>
            {
                new Car
                {
                    Price = 20,
                    Make = "Audi",
                    ProductionDate = DateTime.Now
                },
                new Car
                {
                    Price = 50,
                    Make = "Audi",
                    ProductionDate = DateTime.Now.AddDays(1)
                },
                new Car
                {
                    Price = 30,
                    Make = "Bmw",
                    ProductionDate = DateTime.Now.AddDays(-1)
                }
            }.AsQueryable();
            

            Filter filter = new Filter("Price", FilteringOperator.LessThan, 31);
            Filter filter2 = new Filter("Make", FilteringOperator.Equal, "Audi");
            Filter filter3 = new Filter("ProductionDate", FilteringOperator.GreaterThan, DateTime.Now);

            var result = cars.FilterBy(filter3).Paginated(new PaginationInfo(2, 1));
        }
    }

    public class Car
    {
        public int Price { get; set; }
        public string Make { get; set; }
        public DateTime ProductionDate { get; set; }
    }
}
