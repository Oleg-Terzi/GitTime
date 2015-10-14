using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitTime.Web.Infrastructure.GitHub.Data
{
    [Serializable]
    public class GitHubContext
    {
        #region Properties

        public IDictionary<Int32, GitHubUserInfo> Users { get; private set; }
        public IDictionary<Int32, GitHubIssueInfo> Issues { get; private set; }
        public IDictionary<String, GitHubLabelInfo> Labels { get; private set; }
        public IDictionary<Int32, GitHubRepositoryInfo> Repositories { get; private set; }
        public IDictionary<Int32, GitHubCommentInfo> Comments { get; private set; }

        #endregion

        #region Construction

        public GitHubContext()
        {
            Users = new Dictionary<Int32, GitHubUserInfo>();
            Issues = new Dictionary<Int32, GitHubIssueInfo>();
            Labels = new Dictionary<String, GitHubLabelInfo>();
            Repositories = new Dictionary<Int32, GitHubRepositoryInfo>();
            Comments = new Dictionary<Int32, GitHubCommentInfo>();
        }

        #endregion

        #region Public methods

        public GitHubUserInfo AddUser(GitHubUserInfo user)
        {
            if (user != null)
            {
                if (!Users.ContainsKey(user.ID))
                {
                    Users.Add(user.ID, user);
                    
                    return user;
                }

                return Users[user.ID];
            }

            return null;
        }

        public GitHubIssueInfo AddIssue(GitHubIssueInfo issue)
        {
            if (issue != null)
            {
                if (!Issues.ContainsKey(issue.ID))
                {
                    Issues.Add(issue.ID, issue);

                    return issue;
                }

                return Issues[issue.ID];
            }

            return null;
        }

        public GitHubLabelInfo AddLabel(GitHubLabelInfo label)
        {
            if (label != null)
            {
                if (!Labels.ContainsKey(label.Name))
                {
                    Labels.Add(label.Name, label);

                    return label;
                }

                return Labels[label.Name];
            }

            return null;
        }

        public GitHubRepositoryInfo AddRepository(GitHubRepositoryInfo repository)
        {
            if (repository != null)
            {
                if (!Repositories.ContainsKey(repository.ID))
                {
                    Repositories.Add(repository.ID, repository);

                    return repository;
                }

                return Repositories[repository.ID];
            }

            return null;
        }

        public GitHubCommentInfo AddComment(GitHubCommentInfo comment)
        {
            if (comment != null)
            {
                if (!Comments.ContainsKey(comment.ID))
                {
                    Comments.Add(comment.ID, comment);

                    return comment;
                }

                return Comments[comment.ID];
            }

            return null;
        }

        #endregion
    }
}