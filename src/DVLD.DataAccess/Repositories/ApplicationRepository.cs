using DVLD.Contracts.Common;
using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Application;
using DVLD.Contracts.Requests.Application;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using DVLD.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly AppDbContext _context;

        public ApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationDto> AddAsync(CreateApplicationRequest dto, DateTime CreatedAt, CancellationToken cancellationToken )
        {
            var entity = new Application
            {
                ApplicantPersonId = dto.ApplicantPersonId,
                ApplicationDate = CreatedAt,
                ApplicationTypeId = dto.ApplicationTypeId,
                ApplicationStatus = (enApplicationStatus)dto.ApplicationStatus,
                LastStatusDate = CreatedAt,
                PaidFees = dto.PaidFees,
                CreatedByUserId = dto.CreatedByUserId
            };
           
            await _context.Applications.AddAsync(entity,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToDto();
        }

        public async Task DeleteAsync(int applicationId, CancellationToken cancellationToken)
        {
            await _context.Applications
                .Where(a => a.ApplicationId == applicationId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(int applicationId, CancellationToken cancellationToken)
        {
            return await _context.Applications.AnyAsync(a => a.ApplicationId == applicationId, cancellationToken);
        }

        public async Task<ApplicationDto?> GetActiveAsync(int personId, int typeId, CancellationToken cancellationToken)
            => await _context.Applications
                            .AsNoTracking()
                            .Where
                               (
                                 a => a.ApplicantPersonId == personId
                                  && a.ApplicationTypeId == typeId
                                  && (byte)a.ApplicationStatus == (byte)enApplicationStatus.New     // adjust status as needed
                               )
                            .ProjectToDto()
                            .FirstOrDefaultAsync(cancellationToken);                    

        public async Task<ApplicationDto?> GetActiveForLicenseClassAsync(int personId, int typeId, int licenseClassId, CancellationToken cancellationToken )            
             => await _context.Applications
                .AsNoTracking()
                .Where(a => a.ApplicantPersonId == personId
                         && a.ApplicationTypeId == typeId
                         && a.ApplicationStatus == enApplicationStatus.New
                         && a.LocalDrivingLicenseApplication.LicenseClassId == licenseClassId)
                
                .ProjectToDto()
                .FirstOrDefaultAsync(cancellationToken);                    

        public async Task<IReadOnlyList<ApplicationDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Applications
                .AsNoTracking()
                .ProjectToDto()
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<ApplicationDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken)
            => await _context.Applications
                .AsNoTracking()
                .ProjectToDto()
                .OrderBy(a => a.ApplicationId)
                .ToPagedListAsync(paging, cancellationToken);

        public async Task<ApplicationDto?> GetByIdAsync(int applicationId, CancellationToken cancellationToken)
            => await _context.Applications
                            .AsNoTracking()
                            .Where(a => a.ApplicationId == applicationId)
                            .ProjectToDto()
                            .FirstOrDefaultAsync(cancellationToken);
            
        public async Task<ApplicationDto> UpdateAsync(int applicationId, UpdateApplicationRequest dto, CancellationToken cancellationToken)
        {
            var entity = await _context.Applications.FindAsync(applicationId , cancellationToken)
                ?? throw new KeyNotFoundException($"Application with id {applicationId} not found");

            entity.ApplicantPersonId = dto.ApplicantPersonId;
            entity.ApplicationDate = dto.ApplicationDate;
            entity.ApplicationTypeId = dto.ApplicationTypeId; 
            entity.ApplicationStatus = (enApplicationStatus)dto.ApplicationStatus;
            entity.LastStatusDate = dto.LastStatusDate;
            entity.PaidFees = dto.PaidFees;
            entity.CreatedByUserId = dto.CreatedByUserId;

            await _context.SaveChangesAsync(cancellationToken);
            return entity.ToDto();
        }

        public async Task<bool> UpdateStatusAsync(int applicationId, short newStatus, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var affected = await _context.Applications
                 .Where(a => a.ApplicationId == applicationId)
                 .ExecuteUpdateAsync(s => s
                    .SetProperty(a => a.ApplicationStatus, (enApplicationStatus)(byte)newStatus)
                    .SetProperty(a => a.LastStatusDate, now), cancellationToken);

            return affected > 0;
        }

        
    }



}
