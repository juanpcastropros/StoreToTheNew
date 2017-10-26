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
    public class ProductController : Controller
    {
        private STTContext db = new STTContext();
        private CloudQueue imagesQueue;
        private static CloudBlobContainer imagesBlobContainer;

        public ProductController()
        {
            InitializeStorage();
        }
        // GET: Product
        public async Task<ActionResult> Index(int? category)
        {
            // This code executes an unbounded query; don't do this in a production app,
            // it could return too many rows for the web app to handle. For an example
            // of paging code, see:
            // http://www.asp.net/mvc/tutorials/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
            var productLst = db.Products.AsQueryable();
            if (category != null)
            {
                //productLst = productLst.Where(a => a.Category == (Category)category);
            }
            return View(await productLst.ToListAsync());
            //return View();
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

        public ActionResult Create()
        {
            return View();
        }
        // POST: Ad/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "StoreID,Name,Price,Description,CategoryID")] StoreAdsWeb.Models.ProductModel productModel, // Product product,
            HttpPostedFileBase imageFile)
        {
            CloudBlockBlob imageBlob = null;
            // A production app would implement more robust input validation.
            // For example, validate that the image file size is not too large.
            Guid idtmp;
            Product product = new Product();
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength != 0)
                {
                    imageBlob = await UploadAndSaveBlobAsync(imageFile);
                    product.ImageURL = imageBlob.Uri.ToString();
                }
                product.Price = productModel.Price;
                product.Name = productModel.Name;
                product.Description = productModel.Description;
                idtmp = Guid.Parse(productModel.CategoryID);
                product.Category = db.Categories.SingleOrDefault(i => i.Id.Equals(idtmp));
                idtmp = Guid.Parse(productModel.StoreID);
                product.Store = db.Stores.SingleOrDefault(i => i.Id.Equals(idtmp));
                product.CreationUser = product.ModificatioUser = Session["UserID"].ToString();
                product.ModificationDate=product.CreationDate = DateTime.Now;
                db.Products.Add(product);
                await db.SaveChangesAsync();
                Trace.TraceInformation("Created Product {0} in database", product.Id);

                if (imageBlob != null)
                {
                    var queueMessage = new CloudQueueMessage(product.Id.ToString());
                    await imagesQueue.AddMessageAsync(queueMessage);
                    Trace.TraceInformation("Created queue message for AdId {0}", product.Id);
                }
                return RedirectToAction("Index");
            }

            return View(product);
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
    }
}