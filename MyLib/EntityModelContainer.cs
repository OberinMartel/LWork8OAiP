using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace MyLib
{
    public partial class EntityModelContainer : DbContext
    {
        public EntityModelContainer() : base("name=KassirRuDB")
        { }
        public virtual DbSet<User> UserSet { get; set; }
        public virtual DbSet<TicketPack> TicketPackSet { get; set; }
        public virtual DbSet<Contract> ContractSet { get; set; }
        public TicketPack FindTicketPackWithID(int ID)
        {
            foreach (TicketPack ticketPack in TicketPackSet)
            {
                if (ticketPack.Id == ID) return ticketPack;
            }
            return null;
        }
        public TicketPack FindTicketPackWithName(string Name)
        {
            foreach (TicketPack ticketPack in TicketPackSet)
            {
                if (ticketPack.Name == Name) return ticketPack;
            }
            return null;
        }
        public User FindUserWithEMail(string EMail)
        {
            foreach (User user in UserSet)
            {
                if (user.Email == EMail) return user;
            }
            return null;
        }
        public User FindUserWithID(int ID)
        {
            foreach (User user in UserSet)
            {
                if (user.Id == ID) return user;
            }
            return null;
        }
        public Contract FindContractWithID(int ID)
        {
            foreach (Contract contract in ContractSet)
            {
                if (contract.Id == ID) return contract;
            }
            return null;
        }
    }

}
