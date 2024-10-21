using System.Reflection;
using Flurl;
using LibGit2Sharp;

namespace Akup.Git;

public class LocalGitRepositoryProvider : IGitRepositoryProvider
{
    static LocalGitRepositoryProvider()
    {
        DisableOwnershipCheckOpts();
    }

    public CommitInfo[] GetCommits(string repositoryPath, string authorName, DateTimeOffset? fromDate, DateTimeOffset? toDate)
    {
        using var repo = new Repository(repositoryPath);
        return FindCommits(repo, authorName, fromDate, toDate)
            .Select(x => new CommitInfo()
            {
                Date = x.Author.When,
                Message = x.Message,
                Url = GetCommitUrl(repo, x),
            })
            .ToArray();
    }

    public string[] GetRepositoriesPaths(string searchLocation)
    {
        return Directory.GetDirectories(searchLocation, ".git", SearchOption.AllDirectories);
    }

    private static Commit[] FindCommits(Repository repo, string? authorName = default, DateTimeOffset? from = default, DateTimeOffset? to = default)
    {
        var query = repo.Commits.AsEnumerable();

        if (!string.IsNullOrEmpty(authorName))
        {
            query = query.Where(x => x.Author.Name == authorName);
        }

        if (from != default)
        {
            query = query.Where(x => x.Author.When >= from);
        }

        if (to != default)
        {
            query = query.Where(x => x.Author.When <= to);
        }

        return query.ToArray();
    }

    private static string GetCommitUrl(Repository repo, Commit commit)
    {
        // Builds GitHub url
        return repo.Network.Remotes.First().Url.Replace(".git", string.Empty).AppendPathSegments("commit", commit.Id);
    }

    // TODO: Replace with library methods once one of these are completed
    // https://github.com/libgit2/libgit2sharp/pull/2118
    // https://github.com/libgit2/libgit2sharp/pull/2042
    // https://github.com/libgit2/libgit2sharp/issues/2036
    private static void DisableOwnershipCheckOpts()
    {
        CallNativeMethod("git_libgit2_opts", 36, 0);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields",
        Justification = "See TODO comment above")]
    private static void CallNativeMethod(string methodName, params object[] args)
    {
        Assembly libGit2SharpAssembly = typeof(Repository).GetTypeInfo().Assembly;
        Type proxyType = libGit2SharpAssembly.GetType("LibGit2Sharp.Core.NativeMethods")!;
        MethodInfo methodInfo = proxyType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static, [typeof(int), typeof(int)])!;
        methodInfo.Invoke(null, args);
    }
}
