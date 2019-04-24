using LightningDB;
using System;

namespace SimpleGameAccounts
{
    // Swap this out with whatever database you like. I like the inherent speed, 
    // reliability and simplicity of LMDB, but a SQL flavor is great too if you want features
    // like back ups or multi-user security.
    internal class LmdbPlayerDatabase : IDisposable, IPlayerDatabase
    {
        private LightningEnvironment dbEnv;

        public LmdbPlayerDatabase()
        {
            // You should probably separate additional data into entirely different LMDB environments.
            // One Database is kind of like "one table" in a relational database.
            // For example, ff you wanted to add coins, you would probably want a 
            // new CoinDatabase with one Database for each type of coin, then
            // key -> values of AccountId -> CoinQuantity
            EnvironmentConfiguration config = new EnvironmentConfiguration();
            config.MaxDatabases = 1;

            this.dbEnv = new LightningEnvironment("SimpleGameAccounts", config);
            this.dbEnv.Open();

            using (var trans = this.dbEnv.BeginTransaction())
            using (var db = trans.OpenDatabase("Accounts", new DatabaseConfiguration { Flags = DatabaseOpenFlags.Create }))
            {
                trans.Commit();
            }
        }

        public void Dispose()
        {
            if (this.dbEnv != null)
            {
                this.dbEnv.Dispose();
                this.dbEnv = null;
            }
        }

        /// <summary>
        /// Attempts to create a new player account. Fails if account id already exists.
        /// </summary>
        /// <param name="securePassword">This value should already be secured by the time it gets here. Salted, hashed, etc.</param>
        /// <returns>True if player was successfully created.</returns>
        public bool CreateNewPlayer(int accountId, byte[] securePassword)
        {
            byte[] accountKey = BitConverter.GetBytes(accountId);

            using (var trans = this.dbEnv.BeginTransaction())
            using (var db = trans.OpenDatabase("Accounts"))
            {
                if (trans.ContainsKey(db, accountKey)) return false;

                trans.Put(db, accountKey, securePassword, PutOptions.NoDuplicateData);
                trans.Commit();
                
            }

            using (var trans = this.dbEnv.BeginTransaction(TransactionBeginFlags.ReadOnly))
            using (var db = trans.OpenDatabase("Accounts"))
            {
                var verify = trans.Get(db, accountKey);

                return verify.SequenceEquals(securePassword);
            }
        }

        /// <summary>
        /// Attempts to match an existing player account id and password.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="securePassword">This value should already be secured by the time it gets here. Salted, hashed, etc.</param>
        /// <returns>True if the account information matches a database entry.</returns>
        public bool FindOldPlayer(int accountId, byte[] securePassword)
        {
            byte[] accountKey = BitConverter.GetBytes(accountId);

            using (var trans = this.dbEnv.BeginTransaction(TransactionBeginFlags.ReadOnly))
            using (var db = trans.OpenDatabase("Accounts"))
            {
                var verify = trans.Get(db, accountKey);

                return verify.SequenceEquals(securePassword);
            }
        }
    }
}