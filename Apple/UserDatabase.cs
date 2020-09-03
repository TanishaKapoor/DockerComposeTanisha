using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService
{
    public static class UserDatabase
    {
        public static List<User> userDb = new List<User>
        {
            new User{id=1,name="tanisha",age=24,email="tanisha@gmial.com"},
            new User{id=2,name="mayank",age=23,email="mayank@gmail.com"}
        };
    }
}
