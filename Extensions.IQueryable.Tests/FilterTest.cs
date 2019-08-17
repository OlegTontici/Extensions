using Extensions.IQueryable.Filtering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Extensions.IQueryable.Tests
{
    [TestClass]
    public class FilterTest
    {
        [TestMethod]
        public void Throw_ArgumentException_Once_Initialized_With_Empty_PropertyName()
        {
            // Arrange
            ArgumentException expectedException = null;

            // Act
            try
            {
                new Filter(string.Empty, FilteringOperator.Equal, "value");
            }
            catch (ArgumentException ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
            Assert.AreEqual(expectedException.ParamName, "propertyName");
        }

        [TestMethod]
        public void Throw_ArgumentException_Once_Initialized_With_Null_PropertyName()
        {
            // Arrange
            ArgumentException expectedException = null;

            // Act
            try
            {
                new Filter(null, FilteringOperator.Equal, "value");
            }
            catch (ArgumentException ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
            Assert.AreEqual(expectedException.ParamName, "propertyName");
        }

        [TestMethod]
        public void Throw_ArgumentNullException_Once_Initialized_With_Null_FilteringOperator()
        {
            // Arrange
            ArgumentNullException expectedException = null;

            try
            {
                new Filter("propName", null, "value");
            }
            catch (ArgumentNullException ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
            Assert.AreEqual(expectedException.ParamName, "filteringOperator");
        }

        [TestMethod]
        public void Throw_ArgumentNullException_Once_Initialized_With_Null_SearchValue()
        {
            // Arrange
            ArgumentNullException expectedException = null;

            // Act
            try
            {
                new Filter("propName", FilteringOperator.Equal, null);
            }
            catch (ArgumentNullException ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
            Assert.AreEqual(expectedException.ParamName, "searchValue");
        }
    }
}
