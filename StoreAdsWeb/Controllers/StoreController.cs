using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.IO;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System.Diagnostics;
using BusinessCore;
using BusinessCore.Context;

namespace StoreAdsWeb.Controllers
{
    public class StoreController : Controller
    {
        private STTContext db = new STTContext();
        private CloudQueue imagesQueue;
        private static CloudBlobContainer imagesBlobContainer;

        public StoreController()
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
        // GET: Category
        public async Task<ActionResult> Index(int? obj)
        {
            // This code executes an unbounded query; don't do this in a production app,
            // it could return too many rows for the web app to handle. For an example
            // of paging code, see:
            // http://www.asp.net/mvc/tutorials/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
            var list = db.Stores.AsQueryable();
            if (obj != null)
            {
                //productLst = productLst.Where(a => a.Category == (Category)category);
            }
            return View(await list.ToListAsync());
            //return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        // POST: Ad/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "Name,Description,Coordinates")] Store obj,
            HttpPostedFileBase imageFile)
        {
            CloudBlockBlob imageBlob = null;
            // A production app would implement more robust input validation.
            // For example, validate that the image file size is not too large.
            if (ModelState.IsValid)
            {

                if (imageFile != null && imageFile.ContentLength != 0)
                {
                    imageBlob = await UploadAndSaveBlobAsync(imageFile);
                    obj.ImageURL = imageBlob.Uri.ToString();
                }
                obj.ModificationDate = obj.CreationDate = DateTime.Now;
                obj.ModificatioUser = obj.CreationUser = "Admin";
                db.Stores.Add(obj);
                await db.SaveChangesAsync();
                Trace.TraceInformation("Created category {0} in database", obj.Id);

                if (imageBlob != null)
                {
                    var queueMessage = new CloudQueueMessage(string.Concat("Store||",obj.Id.ToString()));
                    await imagesQueue.AddMessageAsync(queueMessage);
                    Trace.TraceInformation("Created queue message for category {0}", obj.Id);
                }
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store obj = await db.Stores.FindAsync(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            return View(obj);
        }

        // GET: Ad/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store obj = await db.Stores.FindAsync(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            return View(obj);
        }

        // POST: Ad/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,Name,Description,CreationDate,CreationUser,Coordinates,ImageURL,ThumbnailURL")] Store obj,
            HttpPostedFileBase imageFile)
        {
            CloudBlockBlob imageBlob = null;
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength != 0)
                {
                    imageBlob = await UploadAndSaveBlobAsync(imageFile);
                    obj.ImageURL = imageBlob.Uri.ToString();
                }
                obj.ModificationDate = DateTime.Now;
                db.Entry(obj).State = EntityState.Modified;
                await db.SaveChangesAsync();
                Trace.TraceInformation("Updated AdId {0} in database", obj.Id);

                if (imageBlob != null)
                {
                    var queueMessage = new CloudQueueMessage(string.Concat("Store||", obj.Id.ToString()));
                    await imagesQueue.AddMessageAsync(queueMessage);
                    Trace.TraceInformation("Created queue message for AdId {0}", obj.Id);
                }
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        private async Task<CloudBlockBlob> UploadAndSaveBlobAsync(HttpPostedFileBase imageFile)
        {
            Trace.TraceInformation("Uploading image file {0}", imageFile.FileName);

            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            // Retrieve reference to a blob. 
            CloudBlockBlob imageBlob = imagesBlobContainer.GetBlockBlobReference(blobName);
            // Create the blob by uploading a local file.
            using (var fileStream = imageFile.InputStream)
            {
                await imageBlob.UploadFromStreamAsync(fileStream);
            }

            Trace.TraceInformation("Uploaded image file to {0}", imageBlob.Uri.ToString());

            return imageBlob;
        }
        // GET: Ad/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store obj = await db.Stores.FindAsync(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            return View(obj);
        }

        // POST: Ad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Store obj = await db.Stores.FindAsync(id);

            db.Stores.Remove(obj);
            await db.SaveChangesAsync();
            Trace.TraceInformation("Deleted ad {0}", obj.Id);
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}