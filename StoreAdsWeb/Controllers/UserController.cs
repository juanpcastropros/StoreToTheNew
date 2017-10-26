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
using BusinessCore.Security;

namespace StoreAdsWeb.Controllers
{
    public class UserController : Controller
    {
        private STTContext db = new STTContext();
        private CloudQueue imagesQueue;
        private static CloudBlobContainer imagesBlobContainer;

        public UserController()
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
        public async Task<ActionResult> Index(int? level)
        {
            // This code executes an unbounded query; don't do this in a production app,
            // it could return too many rows for the web app to handle. For an example
            // of paging code, see:
            // http://www.asp.net/mvc/tutorials/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
            var userList = db.Users.AsQueryable();
            if (level != null)
            {
                userList= userList.Where(a => a.Level== (AccessLevel)level);
            }
            return View(await userList.ToListAsync());
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
            [Bind(Include = "Name,Password,Level")] User obj)
        {
            CloudBlockBlob imageBlob = null;
            // A production app would implement more robust input validation.
            // For example, validate that the image file size is not too large.
            if (ModelState.IsValid)
            {

                obj.ModificationDate = obj.CreationDate = DateTime.Now;
                obj.CreationUser = obj.ModificatioUser = "admin";
                db.Users.Add(obj);
                await db.SaveChangesAsync();
                Trace.TraceInformation("Created category {0} in database", obj.Id);

                if (imageBlob != null)
                {
                    var queueMessage = new CloudQueueMessage(obj.Id.ToString());
                    await imagesQueue.AddMessageAsync(queueMessage);
                    Trace.TraceInformation("Created queue message for category {0}", obj.Id);
                }
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        public async Task<ActionResult> Details(Guid? id, string name)
        {
            if (id == null || name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User obj = await db.Users.FindAsync(id, name);
            if (obj == null)
            {
                return HttpNotFound();
            }
            return View(obj);
        }

        // GET: Ad/Edit/5
        public async Task<ActionResult> Edit(Guid? id, string name)
        {
            if (id == null || name==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User obj = await db.Users.FindAsync(id, name);
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
            [Bind(Include = "Id,Name,Password,level,CreationDate,CreationUser,")] User obj)
        {
            CloudBlockBlob imageBlob = null;
            if (ModelState.IsValid)
            {
                obj.ModificationDate = DateTime.Now;
                obj.ModificatioUser = "admin";
                db.Entry(obj).State = EntityState.Modified;
                await db.SaveChangesAsync();
                Trace.TraceInformation("Updated AdId {0} in database", obj.Id);

                if (imageBlob != null)
                {
                    var queueMessage = new CloudQueueMessage(obj.Id.ToString());
                    await imagesQueue.AddMessageAsync(queueMessage);
                    Trace.TraceInformation("Created queue message for AdId {0}", obj.Id);
                }
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET: Ad/Delete/5
        public async Task<ActionResult> Delete(Guid? id, string name)
        {
            if (id == null || name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User obj = await db.Users.FindAsync(id, name);
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
            User obj = await db.Users.FindAsync(id);

            db.Users.Remove(obj);
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