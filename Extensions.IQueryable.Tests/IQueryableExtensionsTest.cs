using Extensions.IQueryable.Pagination;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Extensions.IQueryable.Tests
{
    [TestClass]
    public class IQueryableExtensionsTest
    {
        [TestMethod]
        public void Should_Corectly_Paginate_ProvidedData()
        {
            // Arrange
            var data = new string[]
            {
                "a", "b", "c", "d", "e", "f", "g"
            };

            // Act
            var paginationResult = data.AsQueryable().Paginated(new PaginationInfo(2, 2));

            // Assert
            Assert.AreEqual(paginationResult.TotalRecords, data.Count());
            Assert.AreEqual(paginationResult.CurrentPage, 2);
            Assert.AreEqual(paginationResult.PageSize, 2);
            Assert.IsTrue(paginationResult.Data.SequenceEqual(new string[] { "c", "d" }));
        }
    }
}
