using BusinessCore.Context;
using BusinessCore.Security;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace StoreAdsWeb.Controllers
{
    public class NoLogController : Controller
    {
        private STTContext db = new STTContext();
        private CloudQueue imagesQueue;
        private static CloudBlobContainer imagesBlobContainer;
        // GET: NoLog

        public NoLogController()
        {
            InitializeStorage();
        }
        private void InitializeStorage()
        {
            // Open storage account using credentials from .cscfg file.
            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));


            // Get context object for working with blobs, and 
            // set a default retry policy appropriate for a web user interface.
            var blobClient = storageAccount.CreateCloudBlobClient();
            blobClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the blob container.
            imagesBlobContainer = blobClient.GetContainerReference("images");

            // Get context object for working with queues, and 
            // set a default retry policy appropriate for a web user interface.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            queueClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the queue.
            imagesQueue = queueClient.GetQueueReference("images");
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Validate(
           [Bind(Include = "Name,Password")] User obj)
        {
            // A production app would implement more robust input validation.
            // For example, validate that the image file size is not too large.
            if (ModelState.IsValid)
            {
                var temp= db.Users.Where(user => user.Name == obj.Name).Select(user => new { user.Name, user.Password ,user.Level});
                if(temp!=null)
                {
                    if (temp.Count().Equals(1))
                    {
                        if (temp.First().Password.Equals(obj.Password))
                        {
                            Session["user"] = "Valid";
                            Session["userID"] = temp.First().Name;
                            Session["userLevel"] = temp.First().Level;
                            return RedirectToAction("../Home");
                        }
                    }
                }
                Trace.TraceInformation("Created category {0} in database", obj.Id);

                
            }

            return RedirectToAction("Index/wrong");
        }
    }
}