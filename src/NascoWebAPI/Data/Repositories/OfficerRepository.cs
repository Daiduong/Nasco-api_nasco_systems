using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class OfficerRepository : Repository<Officer> , IOfficerRepository
    {
        public OfficerRepository(ApplicationDbContext conText) : base(conText)
        {

        }
        public async  Task<int?> GetPOIdByOffice(int officeId)
        {
           var office = await  this.GetFirstAsync(o => o.OfficerID == officeId && o.PostOfficeId.HasValue);
            if (office != null)
            {
                //var department = _context.Deparments.FirstOrDefault(o => o.DeparmentID == office.DeparmentID);
                //if (department != null) return department.PostOfficeID;
                return office.PostOfficeId;
            }
            return null;
        }
        public IEnumerable<Menu> GetListMenuByOfficer(int officerId)
        {
            ICollection<Menu> listMenu = new List<Menu>();
            Menu menu1 = new Menu();
            menu1.Id = 1;
            menu1.MenuName = "Yêu cầu lấy hàng";
            menu1.Badges = _context.LadingTemps.Count(o => o.OfficerPickup.Value == officerId && o.Status.Value == (int)StatusLadingTemp.WaitingPickUp);
            listMenu.Add(menu1);

            Menu menu2 = new Menu();
            menu2.Id = 2;
            menu2.MenuName = "Lấy hàng";
            menu2.Badges = _context.LadingTemps.Count(o => o.OfficerPickup.Value == officerId && o.Status.Value == (int)StatusLadingTemp.PickingUp);
            listMenu.Add(menu2);

            Menu menu3 = new Menu();
            menu3.Id = 3;
            menu3.MenuName = "Chờ giao hàng";
            menu3.Badges = _context.Ladings.Count(o => o.OfficerDelivery.Value == officerId && o.Status.Value == (int)StatusLading.XuatKho);
            listMenu.Add(menu3);

            Menu menu4 = new Menu();
            menu4.Id = 4;
            menu4.MenuName = "Giao hàng";
            menu4.Badges = _context.Ladings.Count(o => o.OfficerDelivery.Value == officerId && o.Status.Value == (int)StatusLading.DangPhat);
            listMenu.Add(menu4);

            Menu menu5 = new Menu();
            menu5.Id = 5;
            menu5.MenuName = "Hàng chờ trung chuyển";
            menu5.Badges = _context.BKInternals.Count(o => o.OfficerId_sender.Value == officerId && o.Status.Value == 0 && !(o.IsConfirmByOfficer?? false));
            listMenu.Add(menu5);

            return listMenu;
        }
        public bool IsDelivery(int officeId)
        {
            return BelongDepartmentJob(officeId, null , "BPGN");
        }
        public bool BelongDepartmentJob(int officeId, string jobCode, string departmentCode)
        {
            var officer = _context.Officers.FirstOrDefault(o => o.OfficerID == officeId);
            if (officer != null)
            {
                var hasJob = true;
                var hasDepartment = false;
                if (!string.IsNullOrEmpty(jobCode) && officer.JobId.HasValue)
                {
                    hasJob = _context.Jobs.Any(o => o.JobCode != null && o.JobCode.ToUpper().Equals(jobCode.ToUpper()) && o.Id == officer.JobId.Value);
                }
                if (!string.IsNullOrEmpty(departmentCode)&& officer.DeparmentID.HasValue)
                {
                    hasDepartment = _context.Deparments.Any(o => o.DeparmentCode != null && o.DeparmentCode.ToUpper().Equals(departmentCode.ToUpper()) && o.DeparmentID == officer.DeparmentID.Value);
                }
                return hasJob && hasDepartment;
            }
            return false;
            //var objs = from o in _context.Officers
            //           from d in _context.Deparments
            //           from j in _context.Jobs 
            //           where o.DeparmentID != null && d.DeparmentID == o.DeparmentID &&
            //           o.JobId != null && j.Id == o.JobId &&
            //           o.OfficerID == officeId && (jobCode == null || (j.JobCode != null && j.JobCode.ToUpper().Equals(jobCode.ToUpper())))
            //           && (departmentCode == null || (d.DeparmentCode != null && d.DeparmentCode.ToUpper().Equals(departmentCode.ToUpper())))
            //           select o.OfficerID;
            //return objs != null ? objs.Any() : false;
        }
    }
}
