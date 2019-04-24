using System;

namespace SimpleGameAccounts
{
    // The interesting thing about this class is how useless it is.
    // You should only need this class if you are:
    // * Creating a new account. (So you can store the information back on the client.)
    // * Logging in a new connection. (Afterwards, the server should keep track of the account id)
    // * Recovering an account (Same as logging in, but maybe leave a different audit trail.)
    //
    // Ultimately, you shouldn't be holding references to this class. Just keep the AccountId instead.
    //
    // In fact, you might even refactor this class to only return bool for LoginPlayer. 
    // I can't see a reason why you'd want a password there at all.
    public class PlayerAccount
    {
        public readonly int AccountId;

        // Let's be honest. For security purposes, you shouldn't be holding passwords in memory.
        // It might make account creation/recovery slower, but it's best to only keep this data around
        // for as long as it's relevant. One could imagine a hacker getting a process memory dump and 
        // scraping unhashed passwords from it.
        public readonly byte[] AccountPassword;

        internal PlayerAccount(int id, byte[] password)
        {
            this.AccountId = id;
            this.AccountPassword = password;
        }

        // This is a 9 digit number with no chance of leading zeros.
        // This produces 900,000,000 different accounts. Perf will suck when you 
        // start to get statistically close, but you can fix it after 1M downloads. :)
        internal static int RandomAccountId(Random r)
        {
            int output = r.Next(1, 10);
            for (int i = 0; i < 8; ++i)
            {
                output = r.Next(10) + output * 10;
            }

            return output;
        }

        /// <summary>
        /// Creates a new, unique player account
        /// </summary>
        /// <param name="seed">A number to help seed uniqueness. A good idea for this is 
        /// the order a player connected to the server in plus the current time in ticks.</param>
        public static PlayerAccount CreateAccount(int seed, IPlayerDatabase db)
        {
            Random r = new Random(seed);

            // If each byte is "one character", then a 20 digit password is rather strong.
            // You may want to use Base32 or similar to make it prettier for players.
            // These bytes are to be sent down to the player so they can recover their account. 
            // It should be stored locally and securely for them so they don't log in each time.
            byte[] password = new byte[20];
            r.NextBytes(password);

            // These bytes should be salted and hashed for storage. I'm not a crypto-expert, so 
            // I'm not going to try it and invariably add bad code that will need upkeep.
            // But since the premise of this project is to not store personal information,
            // then there's really nothing to lose.
            // Your only real issue is if you care about cheating/hacking. Which you probably still should.
            byte[] securePassword = new byte[21];

            // Salted with a zero at the end. NICE. That'll keep 'em off the trail.
            Array.Copy(password, securePassword, password.Length);

            int accountId;
            do
            {
                accountId = RandomAccountId(r);
            }
            while (!db.CreateNewPlayer(accountId, securePassword));

            return new PlayerAccount(accountId, password);
        }

        /// <summary>
        /// Returns an existing player account with matching information. Or null if it doesn't exist.
        /// </summary>
        public static PlayerAccount LoginPlayer(int accountId, byte[] password, IPlayerDatabase db)
        {
            // Remember how the password was hashed and salted above. You gotta do it again so it'll match the database.
            byte[] securePassword = new byte[21];
            Array.Copy(password, securePassword, password.Length);

            if (db.FindOldPlayer(accountId, securePassword))
            {
                return new PlayerAccount(accountId, password);
            }

            return null;
        }
    }
}
