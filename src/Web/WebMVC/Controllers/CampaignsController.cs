using Microsoft.EntityFrameworkCore.Query.Internal;
using WebMVC.ViewModels;

namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    using AspNetCore.Authorization;
    using AspNetCore.Mvc;
    using Services;
    using ViewModels;
    using System.Threading.Tasks;
    using System;
    using ViewModels.Pagination;
    using global::WebMVC.ViewModels;
    using Microsoft.Extensions.Options;

    [Authorize]
    public class CampaignsController : Controller
    {
        private readonly ICampaignService _campaignService;
        private readonly AppSettings _settings;

        public CampaignsController(ICampaignService campaignService, IOptionsSnapshot<AppSettings> settings)
        {
            _campaignService = campaignService;
            _settings = settings.Value;
        }

        public async Task<IActionResult> Index(int page = 0, int pageSize = 10)
        {
            var campaignList = await _campaignService.GetCampaigns(pageSize, page);

            if(campaignList is null)
            {
                return View();
            }

            var totalPages = (int) Math.Ceiling((decimal) campaignList.Count / pageSize);

            var vm = new CampaignViewModel
            {
                CampaignItems = campaignList.Data,
                PaginationInfo = new PaginationInfo
                {
                    ActualPage = page,
                    ItemsPerPage = campaignList.Data.Count,
                    TotalItems = campaignList.Count,
                    TotalPages = totalPages,
                    Next = page == totalPages - 1 ? "is-disabled" : "",
                    Previous = page == 0 ? "is-disabled" : ""
                }
            };

            ViewBag.IsCampaignDetailFunctionActive = _settings.ActivateCampaignDetailFunction;

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var campaignDto = await _campaignService.GetCampaignById(id);

            if (campaignDto is null)
            {
                return NotFound();
            }

            var campaign = new CampaignItem
            {
                Id = campaignDto.Id,
                Name = campaignDto.Name,
                Description = campaignDto.Description,
                From = campaignDto.From,
                To = campaignDto.To,
                PictureUri = campaignDto.PictureUri
            };

            return View(campaign);
        }
    }
}