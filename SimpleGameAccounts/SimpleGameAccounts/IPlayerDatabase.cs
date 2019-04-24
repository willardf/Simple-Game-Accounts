using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGameAccounts
{
    public interface IPlayerDatabase
    {
        bool CreateNewPlayer(int accountId, byte[] accountPassword);
        bool FindOldPlayer(int accountId, byte[] securePassword);
    }
}
