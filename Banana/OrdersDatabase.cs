using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService
{
    public static class OrdersDatabase
    {
        public static List<Order> orderDb = new List<Order>
        {
            new Order{ orderId=1,customerId=1,orderAmount=1200,orderDate="14-Apr-2020"},
            new Order{ orderId=2,customerId=1,orderAmount=1300,orderDate="15-Apr-2020"},
            new Order{ orderId=3,customerId=2,orderAmount=1400,orderDate="16-Apr-2020"},
            new Order{ orderId=4,customerId=2,orderAmount=1500,orderDate="17-Apr-2020"},

        };
    }
}
