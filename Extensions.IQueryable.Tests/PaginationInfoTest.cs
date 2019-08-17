using Extensions.IQueryable.Pagination;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Extensions.IQueryable.Tests
{
    [TestClass]
    public class PaginationInfoTest
    {
        [TestMethod]
        [DataRow(0)]
        [DataRow(-4)]
        public void Throw_ArgumentOutOfRangeException_Once_Initialized_With_Negative_Or_Zero_PageSize(int pageSize)
        {
            // Arrange
            ArgumentOutOfRangeException expectedException = null;

            // Act
            try
            {
                new PaginationInfo(pageSize, 1);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
            Assert.AreEqual(expectedException.ParamName, "pageSize");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-4)]
        public void Throw_ArgumentOutOfRangeException_Once_Initialized_With_Negative_Or_Zero_CurrentPage(int currentPage)
        {
            // Arrange
            ArgumentOutOfRangeException expectedException = null;

            // Act
            try
            {
                new PaginationInfo(1, currentPage);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
            Assert.AreEqual(expectedException.ParamName, "currentPage");
        }
    }
}
