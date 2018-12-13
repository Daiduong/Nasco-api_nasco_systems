using Microsoft.Extensions.Logging;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class PackageOfLadingRepository : Repository<PackageOfLading>, IPackageOfLadingRepository
    {
        public PackageOfLadingRepository(ApplicationDbContext context) : base(context)
        {

        }
        public async Task Insert(List<NumberDIM> model, int ladingId, int currentUserId, int currentPOId)
        {

            var lading = _context.Ladings.FirstOrDefault(o => o.Id == ladingId && o.State == 0);
            if (lading != null)
            {
                for (int i = 0, orderNumber = 0; i < model.Count; i++)
                {
                    for (int j = 0; j < model[i].Number; j++)
                    {
                        orderNumber++;
                         var packageOfLading = new PackageOfLading()
                        {
                            Code = $"{ lading.Code }.{orderNumber}",
                            CreatedBy = currentUserId,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = currentUserId,
                            ModifiedDate = DateTime.Now,
                            POCurrentId = currentPOId,
                            LadingId = ladingId,
                            State = 0,
                            Height = model[i].Height,
                            Long = model[i].Long,
                            Width = model[i].Width,
                            Mass = model[i].DIM / model[i].Number,
                            Weight = (lading.Weight ?? 0) / (lading.Number ?? 0),
                            StatusId = lading.Status,
                            Order = orderNumber,
                            TotalNumber = 1
                        };
                        _context.PackageOfLadings.Add(packageOfLading);
                    }
                }
                await _context.SaveChangesAsync();
            }

        }
        public async Task Update(int currentUserId, int currentPOId, int packageOfLadingId, int statusId, int? packageId = null, int? bkInternalId = null, int? bkDeliveryId = null, string note = "")
        {
            var packageOfLading = _context.PackageOfLadings.FirstOrDefault(o => o.Id == packageOfLadingId && o.State == 0);
            packageOfLading.StatusId = statusId;
            packageOfLading.POCurrentId = currentPOId;
            packageOfLading.ModifiedBy = currentUserId;
            packageOfLading.ModifiedDate = DateTime.Now;
            packageOfLading.BKDeliveryId = bkDeliveryId;
            packageOfLading.BKInternalId = bkInternalId;
            packageOfLading.PackageId = packageId;
            _context.SaveChanges();
            await this.InsertHistory(currentUserId, currentPOId, packageOfLadingId, statusId, note);
        }
        public async Task Update(int currentUserId, int currentPOId, PackageOfLading packageOfLading, string note = "")
        {
            packageOfLading.POCurrentId = currentPOId;
            packageOfLading.ModifiedBy = currentUserId;
            packageOfLading.ModifiedDate = DateTime.Now;
            this.Update(packageOfLading);
            _context.SaveChanges();
            await this.InsertHistory(currentUserId, currentPOId, packageOfLading.Id, packageOfLading.StatusId, note);
        }
        public async Task UpdateIsPartStatusLading(int ladingId)
        {
            var lading = _context.Ladings.FirstOrDefault(o => o.Id == ladingId && o.State == 0);
            if (lading != null)
            {
                var isPart = false;
                if (this.Any(o => o.LadingId == ladingId && o.State == 0))
                {
                    isPart = this.Any(o => o.LadingId == ladingId && o.StatusId != lading.Status && o.State == 0);
                }
                lading.IsPartStatus = isPart;
                await _context.SaveChangesAsync();
            }
        }
        public async Task InsertHistory(int currentUserId, int currentPOId, int packageOfLadingId, int? statusId, string note = "")
        {
            var packageOfLadingHistory = new PackageOfLadingHistory()
            {
                CreatedBy = currentUserId,
                CreatedDate = DateTime.Now,
                ModifiedBy = currentUserId,
                ModifiedDate = DateTime.Now,
                POCreated = currentPOId,
                PackageOfLadingId = packageOfLadingId,
                State = 0,
                Note = note,
                StatusId = statusId
            };
            _context.PackageOfLadingHistories.Add(packageOfLadingHistory);
            await _context.SaveChangesAsync();
        }
    }
}
