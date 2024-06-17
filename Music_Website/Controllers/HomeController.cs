using Microsoft.AspNetCore.Mvc;
using Music_Website.Data.Contact_Us_Data;
using Music_Website.Data.Music_Data;
using Music_Website.Data.Music_Video_Data;
using Music_Website.Models;
using System.Diagnostics;

namespace Music_Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IContactusDATA _contactusdata;
        private IMusicVideo_Data _musicvideoData;
        private IMusicData _musicData;
        public HomeController(ILogger<HomeController> logger,IContactusDATA contactusdata,IMusicVideo_Data musicVideo_Data,IMusicData musicData)
        {
            _logger = logger;
            _contactusdata = contactusdata;
            _musicvideoData = musicVideo_Data;
            _musicData = musicData;
        }

        public IActionResult Index()
        {
            Music_Website.Models.Mian_Page_View_Model.Index_ViewModel index_ViewModel=new Models.Mian_Page_View_Model.Index_ViewModel();
            index_ViewModel.Music_Video = _musicvideoData.Get_main_menu_mv();
            index_ViewModel.musics = _musicData.Get_Main_Index_musics();
            return View(index_ViewModel);
        }

        public IActionResult Contact_us()
        {
            return View("Views/Home/Contact Us.cshtml");
        }
        public IActionResult Add_contact_us(Contact_us contact)
        {
            Contact_us newrequest = new Contact_us();
            newrequest = contact;
            newrequest.CreatedDate = DateTime.Now;
            newrequest.see_by_admin = false;
            bool addstatus = _contactusdata.add_request(newrequest);
            if(addstatus==true)
            {
                ViewBag.mess = "درخواست شما با موفقیت ثبت شد";
                return View("Views/Home/Contact Us.cshtml");
            }
            else
            {
                ViewBag.mess = "افزودن درخواست با خطا مواجه شد";
                return View("Views/Home/Contact Us.cshtml");
            }
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}