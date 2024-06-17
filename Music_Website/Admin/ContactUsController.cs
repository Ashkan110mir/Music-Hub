using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music_Website.Data.Contact_Us_Data;
using Music_Website.Models;
using System.Drawing.Drawing2D;

namespace Music_Website.Admin
{
    [Authorize]
    public class ContactUsController : Controller
    {
        private IContactusDATA _contactusdata;
        public ContactUsController(IContactusDATA contactus)
        {
            _contactusdata = contactus;
        }
        public List<Contact_us> Refresh()
        {
            return _contactusdata.Get_All_Req();
        }
        public IActionResult All_req(int reqtype)
        {
            if (reqtype == 0)
            {
                return View("Views/Admin Page/ContactUsManage.cshtml", Refresh());
            }
            else
            {
                return RedirectToAction(nameof(Show_Req), new { req_type = reqtype });
            }
        }
        public IActionResult Req_detail(int reqid)
        {
            var req = _contactusdata.Get_Req_Detail(reqid);
            if (req != null)
            {
                _contactusdata.Change_visit_status(reqid);
            }
            else
            {
                return BadRequest();
            }
            return View("Views/Admin Page/ContactUs_Full.cshtml", req);
        }
        public IActionResult Delete_Req(int reqid,int ?reqtype)
        {
            if (reqid != 0)
            {
                bool delete_status = _contactusdata.Delete_Req(reqid);
                if (delete_status == true)
                {
                    if (reqtype == 0)
                    {
                        return View("Views/Admin Page/ContactUsManage.cshtml", Refresh());
                    }
                    else
                    {
                        return RedirectToAction(nameof(Show_Req), new {req_type=reqtype});
                    }
                }
                else
                {
                    ViewBag.mess = "خطایی در حذف رخ داد";
                    return View("Views/Admin Page/ContactUsManage.cshtml", Refresh());
                }
            }
            else
            {
                return BadRequest();
            }
        }
        //req type==1 seen 
        //req type==2 not seen 
        public IActionResult Show_Req(int req_type)
        {
            ViewBag.reqtype = req_type;
            if(req_type==1)
            {
                var seen = _contactusdata.Get_seen_req();
                return View("Views/Admin Page/ContactUsManage.cshtml", seen);
            }
            else if(req_type==2) 
            {
                var not_seen = _contactusdata.Get_not_seen_Req();
                return View("Views/Admin Page/ContactUsManage.cshtml", not_seen);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
