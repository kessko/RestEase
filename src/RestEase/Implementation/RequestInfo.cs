﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestEase.Implementation
{
    /// <summary>
    /// Class containing information to construct a request from.
    /// An instance of this is created per request by the generated interface implementation
    /// </summary>
    public class RequestInfo
    {
        /// <summary>
        /// Gets the HttpMethod which should be used to make the request
        /// </summary>
        public HttpMethod Method { get; private set; }

        /// <summary>
        /// Gets the relative path to the resource to request
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the CancellationToken used to cancel the request
        /// </summary>
        public CancellationToken CancellationToken { get; private set; }

        /// <summary>
        /// Gets the query parameters to append to the request URI
        /// </summary>
        public List<KeyValuePair<string, string>> QueryParams { get; private set; }

        /// <summary>
        /// Gets the parameters which should be substituted into placeholders in the Path
        /// </summary>
        public List<KeyValuePair<string, string>> PathParams { get; private set; }

        /// <summary>
        /// Gets the headers which were applied to the interface
        /// </summary>
        public List<string> ClassHeaders { get; private set; }

        /// <summary>
        /// Gets the headers which were applied to the method being called
        /// </summary>
        public List<string> MethodHeaders { get; private set; }

        /// <summary>
        /// Gets the headers which were passed to the method as parameters
        /// </summary>
        public List<KeyValuePair<string, string>> HeaderParams { get; private set; }

        /// <summary>
        /// Gets information the [Body] method parameter, if it exists
        /// </summary>
        public BodyParameterInfo BodyParameterInfo { get; private set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="RequestInfo"/> class
        /// </summary>
        /// <param name="method">HttpMethod to use when making the request</param>
        /// <param name="path">Relative path to request</param>
        /// <param name="cancellationToken">CancellationToken to use to cancel the requwest</param>
        public RequestInfo(HttpMethod method, string path, CancellationToken cancellationToken)
        {
            this.Method = method;
            this.Path = path;
            this.CancellationToken = cancellationToken;

            this.QueryParams = new List<KeyValuePair<string, string>>();
            this.PathParams = new List<KeyValuePair<string, string>>();
            this.ClassHeaders = new List<string>();
            this.MethodHeaders = new List<string>();
            this.HeaderParams = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Add a query parameter
        /// </summary>
        /// <remarks>value may be an IEnumerable, in which case each value is added separately</remarks>
        /// <typeparam name="T">Type of the value to add</typeparam>
        /// <param name="name">Name of the name/value pair</param>
        /// <param name="value">Value of the name/value pair</param>
        public void AddQueryParameter<T>(string name, T value)
        {
            // Don't want to count strings as IEnumerable
            if (value != null && !(value is string) && value is IEnumerable)
            {
                foreach (var individualValue in (IEnumerable)value)
                {
                    this.QueryParams.Add(new KeyValuePair<string, string>(name, (individualValue ?? String.Empty).ToString()));
                }
            }
            else
            {
                string stringValue = null;
                if (value != null)
                    stringValue = value.ToString();
                this.QueryParams.Add(new KeyValuePair<string, string>(name, stringValue));
            }
        }

        /// <summary>
        /// Add a path parameter: a [PathParam] method parameter which is used to substitude a placeholder in the path
        /// </summary>
        /// <typeparam name="T">Type of the value of the path parameter</typeparam>
        /// <param name="name">Name of the name/value pair</param>
        /// <param name="value">Value of the name/value pair</param>
        public void AddPathParameter<T>(string name, T value)
        {
            string stringValue = null;
            if (value != null)
                stringValue = value.ToString();
            this.PathParams.Add(new KeyValuePair<string, string>(name, stringValue));
        }

        /// <summary>
        /// Add a header which is defined on the interface itself
        /// </summary>
        /// <param name="header">Header to add</param>
        public void AddClassHeader(string header)
        {
            this.ClassHeaders.Add(header);
        }

        /// <summary>
        /// Add a header which is defined on the method
        /// </summary>
        /// <param name="header">Header to add</param>
        public void AddMethodHeader(string header)
        {
            this.MethodHeaders.Add(header);
        }

        /// <summary>
        /// Add a header which is defined as a [Header("foo")] parameter to the method
        /// </summary>
        /// <param name="name">Name of the header (passed to the HeaderAttribute)</param>
        /// <param name="value">Value of the header (value of the parameter)</param>
        public void AddHeaderParameter(string name, string value)
        {
            this.HeaderParams.Add(new KeyValuePair<string, string>(name, value));
        }

        /// <summary>
        /// Set the body specified by the optional [Body] method parameter
        /// </summary>
        /// <param name="serializationMethod">Method to use to serialize the body</param>
        /// <param name="value">Body to serialize</param>
        public void SetBodyParameterInfo(BodySerializationMethod serializationMethod, object value)
        {
            this.BodyParameterInfo = new BodyParameterInfo(serializationMethod, value);
        }
    }
}