using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleGameAccounts;

namespace SimpleGameAccountsTests
{
    [TestClass]
    public class PlayerAccountTests
    {
        [TestInitialize]
        public void Setup()
        {
            if (Directory.Exists("SimpleGameAccounts"))
                Directory.Delete("SimpleGameAccounts", true);
        }

        [TestMethod]
        public void RandomAccountNumbersAreCorrect()
        {
            Random r = new Random(0);

            // With random stuff, it's best to try as many times as you can bear to wait.
            // On my computer, 10k tries takes ~15ms, which also suggests good perf. Cool.
            for (int i = 0; i < 10000; ++i)
            {
                var dut = PlayerAccount.RandomAccountId(r);
                var dutString = dut.ToString();

                // Account numbers should always be 9 digits, no leading zeros
                Assert.AreEqual(9, dutString.Length);
                Assert.AreNotEqual('0', dutString[0]);
            }
        }

        [TestMethod]
        public void CreatesANewPlayer()
        {
            using (LmdbPlayerDatabase db = new LmdbPlayerDatabase())
            {
                var output1 = PlayerAccount.CreateAccount(0, db);
                Assert.IsNotNull(output1);

                var output2 = PlayerAccount.CreateAccount(0, db);
                Assert.IsNotNull(output2);

                Assert.AreNotEqual(output1.AccountId, output2.AccountId);
            }
        }

        [TestMethod]
        public void FindsAnOldPlayer()
        {
            using (LmdbPlayerDatabase db = new LmdbPlayerDatabase())
            {
                // Shouldn't be able to find this player yet.
                var output0 = PlayerAccount.LoginPlayer(0, new byte[] { 1, 2, 3, 4 }, db);
                Assert.IsNull(output0);

                var output1 = PlayerAccount.CreateAccount(0, db);
                Assert.IsNotNull(output1);

                var output2 = PlayerAccount.LoginPlayer(output1.AccountId, output1.AccountPassword, db);

                Assert.AreEqual(output1.AccountId, output2.AccountId);
                Assert.AreEqual(output1.AccountPassword, output2.AccountPassword);
            }
        }
    }
}
