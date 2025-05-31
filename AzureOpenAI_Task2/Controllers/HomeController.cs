using AzureOpenAI_Task2.Models.ViewModels;
using AzureOpenAI_Task2.UOW;
using Microsoft.AspNetCore.Mvc;

namespace AzureOpenAI_Task2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("/CreateNewSession")]
        public IActionResult CreateNewSession()
        {
            return View("UploadDocument");
        }

        [HttpGet]
        [Route("/OldSessions")]
        public async Task<IActionResult> OldSessions()
        {
            var oldChats = await _unitOfWork.ChatRepository.GetAllAsync();

            return View("~/Views/Document/OldSessions.cshtml", oldChats);
        }

    }
}
