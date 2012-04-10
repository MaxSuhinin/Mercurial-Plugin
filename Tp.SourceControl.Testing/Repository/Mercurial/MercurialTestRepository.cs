// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;
using System.Linq;
using System.Text;
using Mercurial;
using StructureMap;
using MercurialSDK = Mercurial;

namespace Tp.SourceControl.Testing.Repository.Mercurial
{
	public class MercurialTestRepository : VcsTestRepository<MercurialTestRepository>
	{
        public MercurialTestRepository()
		{
            ObjectFactory.Configure(x => x.For<MercurialTestRepository>().HybridHttpOrThreadLocalScoped().Use(this));
		}

        private MercurialSDK.Repository _repositiory;
		//private NGit.Api.Git _git;

		private string ClonedRepoFolder
		{
			get { return LocalRepositoryPath + "Cloned"; }
		}

		protected override void OnTestRepositoryDeployed()
		{
			base.OnTestRepositoryDeployed();

            if (Directory.Exists(ClonedRepoFolder))
                Directory.Delete(ClonedRepoFolder, true);

		    Directory.CreateDirectory(ClonedRepoFolder);

            _repositiory = new MercurialSDK.Repository(ClonedRepoFolder);
            _repositiory.Clone(LocalRepositoryPath, new CloneCommand());

            //_git = NGit.Api.Git.CloneRepository()
            //    .SetURI(LocalRepositoryPath)
            //    .SetDirectory(ClonedRepoFolder).Call();

            //BatchingProgressMonitor.ShutdownNow();
		}

		protected override string Name
		{
			get { return "TestRepository"; }
		}

		public override string Login
		{
			get { return "test"; }
		}

		public override string Password
		{
			get { return "test"; }
		}

		public override void Commit(string commitComment)
		{
			Commit("secondFile.txt", "my changed content", commitComment);
		}

		public override string Commit(string filePath, string changedContent, string commitComment)
		{
			using (var file = File.OpenWrite(Path.Combine(ClonedRepoFolder, filePath)))
			{
				var changes = new UTF8Encoding(true).GetBytes(changedContent);
				file.Write(changes, 0, changes.Length);
			}

            _repositiory.Add(new AddCommand().WithPaths(filePath));
		    _repositiory.Commit(commitComment, new CommitCommand());
            _repositiory.Push();

		    var lastChangeset = _repositiory.Log().First();

            return lastChangeset.Revision;
		}

		public override void CheckoutBranch(string branch)
		{
			var fullBranchName = "refs/heads/" + branch;
            //var list = _git.BranchList().Call();
            //var branchExists = list.Any(x => x.GetName() == fullBranchName);
            //if (!branchExists)
            //{
            //    var remoteBranchName = "refs/remotes/origin/" + branch;
            //    _git.BranchCreate().SetStartPoint(remoteBranchName).SetName(branch).Call();
            //}
            //_git.Checkout().SetName(branch).Call();

            //BatchingProgressMonitor.ShutdownNow();
		}

		public override string CherryPick(string revisionId)
		{
            //var result = _git.CherryPick().Include(ObjectId.FromString(revisionId)).Call();
            //_git.Push().Call();

            //BatchingProgressMonitor.ShutdownNow();

            //return result.GetCherryPickedRefs().Select(x => x.GetName()).First();
		    return "";
		}
	}
}