namespace BugBounty
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;
    using System.Web;
    using System.Net;
    using System.Text.RegularExpressions;

    public class FileUpload
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var fileName = string.Empty;
                var timeStamp = context.Request["timeStamp"];
                var bugId = Guid.NewGuid();
                var targetFolder = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\images\\" + bugId.ToString());
                var file = context.Request.Files[0];

                if (file.ContentLength != 0)
                {
                    if (!Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }

                    if (File.Exists(targetFolder + "\\" + timeStamp + ".png"))
                    {
                        File.Delete(targetFolder + "\\" + timeStamp + ".png");
                    }

                    var binaryReader = new BinaryReader(file.InputStream);
                    var memoryBytes = binaryReader.ReadBytes(file.ContentLength);
                    using (var memoryStream = new MemoryStream(memoryBytes))
                    {
                        var imageStream = Image.FromStream(memoryStream);
                        imageStream.Save(targetFolder + "\\" + timeStamp + ".png", ImageFormat.Png);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}