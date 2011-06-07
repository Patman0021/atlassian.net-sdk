﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlassian.Jira.Linq;

namespace Atlassian.Jira
{
    /// <summary>
    /// An issue file attachment 
    /// </summary>
    public class Attachment
    {
        private readonly string _author;
        private readonly DateTime? _created;
        private readonly string _fileName;
        private readonly string _mimeType;
        private readonly long? _fileSize;
        private readonly string _id;
        private readonly Jira _jira;

        internal Attachment(Jira jira, RemoteAttachment remoteAttachment)
        {
            _jira = jira;
            _author = remoteAttachment.author;
            _created = remoteAttachment.created;
            _fileName = remoteAttachment.filename;
            _mimeType = remoteAttachment.mimetype;
            _fileSize = remoteAttachment.filesize;
            _id = remoteAttachment.id;
        }

        /// <summary>
        /// Id of attachment
        /// </summary>
        public string Id
        {
            get { return _id; }
        } 

        /// <summary>
        /// Author of attachment (user that uploaded the file)
        /// </summary>
        public string Author
        {
            get { return _author; }
        }

        /// <summary>
        /// Date of creation
        /// </summary>
        public DateTime? Created
        {
            get { return _created; }
        }

        /// <summary>
        /// File name of the attachment
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
        }

        /// <summary>
        /// Mime type
        /// </summary>
        public string MimeType
        {
            get { return _mimeType; }
        }

        /// <summary>
        /// File size
        /// </summary>
        public long? FileSize
        {
            get { return _fileSize; }
        }

        /// <summary>
        /// Downloads attachment to specified file
        /// </summary>
        /// <param name="fullFileName">Full file name where attachment will be downloaded</param>
        public void Download(string fullFileName)
        {
            Download(new WebClientWrapper(), fullFileName);
        }

        /// <summary>
        /// Downloads attachment to specified file
        /// </summary>
        /// <param name="webClient">Implementation of IWebClient used to download.</param>
        /// <param name="fullFileName">Full file name where attachment will be downloaded</param>
        public void Download(IWebClient webClient, string fullFileName)
        {
            webClient.AddQueryString("os_username", _jira.UserName);
            webClient.AddQueryString("os_password", _jira.Password);

            var url = String.Format("{0}secure/attachment/{1}/{2}",
                _jira.Url.EndsWith("/")? _jira.Url: _jira.Url + "/",
                this.Id,
                this.FileName);

            webClient.Download(url, fullFileName);
        }

        
    }
}