using MovieStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieStore.Domain.Abstract {
    public interface IOrderRepository {
        IEnumerable<Order> Orders { get; }
        void SaveOrder(Order order);
    }
}
