using MovieStore.Domain.Abstract;
using MovieStore.Domain.Entities;
using System.Collections.Generic;

namespace MovieStore.Domain.Concrete {
    public class EFOrderRepository : IOrderRepository {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Order> Orders {
            get { return context.Orders; }
        }

        public void SaveOrder(Order order) {
            if (order.OrderID == 0) {
                context.Orders.Add(order);
            } else {
                Order dbEntry =
                context.Orders.Find(order.OrderID);
                if (dbEntry != null) {
                    dbEntry.MovieID = order.MovieID;
                    dbEntry.Name = order.Name;
                    dbEntry.Price = order.Price;
                    dbEntry.Category = order.Category;
                    dbEntry.Quantity = order.Quantity;
                }
            }
            context.SaveChanges();
        }
    }
}
