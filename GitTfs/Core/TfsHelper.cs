using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using StructureMap;

namespace Sep.Git.Tfs.Core
{
    public class TfsHelper : ITfsHelper
    {
        private TeamFoundationServer server;
        private string username;

        public string TfsClientLibraryVersion
        {
            // TODO -- fill this in.
            get { return "TODO"; }
        }

        public string Url
        {
            get { return server == null ? null : server.Uri.ToString(); }
            set { SetServer(value, Username); }
        }

        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                SetServer(Url, value);
            }
        }

        private void SetServer(string url, string username)
        {
            if(string.IsNullOrEmpty(url))
            {
                server = null;
            }
            else
            {
                if(string.IsNullOrEmpty(username))
                {
                    server = new TeamFoundationServer(url);
                }
                else
                {
                    server = new TeamFoundationServer(url, MakeCredentials(username));
                }
            }
        }

        private ICredentials MakeCredentials(string username)
        {
            throw new NotImplementedException("TODO: Using a non-default username is not yet supported.");
        }

        private TeamFoundationServer Server
        {
            get
            {
                return server;
            }
        }
        private VersionControlServer VersionControl
        {
            get { return (VersionControlServer)Server.GetService(typeof(VersionControlServer)); }
        }

        public IEnumerable<ITfsChangeset> GetChangesets(string basePath, long firstChangeset)
        {
            var changesets = VersionControl.QueryHistory(basePath, VersionSpec.Latest, 0, RecursionType.Full,
                                        null, new ChangesetVersionSpec((int) firstChangeset), VersionSpec.Latest, int.MaxValue, true,
                                        true, true);
            foreach (Changeset changeset in changesets)
            {
                yield return
                    new TfsChangeset(this, changeset)
                        {
                            Summary =
                                new TfsChangesetInfo()
                                    {ChangesetId = changeset.ChangesetId, TfsSourcePath = basePath, TfsUrl = Url}
                        };
            }
        }
    }
}