namespace ForCode.DapperPost.Tests
{
    using ForCode.DapperPost;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Data;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.SQLite;
    using System.IO;

    [TestClass]
    public class DapperWrapperTests
    {
        private const string DdName = "Test.sqlite";
        private const string ConnectionString = "Data Source=Test.sqlite;Version=3;";

        private IDbConnection Connection { get; set; }

        [TestInitialize]
        public void Setup()
        {
            SQLiteConnection.CreateFile(DdName);
            this.Connection = new SQLiteConnection(ConnectionString);
            this.Connection.Open();
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE [DummyEntity] (Id INT, ParentFkyId Id)";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO [DummyEntity] VALUES ('1', '1')";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO [DummyEntity] VALUES ('2', '2')";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO [DummyEntity] VALUES ('3', '1')";
                command.ExecuteNonQuery();
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            this.Connection.Close();
            this.Connection.Dispose();
            if (File.Exists(DdName))
            {
                File.Delete(DdName);
            }
        }

        [TestMethod]
        public void Should_ExtendIConnectionAsDapperDoes()
        {
            IDbConnection connection = this.Connection;            
            var collection = connection.Query<DummyEntity>();
            Assert.IsTrue(collection.Any());
        }

        [TestMethod]
        public void Should_ReturnExpectedItemCount()
        {
            IDbConnection connection = this.Connection;
            var collection = connection.Query<DummyEntity>();
            Assert.AreEqual(3, collection.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(NotQueryableTypeException))]
        public void Should_ThrowOnQueryingForNotValidType()
        {
            IDbConnection connection = this.Connection;
            var collection = connection.Query<DapperWrapperTests>();
            Assert.Fail();
        }

        [Table("DummyEntity")]
        private class DummyEntity
        {
            [Column]
            public int Id { get; set; }

            [Column]
            public int ParentFkyId { get; set; }
        }
    }
}
