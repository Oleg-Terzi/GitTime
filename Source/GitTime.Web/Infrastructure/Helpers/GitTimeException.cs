using System;
using System.Runtime.Serialization;
using System.Security;

namespace GitTime.Web.Infrastructure.Helpers
{
    [Serializable]
    public class GitTimeException : Exception, ISerializable
    {
        #region Constructors

        public GitTimeException()
        {
        }

        public GitTimeException(String message)
            : base(message)
        {
        }

        public GitTimeException(String message, Exception inner)
            : base(message, inner)
        {
        }

        protected GitTimeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Interface (ISerializable)

        [SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion

        public static GitTimeException Create(String text, params Object[] args)
        {
            var message = CultureHelper.Format(text, args);
            var ex = new GitTimeException(message);
            return ex;
        }

        public static GitTimeException Create(Exception inner, String text, params Object[] args)
        {
            var message = CultureHelper.Format(text, args);
            var ex = new GitTimeException(message, inner);
            return ex;
        }
    }
}