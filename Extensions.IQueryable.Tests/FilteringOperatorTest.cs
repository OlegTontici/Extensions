using Extensions.IQueryable.Filtering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Extensions.IQueryable.Tests
{
    [TestClass]
    public class FilteringOperatorTest
    {
        [TestMethod]
        public void Should_Confirm_Equality_Of_Two_Filters()
        {
            // Arrange
            var firstFilter = FilteringOperator.Equal;
            var secondFilter = FilteringOperator.Equal;

            // Act and Assert
            Assert.IsTrue(firstFilter.Equals(secondFilter));
        }

        [TestMethod]
        public void Should_Infirm_Equality_Of_Two_Filters()
        {
            // Arrange
            var firstOperator = FilteringOperator.Equal;
            var secondOperator = FilteringOperator.Contains;

            // Act and Assert
            Assert.IsFalse(firstOperator.Equals(secondOperator));
        }

        [TestMethod]
        public void Should_Return_Operator_DisplayName_Once_Converted_To_String()
        {
            // Arrange
            var firstOperator = FilteringOperator.Equal;

            // Act and Assert
            Assert.AreEqual(firstOperator.ToString(), firstOperator.DisplayName);
        }

        [TestMethod]
        public void Should_Return_All_Available_Filtering_Operators()
        {
            // Arrange and Act 
            var filteringOperators = FilteringOperator.GetAll();

            // Assert
            Assert.AreEqual(filteringOperators.Count(o => o.Equals(FilteringOperator.Equal)), 1);
            Assert.AreEqual(filteringOperators.Count(o => o.Equals(FilteringOperator.NotEqual)), 1);
            Assert.AreEqual(filteringOperators.Count(o => o.Equals(FilteringOperator.GreaterThan)), 1);
            Assert.AreEqual(filteringOperators.Count(o => o.Equals(FilteringOperator.GreaterThanOrEqual)), 1);
            Assert.AreEqual(filteringOperators.Count(o => o.Equals(FilteringOperator.LessThan)), 1);
            Assert.AreEqual(filteringOperators.Count(o => o.Equals(FilteringOperator.LessThanOrEqual)), 1);
            Assert.AreEqual(filteringOperators.Count(o => o.Equals(FilteringOperator.Contains)), 1);
            Assert.AreEqual(filteringOperators.Count(o => o.Equals(FilteringOperator.StartsWith)), 1);
        }

        [TestMethod]
        public void Should_Create_Correct_Operator_From_Value()
        {
            // Arrange
            var operatorValue = 0;
            var expectedFilteringOperator = FilteringOperator.Equal;

            // Act
            var filteringOperator = FilteringOperator.FromValue(operatorValue);

            // Assert
            Assert.IsTrue(filteringOperator.Equals(expectedFilteringOperator));
        }

        [TestMethod]
        public void Should_Create_Correct_Operator_From_DisplayName()
        {
            // Arrange
            var operatorDisplayName = ">=";
            var expectedFilteringOperator = FilteringOperator.GreaterThanOrEqual;

            // Act
            var filteringOperator = FilteringOperator.FromDisplayName(operatorDisplayName);

            // Assert
            Assert.IsTrue(filteringOperator.Equals(expectedFilteringOperator));
        }
    }
}
