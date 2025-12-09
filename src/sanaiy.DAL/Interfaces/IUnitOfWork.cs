using sanaiy.BLL.Entities;
using sanaiy.DAL.Interfaces;
using System;
using System.Threading.Tasks;

namespace sanaiy.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Client> Clients { get; }
        IGenericRepository<Craftsman> Craftsmen { get; }
        IGenericRepository<Service> Services { get; }
        IGenericRepository<Address> Addresses { get; }
        IGenericRepository<Wallet> Wallets { get; }
        IGenericRepository<Booking> Bookings { get; }
        IGenericRepository<Quote> Quotes { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<PaymentTransaction> PaymentTransactions { get; }
        IGenericRepository<Violation> Violations { get; }
        IGenericRepository<Commission> Commissions { get; }
        IGenericRepository<WalletTransaction> WalletTransactions { get; }

        // ?????? Notifications:
        IGenericRepository<Notification> Notifications { get; }


        int Complete();
        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
