using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void UpdateHeaders(ref HttpApplication app, ref HttpContext ctx, string header, string value)
        {
            app.Request.Headers[header] = value;
            ctx.Request.Headers[header] = value;

        }

        public void RemoveHeaders(ref HttpApplication app, ref HttpContext ctx, string header)
        {
            app.Request.Headers.Remove(header);
            ctx.Request.Headers.Remove(header);

        }

        private string GetContentBody(HttpRequest request)
        {
            // Read the request content and then reset the stream position back to its original place.
            var content = request.BinaryRead((int)request.InputStream.Length);
            request.InputStream.Position = 0;
            var contentStr = Encoding.UTF8.GetString(content).Trim();
            return contentStr;
        }

        /// <summary>
        /// Fires at the start of each new web request
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">Event Arguments</param>
        public void Context_BeginRequest(object sender, EventArgs e)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm;ss");
            HttpApplication app = sender as HttpApplication;
            HttpContext ctx = app.Context;

            string requestEncoding = ctx.Request.Headers["Content-encoding"];

            foreach(string header  in app.Request.Headers)
            {
                Debug.WriteLine($"{header} : {app.Request.Headers[header]}");
            }
            
            /*
            if (requestEncoding != null && requestEncoding == "gzip")
            {
                app.Request.Filter =
                 new GZipStream(app.Request.Filter, CompressionMode.Decompress);
                var content = app.Request.BinaryRead((int)app.Request.InputStream.Length);
                app.Request.InputStream.Position = 0;
                
                var contentStr = Encoding.UTF8.GetString(content);

                Debug.WriteLine($"{timestamp} : {contentStr}");

                using (GZipStream gzip = new GZipStream(ctx.Request.InputStream, CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(gzip))
                    {
                        String content = reader.ReadToEnd();
                        var result = content;
                    }
                }
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
            }*/
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