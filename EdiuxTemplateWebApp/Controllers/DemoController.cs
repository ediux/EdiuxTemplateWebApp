using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Controllers
{
    [AllowAnonymous]
    public class DemoController : Controller
    {
        public DemoController()
        {
            StorageRoot = Path.Combine( HttpRuntime.AppDomainAppPath, "Upload");
        }
        // GET: Demo
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Manage");
        }

        public ActionResult General()
        {
            return View();
        }

        public ActionResult Buttons()
        {
            return View();
        }

        public ActionResult Tabs()
        {
            return View();
        }

        public ActionResult Accordions()
        {
            return View();
        }

        public ActionResult NestableList()
        {
            return View();
        }
        public ActionResult Grid()
        {
            return View();
        }

        public ActionResult Dialogs()
        {
            return View();
        }

        public ActionResult FormElements()
        {
            return View();
        }

        public ActionResult FormValidation()
        {
            return View();
        }

        public ActionResult FormMasks()
        {
            return View();
        }

        public ActionResult MultipleFileUpload()
        {
            if (Request.IsAjaxRequest())
            {
                List<ViewDataUploadFilesResult> result = new List<ViewDataUploadFilesResult>();
                result.Add(new ViewDataUploadFilesResult() { isUploaded = true, error = "test" });
                return Json(result, "application/json", JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        private string StorageRoot;
        [HttpPost]
        public ActionResult MultipleFileUpload(IEnumerable<HttpPostedFileBase> demos)
        {
            var r = new List<ViewDataUploadFilesResult>();

            if (demos == null)
            {
                if (Request.Files.Count == 0)
                {
                    r.Add(new ViewDataUploadFilesResult() { isUploaded = false, error = "File is empty." });
                    return Json(new { files=r } , "application/json", JsonRequestBehavior.AllowGet);
                }

                if (Request.Files.Count > 0)
                {
                    if (Directory.Exists(StorageRoot) == false)
                    {
                        Directory.CreateDirectory(StorageRoot);
                    }
                    try
                    {
                        var statuses = r;

                        var headers = Request.Headers;

                        if (string.IsNullOrEmpty(headers["X-File-Name"]))
                        {
                            UploadWholeFile(Request, statuses);
                        }
                        else
                        {
                            UploadPartialFile(headers["X-File-Name"], Request, statuses);
                        }

                        JsonResult result = Json(new { files = r }, "application/json", JsonRequestBehavior.AllowGet);                      
                        return result;
                    }
                    catch (Exception ex)
                    {
                        r.Add(new Controllers.ViewDataUploadFilesResult()
                        {
                            isUploaded = false,
                            error = ex.Message                         
                        });
                        return Json(new { files = r }, "application/json", JsonRequestBehavior.AllowGet);
                    }
                }
            }


            //List<object> response = new List<object>();

            foreach (var file in demos)
            {
                try
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var filename = Path.GetFileName(file.FileName);
                        string uploadfolder = StorageRoot;
                        if (Directory.Exists(uploadfolder) == false)
                        {
                            Directory.CreateDirectory(uploadfolder);
                        }
                        var path = Path.Combine(uploadfolder, filename);
                        file.SaveAs(path);
                        r.Add(new ViewDataUploadFilesResult()
                        {
                            isUploaded = true,
                            error="",
                            name = filename,
                            size = file.ContentLength,
                            type = file.ContentType,
                            url = Url.Action("Download", "Demo", new { id = filename }),
                            delete_url = Url.Action("Delete", "Demo", new { id = filename }),
                            thumbnail_url = "",
                            delete_type = "GET",
                        });
                    }
                }
                catch (Exception ex)
                {
                    var filename = Path.GetFileName(file.FileName);

                    r.Add(new ViewDataUploadFilesResult()
                    {
                        isUploaded = true,
                        error = ex.Message,
                        name = file.FileName,
                        size = file.ContentLength,
                        type = file.ContentType,
                        url = Url.Action("Download", "Demo", new { id = filename }),
                        delete_url = Url.Action("Delete", "Demo", new { id = filename }),
                        thumbnail_url = @"",
                        delete_type = "GET",
                    });
                    continue;
                }
                
            }
            //return Json(response, "text/html", JsonRequestBehavior.AllowGet);

            return Json(new { files = r }, "application/json", JsonRequestBehavior.AllowGet);

        }

        public ActionResult Download(string id)
        {
            return View();
        }
        private string EncodeFile(string fileName)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
        }

        //DONT USE THIS IF YOU NEED TO ALLOW LARGE FILES UPLOADS
        //Credit to i-e-b and his ASP.Net uploader for the bulk of the upload helper methods - https://github.com/i-e-b/jQueryFileUpload.Net
        private void UploadPartialFile(string fileName, HttpRequestBase request, List<ViewDataUploadFilesResult> statuses)
        {
            if (request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var file = request.Files[0];
            var inputStream = file.InputStream;

            var fullName = Path.Combine(StorageRoot, Path.GetFileName(fileName));

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }
            ViewDataUploadFilesResult fileresult = new ViewDataUploadFilesResult()
            {
                isUploaded = true,
                name = fileName,
                size = file.ContentLength,
                type = file.ContentType,
                url = Url.Action("Download", "Demo", new { id = fileName }),
                delete_url = Url.Action("Delete", "Demo", new { id = fileName }),
                thumbnail_url = "",
                delete_type = "GET",
            };

            if (fileresult.type.ToLowerInvariant().StartsWith("image/"))
            {
                fileresult.thumbnail_url = string.Format("/Upload/{0}", fileName);
            }
          

            statuses.Add(fileresult);
        }

        //DONT USE THIS IF YOU NEED TO ALLOW LARGE FILES UPLOADS
        //Credit to i-e-b and his ASP.Net uploader for the bulk of the upload helper methods - https://github.com/i-e-b/jQueryFileUpload.Net
        private void UploadWholeFile(HttpRequestBase request, List<ViewDataUploadFilesResult> statuses)
        {
            for (int i = 0; i < request.Files.Count; i++)
            {
                try
                {
                    var file = request.Files[i];

                    var fullPath = Path.Combine(StorageRoot, Path.GetFileName(file.FileName));

                    file.SaveAs(fullPath);

                    ViewDataUploadFilesResult fileresult = new ViewDataUploadFilesResult()
                    {
                        isUploaded = true,
                        name = file.FileName,
                        size = file.ContentLength,
                        type = file.ContentType,
                        url = Url.Action("Download", "Demo", new { id = file.FileName }),
                        delete_url = Url.Action("Delete", "Demo", new { id = file.FileName }),
                        thumbnail_url = "",
                        delete_type = "GET",
                    };

                    if (fileresult.type.ToLowerInvariant().StartsWith("image/"))
                    {
                        fileresult.thumbnail_url = string.Format("/Upload/{0}", file.FileName);
                    }

                    statuses.Add(fileresult);
                }
                catch (Exception ex)
                {
                    ViewDataUploadFilesResult fileresult = new ViewDataUploadFilesResult()
                    {
                        isUploaded = false,
                        error = ex.Message,
                        name = request.Files[i].FileName,
                        size = request.Files[i].ContentLength,
                        type = request.Files[i].ContentType,
                        url = Url.Action("Download", "Demo", new { id = request.Files[i].FileName }),
                        delete_url = Url.Action("Delete", "Demo", new { id = request.Files[i].FileName }),
                        thumbnail_url = "",
                        delete_type = "GET",
                    };

                    if (fileresult.type.ToLowerInvariant().StartsWith("image/"))
                    {
                        fileresult.thumbnail_url = string.Format("/Upload/{0}", request.Files[i].FileName);
                    }

                    statuses.Add(fileresult);

                    continue;
                }
          
            }
        }

        public ActionResult DropzoneFileUpload()
        {
            return View();
        }

        public ActionResult StaticTable()
        {
            return View();
        }

        public ActionResult DynamicTable()
        {
            return View();
        }

        public ActionResult Chart()
        {
            return View();
        }

        public ActionResult IconsNew()
        {
            return View();
        }

        public ActionResult Icons()
        {
            return View();
        }
    }

    public class ViewDataUploadFilesResult
    {
        public string name { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string delete_url { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_type { get; set; }
        public bool isUploaded { get; set; }
        public string error { get; set; }
    }
}