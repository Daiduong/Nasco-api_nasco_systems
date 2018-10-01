using Microsoft.EntityFrameworkCore;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class MAWBRepository : Repository<MAWB>, IMAWBRepository
    {
        public MAWBRepository(ApplicationDbContext conText) : base(conText)
        {

        }
        public async Task<MAWB> GetById(int id)
        {
            return await this.GetSingleAsync(x => x.Id == id);
        }
        public async Task<IEnumerable<MAWBModel>> GetListMAWBNew(int poId)
        {
            return (await this.GetAsync(x => (x.PoFrom ?? 0) == poId && (x.IsNew ?? true))).Select(x => new MAWBModel(x));
        }
        public async Task<bool> IsExistedCode(string code, int? id = null)
        {
            return await _context.MAWBs.AnyAsync(o => (!id.HasValue || id.Value != o.Id) && o.MAWBCode != null && o.MAWBCode.ToUpper().Equals(code.ToUpper()));
        }
        public async Task<ResultModel<MAWB>> AddOrEdit(MAWBModel model)
        {
            var result = new ResultModel<MAWB>();
            try
            {
                var edit = true;
                var obj = new MAWB();
                var officer = _context.Set<Officer>().Include(o => o.PostOffice).Single(o => o.OfficerID == model.ModifiedBy);
                var currentPOId = officer.PostOfficeId;
                var currentPO = officer.PostOffice;
                var currentUserId = officer.OfficerID;
                var now = DateTime.Now;
                if (string.IsNullOrEmpty(model.MAWBCode))
                {
                    return result.Init(message: "Vui lòng nhập mã AWB");
                }
                if (await this.IsExistedCode(model.MAWBCode, model.Id))
                {
                    return result.Init(message: "Mã MAWB đã tồn tại");
                }
                if (string.IsNullOrEmpty(model.FlightNumber))
                {
                    return result.Init(message: "Vui lòng nhập số hiệu chuyến bay");
                }
                if (model.PoTo == 0)
                {
                    return result.Init(message: "Không tìm thấy trạm đến");
                }
                if (model.AirlineId == 0)
                {
                    return result.Init(message: "Không tìm thấy hãng bay");
                }
                if (!model.ExpectedDateTime.HasValue)
                {
                    return result.Init(message: "Vui lòng chọn thời gian dự kiến");
                }
                if ((model.MaxWeight ?? 0) <= 0)
                {
                    return result.Init(message: "Vui lòng nhập trọng lượng book");
                }
                if (model.Id > 0)
                {
                    obj = await this.GetById(model.Id);
                    if (obj == null)
                    {
                        return result.Init(message: "Không tìm thấy thông tin lịch bay");
                    }
                    if (obj.RealDateTime.HasValue)
                    {
                        return result.Init(message: "Không được phép chỉnh sửa do chuyến bay này đã khởi hành.");
                    }
                    if (string.IsNullOrEmpty(model.ReasonContent))
                    {
                        return result.Init(message: "Vui lòng nhập lý do điều chỉnh.");
                    }
                }
                else
                {
                    obj.CreatedDate = now;
                    obj.PoFrom = currentPOId;
                    obj.CreatedBy = currentUserId;
                    edit = false;
                }
                obj.ModifiedDate = now;
                obj.ModifiedBy = currentUserId;
                obj.ExpectedDateTime = model.ExpectedDateTime;
                obj.FlightNumber = model.FlightNumber;
                obj.MaxWeight = model.MaxWeight;
                obj.AirlineId = model.AirlineId;
                obj.IsNew = obj.IsNew ?? true;
                obj.Note = model.Note;
                obj.ReasonContent = model.ReasonContent;
                if (edit && (obj.IsNew ?? true) == false)
                {
                }
                else
                {
                    obj.PoTo = model.PoTo;
                    obj.MAWBCode = model.MAWBCode;
                }
                if (!edit)
                    this.Insert(obj);
                else
                    this.Update(obj);
                this.SaveChanges();
                result.Error = 0;
                result.Message = edit ? "Cập nhật lịch bay thành công" : "Thêm lịch bay thành công";
                result.Data = obj;
            }
            catch (Exception ex)
            {
                result.Message= ex.Message;
            }
            return result;
        }
    }

}
