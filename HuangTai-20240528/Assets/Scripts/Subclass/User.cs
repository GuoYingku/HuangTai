using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuangtaiPowerPlantControlSystem
{
    internal class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public User(string UserName, string Password)
        {
            this.UserName = UserName ?? throw new ArgumentNullException(nameof(UserName));
            this.Password = Password ?? throw new ArgumentNullException(nameof(Password));
        }

        public override string ToString() 
        {
            return "操作员："+UserName+"密码"+Password;
        }
    }
}
