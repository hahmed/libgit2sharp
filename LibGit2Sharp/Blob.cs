﻿using System;
using System.IO;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    /// Stores the binary content of a tracked file.
    /// </summary>
    public class Blob : GitObject
    {
        private readonly ILazy<Int64> lazySize;
        private readonly ILazy<bool> lazyIsBinary;

        /// <summary>
        /// Needed for mocking purposes.
        /// </summary>
        protected Blob()
        { }

        internal Blob(Repository repo, ObjectId id)
            : base(repo, id)
        {
            lazySize = GitObjectLazyGroup.Singleton(repo, id, Proxy.git_blob_rawsize);
            lazyIsBinary = GitObjectLazyGroup.Singleton(repo, id, Proxy.git_blob_is_binary);
        }

        /// <summary>
        /// Gets the size in bytes of the raw content of a blob.
        /// <para> Please note that this would load entire blob content in the memory to compute the Size.
        /// In order to read blob size from header, Repository.ObjectMetadata.RetrieveObjectMetadata(Blob.Id).Size
        /// can be used.
        /// </para>
        /// </summary>
        public virtual int Size { get { return (int)lazySize.Value; } }

        /// <summary>
        ///  Determine if the blob content is most certainly binary or not.
        /// </summary>
        public virtual bool IsBinary { get { return lazyIsBinary.Value; } }

        /// <summary>
        /// Gets the blob content in a <see cref="Stream"/>.
        /// </summary>
        public virtual Stream GetContentStream()
        {
            return Proxy.git_blob_rawcontent_stream(repo.Handle, Id, Size);
        }

        /// <summary>
        /// Gets the blob content in a <see cref="Stream"/> as it would be
        /// checked out to the working directory.
        /// <param name="filteringOptions">Parameter controlling content filtering behavior</param>
        /// </summary>
        public virtual Stream GetContentStream(FilteringOptions filteringOptions)
        {
            Ensure.ArgumentNotNull(filteringOptions, "filteringOptions");
            return Proxy.git_blob_filtered_content_stream(repo.Handle, Id, filteringOptions.HintPath, false);
        }
    }
}
