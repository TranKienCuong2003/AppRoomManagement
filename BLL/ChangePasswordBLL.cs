using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ChangePasswordBLL
    {
        QLPhongTroDataContext db;

        public ChangePasswordBLL()
        {
            db = new QLPhongTroDataContext();
        }

        public string GetPassword(string id)
        {
            try
            {
                string hashedPassword = (from ec in db.EmployeeAccounts
                                         where ec.EmployeeID.Trim() == id
                                         select ec.PasswordHash).FirstOrDefault();
                return hashedPassword;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public bool UpdatePasswordById(string id, string pass)
        {
            try
            {
                var acc = from ac in db.EmployeeAccounts where ac.EmployeeID.Trim().Equals(id) select ac;
                if (acc != null )
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(pass);
                    acc.FirstOrDefault().PasswordHash = hashedPassword;
                    db.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
