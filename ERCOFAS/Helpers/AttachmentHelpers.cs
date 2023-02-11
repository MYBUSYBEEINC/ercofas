using ERCOFAS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace ERCOFAS.Helpers
{
    /// <summary>
    /// The helper class for the attachment.
    /// </summary>
    public static class AttachmentHelpers
    {
        /// <summary>
        /// Saves the attachment to the directory.
        /// </summary>
        /// <param name="file">The posted file.</param>
        public static Tuple<string, string, string> SaveToDirectory(HttpPostedFileBase file)
        {
            var fileNameWithExtension = Path.GetFileName(file.FileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(fileNameWithExtension);
            string shortUniqueId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            string uniqueId = StringExtensions.RemoveSpecialCharacters(shortUniqueId);
            string uniqueFileName = string.Format("{0}_{1}{2}", fileNameWithoutExtension, uniqueId, extension);
            var path = Path.Combine(HostingEnvironment.MapPath("~/Documents"), uniqueFileName);
            file.SaveAs(path);

            return Tuple.Create(fileNameWithExtension, uniqueFileName, path);
        }
    }
}