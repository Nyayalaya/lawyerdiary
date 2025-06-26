using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.Lawyer;
using CourtApp.Application.Extensions;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.LawyerDiary;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Lawyer
{
    public class LawyerGetAllQuery : IRequest<PaginatedResult<LawyerResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<string> LinkedIds { get; set; }
        public string Role { get; set; }
    }
    public class LawyerGetAllQueryHandler : IRequestHandler<LawyerGetAllQuery, PaginatedResult<LawyerResponse>>
    {
        private readonly ILawyerRepository _repository;
        public LawyerGetAllQueryHandler(ILawyerRepository _repository)
        {
            this._repository = _repository;
        }
        public async Task<PaginatedResult<LawyerResponse>> Handle(LawyerGetAllQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<LawyerMasterEntity, LawyerResponse>> expression = e => new LawyerResponse
            {
                Id = e.Id,
                Email = e.Email,
                EnrollNumber = e.EnrollNumber,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Dob = e.Dob,
                Address = e.Address,
                Mobile = e.Mobile,
                ProfileImgPath = e.ProfileImgPath
            };

            var query = _repository.Entities.AsQueryable();

            // Apply filtering based on role
            if (request.Role != "SuperAdmin")
            {
                query = query.Where(r => request.LinkedIds.Contains(r.CreatedBy));
            }

            // Fetch paginated result with projection
            var paginatedList = await query
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);

            // Set total count using the filtered query
            paginatedList.TotalCount = await query.CountAsync();

            return paginatedList;

            //Expression<Func<LawyerMasterEntity, LawyerResponse>> expression = e => new LawyerResponse
            //{
            //    Id = e.Id,
            //    Email = e.Email,
            //    EnrollNumber = e.EnrollNumber,
            //    FirstName = e.FirstName,
            //    LastName = e.LastName,
            //    Dob = e.Dob,
            //    Address = e.Address,
            //    Mobile = e.Mobile,
            //    ProfileImgPath = e.ProfileImgPath
            //};            
            //var query = _repository.Entities.AsQueryable();

            //// Apply the condition for "SuperAdmin"
            //if (request.Role != "SuperAdmin")            
            //    query = query.Where(r =>request.LinkedIds.Contains(r.CreatedBy));


            //var paginatedList = await query
            //    .Select(expression)
            //    .ToPaginatedListAsync(request.PageNumber, request.PageSize);

            //// TotalCount should respect the role condition as well
            //if (request.Role != "SuperAdmin")            
            //    paginatedList.TotalCount = _repository
            //        .Entities
            //        .Count(r =>request.LinkedIds.Contains(r.Id.ToString()));            
            //else            
            //    paginatedList.TotalCount = _repository.Entities.Count();           

            //return paginatedList;
        }
    }
}
