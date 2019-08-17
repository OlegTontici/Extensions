using Extensions.IQueryable.Filtering;
using Extensions.IQueryable;
using System.Collections.Generic;
using System.Linq;
using Extensions.IQueryable.Pagination;

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
                    Make = "Audi"
                },
                new Car
                {
                    Price = 50,
                    Make = "Audi"
                },
                new Car
                {
                    Price = 30,
                    Make = "Bmw"
                }
            }.AsQueryable();
            

            Filter filter = new Filter("Price", FilteringOperator.LessThan, 31);
            Filter filter2 = new Filter("Make", FilteringOperator.Equal, "Audi");

            var result = cars.FilterBy(filter).Paginated(new PaginationInfo(1, 1));
        }
    }

    public class Car
    {
        public int Price { get; set; }
        public string Make { get; set; }
    }
}
