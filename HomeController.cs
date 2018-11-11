using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PdfSharp.Drawing;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace Spinner.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        [HttpPost]
        public ActionResult Download(FormCollection fc)
        {
            //start the tier to measure the download time
            System.Diagnostics.Stopwatch downloadTimer = new System.Diagnostics.Stopwatch();
            downloadTimer.Start();

            StringBuilder HTMLstring = new StringBuilder("<html><head></head><body><h1>Example PDF page. Spinner in ASP.NET MVC. thoughtsonprogramming.wordpress.com</h1></body></html>");
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + "PDFfile.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            var configurationOptions = new PdfGenerateConfig();
            configurationOptions.PageOrientation = PdfSharp.PageOrientation.Landscape;
            configurationOptions.PageSize = PdfSharp.PageSize.Letter;
            configurationOptions.MarginBottom = 19;
            configurationOptions.MarginLeft = 2;

            var output = PdfGenerator.GeneratePdf(HTMLstring.ToString(), configurationOptions);

            XFont font = new XFont("Segoe UI,Open Sans, sans-serif, serif", 7);
            XBrush brush = XBrushes.Black;

            var outputstream = new MemoryStream();
            output.Save(outputstream);

            var temp = fc["spinnerToken"].ToString().Trim();

            //WARNING : This is only for this example REMOVE IN REAL WORLD SCENARIO !!!!!!!!!!!!!!!!!!!!!!!
            //System.Threading.Thread.Sleep(5000);
            //WARNING : This is only for this example REMOVE IN REAL WORLD SCENARIO !!!!!!!!!!!!!!!!!!!!!!!
            
            
            downloadTimer.Stop();
            string timeTookToDownload = downloadTimer.Elapsed.Days + " Days " + downloadTimer.Elapsed.Hours + " Hours " + downloadTimer.Elapsed.Minutes + " Minutes " + downloadTimer.Elapsed.Seconds + " Seconds " + downloadTimer.Elapsed.Milliseconds + " Milliseconds ";

            Session["spinnerToken"] = fc["spinnerToken"].ToString().Trim();
            Session["timetooktodownload"] = timeTookToDownload;

            Response.BinaryWrite(outputstream.ToArray());

            return View("Index");
        }
        [HttpPost]
        public ActionResult CheckForSpinnerSession()
        {
            string returnvalue = "false";
            string fileDownloadTime = "false";

            if (HttpContext.Session["spinnerToken"] != null)
            {
                returnvalue = Session["spinnerToken"].ToString();
            }

            if (HttpContext.Session["timetooktodownload"] != null)
            {
                fileDownloadTime = HttpContext.Session["timetooktodownload"].ToString();
            }
            
            return Json(new Models.DownloadStats {  FileDownloadOK= returnvalue , FileDownloadTime= fileDownloadTime});
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}