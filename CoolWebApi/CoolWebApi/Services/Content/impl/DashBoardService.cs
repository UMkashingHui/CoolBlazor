using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using CoolWebApi.Utils.Wrapper;
using CoolWebApi.Models.Responses.Content;
using CoolWebApi.Services.Identity;
using CoolWebApi.Utils.Entities.Catalog;
using CoolWebApi.Utils.Entities.Misc;
using CoolWebApi.Utils.Entities.ExtendedAttributes;
using CoolWebApi.Utils.Repositories;

namespace CoolWebApi.Services.Content.impl
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IStringLocalizer<DashBoardService> _localizer;

        public DashBoardService(IUnitOfWork<int> unitOfWork, IUserService userService, IRoleService roleService, IStringLocalizer<DashBoardService> localizer)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _roleService = roleService;
            _localizer = localizer;
        }
        public async Task<Result<DashboardDataResponse>> GetDashBoardData()
        {
            var response = new DashboardDataResponse
            {
                ProductCount = await _unitOfWork.Repository<Product>().Entities.CountAsync(),
                BrandCount = await _unitOfWork.Repository<Brand>().Entities.CountAsync(),
                DocumentCount = await _unitOfWork.Repository<Document>().Entities.CountAsync(),
                DocumentTypeCount = await _unitOfWork.Repository<DocumentType>().Entities.CountAsync(),
                DocumentExtendedAttributeCount = await _unitOfWork.Repository<DocumentExtendedAttribute>().Entities.CountAsync(),
                UserCount = await _userService.GetCountAsync(),
                RoleCount = await _roleService.GetCountAsync()
            };

            var selectedYear = DateTime.Now.Year;
            double[] productsFigure = new double[13];
            double[] brandsFigure = new double[13];
            double[] documentsFigure = new double[13];
            double[] documentTypesFigure = new double[13];
            double[] documentExtendedAttributesFigure = new double[13];
            for (int i = 1; i <= 12; i++)
            {
                var month = i;
                var filterStartDate = new DateTime(selectedYear, month, 01);
                var filterEndDate = new DateTime(selectedYear, month, DateTime.DaysInMonth(selectedYear, month), 23, 59, 59); // Monthly Based

                productsFigure[i - 1] = await _unitOfWork.Repository<Product>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync();
                brandsFigure[i - 1] = await _unitOfWork.Repository<Brand>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync();
                documentsFigure[i - 1] = await _unitOfWork.Repository<Document>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync();
                documentTypesFigure[i - 1] = await _unitOfWork.Repository<DocumentType>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync();
                documentExtendedAttributesFigure[i - 1] = await _unitOfWork.Repository<DocumentExtendedAttribute>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync();
            }

            response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Products"], Data = productsFigure });
            response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Brands"], Data = brandsFigure });
            response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Documents"], Data = documentsFigure });
            response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Document Types"], Data = documentTypesFigure });
            response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Document Extended Attributes"], Data = documentExtendedAttributesFigure });

            return await Result<DashboardDataResponse>.SuccessAsync(response);
        }

    }
}