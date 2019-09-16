using Extensions.IQueryable.Filtering;
using Extensions.IQueryable.Pagination;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Extensions.IQueryable.Tests
{
    public class Car
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public string Make { get; set; }
        public DateTime ProductionDate { get; set; }
        public bool IsSecondHand { get; set; }

        public bool Equals(Car obj)
        {
            return obj != null &&
                Price == obj.Price &&
                Make == obj.Make &&
                ProductionDate == obj.ProductionDate;
        }
    }

    [TestClass]
    public class IQueryableExtensionsTest
    {
        private static readonly Car bmw = new Car
        {
            Id = Guid.NewGuid(),
            Make = "BMW",
            Price = 15000,
            ProductionDate = new DateTime(2018, 04, 04),
            IsSecondHand = true
        };
        private static readonly Car toyota = new Car
        {
            Id = Guid.NewGuid(),
            Make = "Toyota",
            Price = 20000,
            ProductionDate = new DateTime(2017, 04, 04),
            IsSecondHand = false
        };
        private static readonly Car renault = new Car
        {
            Id = Guid.NewGuid(),
            Make = "Renault",
            Price = 6000,
            ProductionDate = new DateTime(2014, 04, 04),
            IsSecondHand = true
        };
        private static IQueryable<Car> cars = new List<Car>
        {
            bmw, toyota, renault
        }.AsQueryable();

        [TestMethod]
        public void Should_Correctly_Paginate_ProvidedData()
        {
            // Act
            var paginatedQuery = cars.AsQueryable().Paginated(new PaginationInfo(2, 2));
            var result = paginatedQuery.ToList();

            // Assert
            Assert.IsTrue(result.SequenceEqual(new List<Car> { cars.Last() }));
        }

        #region type string
        [TestMethod]
        public void Should_Correctly_Apply_Equal_Filter_On_String_Type()
        {
            //Act

            var filter = new SimpleFilter(nameof(Car.Make), FilteringOperator.Equal, bmw.Make)
                .Or(new ScopedFilter(new SimpleFilter(nameof(Car.Make), FilteringOperator.Equal, toyota.Make).And(new SimpleFilter(nameof(Car.Price), FilteringOperator.Equal, 20001))))
                .And(new SimpleFilter(nameof(Car.Make), FilteringOperator.Equal, renault.Make));

            var result = cars.FilterBy(filter).ToList();

            //    var result = cars.FilterBy(new Filter(nameof(Car.Make), FilteringOperator.Equal, bmw.Make, LogicalConnection.Or),
            //new Filter(nameof(Car.Make), FilteringOperator.Equal, toyota.Make, LogicalConnection.And),
            //new Filter(nameof(Car.Price), FilteringOperator.Equal, 20000, LogicalConnection.Or),
            //new Filter(nameof(Car.Make), FilteringOperator.Equal, "Renault", LogicalConnection.Or)).ToList();

            //    Assert.IsNotNull(result);
            //Assert.AreEqual(result.Count(), 1);
            //Assert.AreEqual(result.Count(c => c.Equals(bmw)), 1);
        }

        //[TestMethod]
        //public void Should_Correctly_Apply_NotEqual_Filter_On_String_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.Make), FilteringOperator.NotEqual, bmw.Make)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 2);
        //    Assert.AreEqual(result.Count(c => !c.Equals(bmw)), 2);
        //}

        //[TestMethod]
        //public void Should_Correctly_Apply_Contains_Filter_On_String_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.Make), FilteringOperator.Contains, "a")).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 2);
        //    Assert.AreEqual(result.Count(c => c.Equals(toyota)), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(renault)), 1);
        //}

        //[TestMethod]
        //public void Should_Correctly_Apply_StartsWith_Filter_On_String_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.Make), FilteringOperator.StartsWith, "T")).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(toyota)), 1);
        //}

        //[TestMethod]
        //public void Should_Throw_ArgumentException_When_LessThan_Filter_Is_Applied_On_String_Type()
        //{
        //    // Act and Assert
        //    Assert.ThrowsException<ArgumentException>(() => cars.FilterBy(new Filter(nameof(Car.Make), FilteringOperator.LessThan, bmw.Make)).ToList());
        //}

        //[TestMethod]
        //public void Should_Throw_ArgumentException_When_LessThanOrEqual_Filter_Is_Applied_On_String_Type()
        //{
        //    // Act and Assert
        //    Assert.ThrowsException<ArgumentException>(() => cars.FilterBy(new Filter(nameof(Car.Make), FilteringOperator.LessThanOrEqual, bmw.Make)).ToList());
        //}

        //[TestMethod]
        //public void Should_Throw_ArgumentException_When_GreaterThan_Filter_Is_Applied_On_String_Type()
        //{
        //    // Act and Assert
        //    Assert.ThrowsException<ArgumentException>(() => cars.FilterBy(new Filter(nameof(Car.Make), FilteringOperator.GreaterThan, bmw.Make)).ToList());
        //}

        //[TestMethod]
        //public void Should_Throw_ArgumentException_When_GreaterThanOrEqual_Filter_Is_Applied_On_String_Type()
        //{
        //    // Act and Assert
        //    Assert.ThrowsException<ArgumentException>(() => cars.FilterBy(new Filter(nameof(Car.Make), FilteringOperator.GreaterThanOrEqual, bmw.Make)).ToList());
        //}
        //#endregion

        //#region number types
        //[TestMethod]
        //public void Should_Correctly_Apply_Equal_Filter_On_Decimal_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.Price), FilteringOperator.Equal, bmw.Price)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(bmw)), 1);
        //}

        //[TestMethod]
        //public void Should_Correctly_Apply_NotEqual_Filter_On_Decimal_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.Price), FilteringOperator.NotEqual, bmw.Price)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 2);
        //    Assert.AreEqual(result.Count(c => c.Equals(renault)), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(toyota)), 1);
        //}

        //[TestMethod]
        //public void Should_Correctly_Apply_LessThan_Filter_On_Decimal_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.Price), FilteringOperator.LessThan, bmw.Price)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(renault)), 1);
        //}

        //[TestMethod]
        //public void Should_Correctly_Apply_LessThanOrEqual_Filter_On_Decimal_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.Price), FilteringOperator.LessThanOrEqual, bmw.Price)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 2);
        //    Assert.AreEqual(result.Count(c => c.Equals(renault)), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(bmw)), 1);
        //}

        //[TestMethod]
        //public void Should_Correctly_Apply_GreaterThan_Filter_On_Decimal_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.Price), FilteringOperator.GreaterThan, bmw.Price)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(toyota)), 1);
        //}

        //[TestMethod]
        //public void Should_Correctly_Apply_GreaterThanOrEqual_Filter_On_Decimal_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.Price), FilteringOperator.GreaterThanOrEqual, bmw.Price)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 2);
        //    Assert.AreEqual(result.Count(c => c.Equals(toyota)), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(bmw)), 1);
        //}
        //#endregion

        //#region type DateTime
        //[TestMethod]
        //public void Should_Correctly_Apply_Equal_Filter_On_DateTime_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.ProductionDate), FilteringOperator.Equal, bmw.ProductionDate)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(bmw)), 1);
        //}

        //[TestMethod]
        //public void Should_Correctly_Apply_NotEqual_Filter_On_DateTime_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.ProductionDate), FilteringOperator.NotEqual, bmw.ProductionDate)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 2);
        //    Assert.AreEqual(result.Count(c => c.Equals(renault)), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(toyota)), 1);
        //}

        //[TestMethod]
        //public void Should_Correctly_Apply_LessThan_Filter_On_DateTime_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.ProductionDate), FilteringOperator.LessThan, bmw.ProductionDate)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 2);
        //    Assert.AreEqual(result.Count(c => c.Equals(toyota)), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(renault)), 1);
        //}

        //[TestMethod]
        //public void Should_Correctly_Apply_LessThanOrEqual_Filter_On_DateTime_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.ProductionDate), FilteringOperator.LessThanOrEqual, bmw.ProductionDate)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 3);
        //    Assert.AreEqual(result.Count(c => c.Equals(toyota)), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(renault)), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(bmw)), 1);
        //}

        //[TestMethod]
        //public void Should_Correctly_Apply_GreaterThan_Filter_On_DateTime_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.ProductionDate), FilteringOperator.GreaterThan, bmw.ProductionDate)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 0);
        //}

        //[TestMethod]
        //public void Should_Correctly_Apply_GreaterThanOrEqual_Filter_On_DateTime_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.ProductionDate), FilteringOperator.GreaterThanOrEqual, bmw.ProductionDate)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(bmw)), 1);
        //}

        //[TestMethod]
        //public void Should_Throw_ArgumentException_When_Contains_Filter_Is_Applied_On_DateTime_Type()
        //{
        //    // Act and Assert
        //    Assert.ThrowsException<ArgumentException>(() => cars.FilterBy(new Filter(nameof(Car.ProductionDate), FilteringOperator.Contains, DateTime.Now)).ToList());
        //}

        //[TestMethod]
        //public void Should_Throw_ArgumentException_When_StartsWith_Filter_Is_Applied_On_DateTime_Type()
        //{
        //    // Act and Assert
        //    Assert.ThrowsException<ArgumentException>(() => cars.FilterBy(new Filter(nameof(Car.ProductionDate), FilteringOperator.StartsWith, DateTime.Now)).ToList());
        //}
        //#endregion

        //#region type bool
        //[TestMethod]
        //public void Should_Correctly_Apply_Equal_Filter_On_Bool_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.IsSecondHand), FilteringOperator.Equal, true)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 2);
        //    Assert.AreEqual(result.Count(c => c.Equals(bmw)), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(renault)), 1);
        //}

        //[TestMethod]
        //public void Should_Correctly_Apply_NotEqual_Filter_On_Bool_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.IsSecondHand), FilteringOperator.NotEqual, true)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(toyota)), 1);
        //}
        //#endregion

        //#region type Guid
        //[TestMethod]
        //public void Should_Correctly_Apply_Equal_Filter_On_Guid_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.Id), FilteringOperator.Equal, bmw.Id)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(bmw)), 1);
        //}

        //[TestMethod]
        //public void Should_Correctly_Apply_NotEqual_Filter_On_Guid_Type()
        //{
        //    // Act
        //    var result = cars.FilterBy(new Filter(nameof(Car.Id), FilteringOperator.NotEqual, bmw.Id)).ToList();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(result.Count(), 2);
        //    Assert.AreEqual(result.Count(c => c.Equals(toyota)), 1);
        //    Assert.AreEqual(result.Count(c => c.Equals(renault)), 1);
        //}
        #endregion
    }
}
