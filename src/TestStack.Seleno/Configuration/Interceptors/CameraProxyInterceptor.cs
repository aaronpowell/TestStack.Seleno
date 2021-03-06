﻿using System;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using TestStack.Seleno.Configuration.Contracts;

namespace TestStack.Seleno.Configuration.Interceptors
{
    class CameraProxyInterceptor : IInterceptor
    {
        private readonly ICamera _camera;
        private readonly string _filename;
        private readonly ILogger _logger;

        public CameraProxyInterceptor(ICamera camera, string filename, ILogger logger)
        {
            _camera = camera;
            _filename = filename;
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (SelenoReceivedException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.ErrorFormat(e, "Error invoking {0}.{1}", invocation.TargetType.Name, invocation.Method.Name);
                var filename = _filename + "_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

                if (_camera.ScreenshotTaker == null)
                    _logger.WarnFormat("ITakesScreenshot isn't supported by the web driver {0} so taking a screenshot probably won't work", _camera.Browser.GetType().Name);
                else
                    _logger.DebugFormat("Taking screenshot with filename: {0}", filename);

                try
                {
                    _camera.TakeScreenshot(filename);
                }
                catch (Exception ex)
                {
                    _logger.Error("Error saving a screenshot", ex);
                }

                throw new SelenoReceivedException(e);
            }
        }
    }
    
    /// <summary>
    /// Wraps an exception that has been received and processed by Seleno.
    /// </summary>
    public class SelenoReceivedException : Exception
    {
        /// <summary>
        /// Creates a <see cref="SelenoReceivedException"/>.
        /// </summary>
        /// <param name="innerException">The exception that has been caught and processed by Seleno</param>
        public SelenoReceivedException(Exception innerException)
            : base(innerException.Message, innerException)
        {}
    }
}
