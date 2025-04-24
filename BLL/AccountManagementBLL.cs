using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class AccountManagementBLL
    {
        QLPhongTroDataContext db;

        public AccountManagementBLL()
        {
            db = new QLPhongTroDataContext();
        }

        public List<EmployeeAccount> GetListAccount()
        {
            try
            {
                return (from ac in db.EmployeeAccounts select ac).ToList();
            }
            catch (Exception)
            {
                return new List<EmployeeAccount>();
            }
        }

        public List<string> GetListEmployee()
        {
            try
            {
                var lst = from em in db.Employees select $"{em.EmployeeID.Trim()} - {em.LastName.Trim()} {em.FirstName.Trim()}";
                return lst.ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public bool CheckEmployeeAccountExists(string employeeId)
        {
            try
            {
                bool accountExists = db.EmployeeAccounts.Any(ac => ac.EmployeeID.Trim() == employeeId);
                return accountExists;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool CheckUsernameExists(string username)
        {
            try
            {
                bool usernameExists = db.EmployeeAccounts.Any(ac => ac.Username.Trim() == username.Trim());
                return usernameExists;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool InsertData(string idEmp, string user, string pass, int status, string role)
        {
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(pass);

                var newAcc = new EmployeeAccount()
                {
                    EmployeeID = idEmp,
                    Username = user,
                    PasswordHash = hashedPassword,
                    Status = status,
                    Role = role,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                db.EmployeeAccounts.InsertOnSubmit(newAcc);
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ChangePassword(int id, string password)
        {
            try
            {
                var acc = from ac in db.EmployeeAccounts where ac.AccountID == id select ac;
                if (acc != null)
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                    acc.FirstOrDefault().PasswordHash = hashedPassword;

                    db.SubmitChanges();
                    return true;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ChangeStatus(int id, int status)
        {
            try
            {
                var acc = from ac in db.EmployeeAccounts where ac.AccountID == id select ac;
                if (acc != null)
                {
                    acc.FirstOrDefault().Status = status;

                    db.SubmitChanges();
                    return true;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ChangeRole(int id, string role)
        {
            try
            {
                var acc = from ac in db.EmployeeAccounts where ac.AccountID == id select ac;
                if (acc != null)
                {
                    acc.FirstOrDefault().Role = role;

                    db.SubmitChanges();
                    return true;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
