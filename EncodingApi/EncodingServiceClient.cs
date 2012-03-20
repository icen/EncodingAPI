﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using EncodingApi.Models;

namespace EncodingApi
{
    public class EncodingServiceClient : BasicXmlWebServiceClient
    {
        ///// <summary>
        ///// The default host URI.
        ///// </summary>
        public override Uri DefaultHost 
        { get { return new Uri("http://manage.encoding.com/"); } }

        ///// <summary>
        ///// The host URI to use when SSL is enabled.
        ///// </summary>
        public override Uri DefaultSslHost 
        { get { return new Uri("https://manage.encoding.com/"); } }

        /// <summary>
        /// Encoding.com's user id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Encoding.com's user key.
        /// </summary>
        public string UserKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the EncodingWebRequest class.
        /// </summary>
        public EncodingServiceClient()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EncodingWebRequest class for the specified user key
        /// and user id.
        /// </summary>
        /// <param name="uid">The 3 to 5 digits user id.</param>
        /// <param name="ukey">The user key.</param>
        public EncodingServiceClient(string uid, string ukey)
            : base()
        {
            UserId = uid;
            UserKey = ukey;
        }

        /// <summary>
        /// Requests a list of media meta from server.
        /// </summary>
        /// <returns>A collection of GetMediaListResponse.Media.</returns>
        public ICollection<GetMediaListResponse.Media> GetMediaList()
        {
            var result = GetResponse<GetMediaListResponse>(EncodingQuery.CreateGetMediaListQuery());
            if (result.Errors.Count > 0)
            {
                string message = result.Errors.First();
                EncodingServiceException ex = new EncodingServiceException(message);
                ex.Data.Add("errors", result.Errors);
                throw ex;
            }
            return result.MediaList;
        }

        public void GetMediaListAsync(Action<ICollection<GetMediaListResponse.Media>> callback,
                                      Action<ICollection<string>> errors)
        {
            //SendGetMediaListRequestAsync((response) =>
            //{
            //    callback(response.MediaList);
            //    errors(response.Errors);
            //});

            GetResponseAsync<GetMediaListResponse>(EncodingQuery.CreateGetMediaListQuery(),
            (response) =>
            {
                callback(response.MediaList);
                errors(response.Errors);
            });
        }

        public virtual T GetResponse<T>(EncodingQuery query) where T : class, new()
        {
            if (UserId == null || UserKey == null)
                throw new EncodingServiceException("UserId or UserKey is empty");

            query.UserId = UserId;
            query.UserKey = UserKey;
            
            string xml = String.Empty;
            try
            {
                xml = GetXmlResponse(Serialize(query));
            }
            catch (WebException ex)
            {
                throw new EncodingServiceException(ex.Message, ex);
            }
            return Deserialize<T>(xml);
        }

        public virtual void GetResponseAsync<T>(EncodingQuery query, Action<T> callback)
            where T : class, new()
        {
            if (UserId == null || UserKey == null)
                throw new EncodingServiceException("UserId or UserKey is empty");

            query.UserId = UserId;
            query.UserKey = UserKey;

            GetXmlResponseAsync(Serialize(query), (xml) =>
            {
                callback(Deserialize<T>(xml));
            });
        }
    }
}
