using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;

namespace WcfServiceDemo
{
    public class GZipDecompressModule : IHttpModule
    {
        private bool isDisposed = false;

        ~GZipDecompressModule()
        {
            Dispose(false);
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(Context_BeginRequest);
            context.PreSendRequestHeaders += new EventHandler(context_PreSendRequestHeaders);
        }

        void context_PreSendRequestHeaders(object sender, EventArgs e)
        {
            // Fix headers having been lost if an exception occurred.
            HttpApplication app = sender as HttpApplication;
            HttpContext ctx = app.Context;
            if (app.Response.Filter is GZipStream) SetEncoding("gzip");
            else if (app.Response.Filter is DeflateStream) SetEncoding("deflate");

            // Fix double header
            if (ctx.Response.Headers["Content-encoding"] == "gzip,gzip")
                ctx.Response.Headers.Set("Content-encoding", "gzip");
        }

        /// <summary>
        /// Fires at the start of each new web request
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">Event Arguments</param>
        public void Context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            HttpContext ctx = app.Context;

            string requestEncoding = ctx.Request.Headers["Content-encoding"];
            if (requestEncoding != null && requestEncoding == "gzip")
            {
                app.Request.Filter =
                 new GZipStream(app.Request.Filter, CompressionMode.Decompress);

                var content = app.Request.BinaryRead((int)app.Request.InputStream.Length);
                app.Request.InputStream.Position = 0;
                var contentStr = Encoding.UTF8.GetString(content);

                /*ctx.Request.Filter =
                 new GZipStream(app.Request.Filter, CompressionMode.Decompress);

                var content2 = ctx.Request.BinaryRead((int)ctx.Request.InputStream.Length);
                ctx.Request.InputStream.Position = 0;
                var contentStr2 = Encoding.UTF8.GetString(content2);*/

                /*using (GZipStream gzip = new GZipStream(ctx.Request.InputStream, CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(gzip))
                    {
                        String content = reader.ReadToEnd();
                        var result = content;
                    }
                }*/
            }


            // Add response compression filter if the client accepts compressed responses
            if (IsEncodingAccepted("gzip"))
            {
                app.Response.Filter = new GZipStream(ctx.Response.Filter, CompressionMode.Compress);
                SetEncoding("gzip");
            }
            else if (IsEncodingAccepted("deflate"))
            {
                app.Response.Filter = new DeflateStream(ctx.Response.Filter, CompressionMode.Compress);
                SetEncoding("deflate");
            }
        }

        private bool IsEncodingAccepted(string encoding)
        {
            return HttpContext.Current.Request.Headers["Accept-encoding"] != null &&
                HttpContext.Current.Request.Headers["Accept-encoding"].Contains(encoding);
        }

        private void SetEncoding(string encoding)
        {
            HttpContext ctx = HttpContext.Current;
            string responseEncodings = ctx.Response.Headers.Get("Content-encoding");
            if (responseEncodings == null || !responseEncodings.Contains(encoding))
                HttpContext.Current.Response.AppendHeader("Content-encoding", encoding);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool dispose)
        {
            isDisposed = dispose;
        }
    }
}