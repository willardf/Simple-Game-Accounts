using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleGameAccounts;

namespace SimpleGameAccountsTests
{
    [TestClass]
    public class LmdbPlayerDatabaseTests
    {
        [TestInitialize]
        public void Setup()
        {
            if (Directory.Exists("SimpleGameAccounts"))
                Directory.Delete("SimpleGameAccounts", true);
        }

        [TestMethod]
        public void CreatesANewPlayer()
        {
            using (LmdbPlayerDatabase dut = new LmdbPlayerDatabase())
            {
                // Can create player once
                Assert.IsTrue(dut.CreateNewPlayer(0, new byte[] { 1, 2, 3, 4 }));

                // Can't recreate player with same password
                Assert.IsFalse(dut.CreateNewPlayer(0, new byte[] { 1, 2, 3, 4 }));

                // Can recreate with a different password
                Assert.IsFalse(dut.CreateNewPlayer(0, new byte[] { 2, 3, 4, 5 }));
            }
        }

        [TestMethod]
        public void FindsAnOldPlayer()
        {
            using (LmdbPlayerDatabase dut = new LmdbPlayerDatabase())
            {
                // Shouldn't be able to find this player yet.
                Assert.IsFalse(dut.FindOldPlayer(0, new byte[] { 1, 2, 3, 4 }));

                // Create the player
                Assert.IsTrue(dut.CreateNewPlayer(0, new byte[] { 1, 2, 3, 4 }));

                // Can find player with correct password
                Assert.IsTrue(dut.FindOldPlayer(0, new byte[] { 1, 2, 3, 4 }));

                // Can't find player with wrong password
                Assert.IsFalse(dut.FindOldPlayer(0, new byte[] { 2, 3, 4, 5 }));
            }
        }
    }
}
