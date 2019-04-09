using Extensions.IQueryable.Filtering;
using Extensions.IQueryable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Filter filter = new Filter("Price", FilteringOperators.LessThan, 31);
            Filter filter2 = new Filter("Make", FilteringOperators.Equal, "Audi");

            var result = cars.FilterBy(filter, filter2);
        }
    }

    public class Car
    {
        public int Price { get; set; }
        public string Make { get; set; }
    }
}
