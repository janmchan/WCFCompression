﻿using System;
using System.Diagnostics;
using System.Text;
using System.Web;

namespace WcfServiceDemo
{
    public class InterceptorModule : IHttpModule
    {
        private bool isDisposed = false;

        ~InterceptorModule()
        {
            Dispose(false);
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(Context_BeginRequest);
        }

        public void Context_BeginRequest(object sender, EventArgs e)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm;ss");
            HttpApplication app = sender as HttpApplication;
            HttpContext ctx = app.Context;
            var originalType = app.Request.Headers["Content-Type"];
            var newType = originalType.Replace("soap+xml;charset=UTF-8", "soap+msbin1+gzip");
            app.Request.Headers["Content-Type"] = newType;
            app.Request.ContentType = newType;
            //Enable this line to see the decompressed value using SoapUI
            //app.Request.Filter = new GZipStream(app.Request.Filter, CompressionMode.Decompress);
            //var result = GetContentBody(app.Request);
            Debug.WriteLine($"{timestamp} : { app.Request.ServerVariables["ALL_RAW"]}");
            //System.Diagnostics.Debug.WriteLine($"{ timestamp} : {result}");

        }

        private string GetContentBody(HttpRequest request)
        {
            // Read the request content and then reset the stream position back to its original place.
            var content = request.BinaryRead((int)request.InputStream.Length);
            request.InputStream.Position = 0;
            var contentStr = Encoding.UTF8.GetString(content).Trim();
            return contentStr;
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