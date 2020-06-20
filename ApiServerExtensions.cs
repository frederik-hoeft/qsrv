using qsrv.ApiRequests;
using qsrv.ApiResponses;
using qsrv.Database;

namespace qsrv
{
    public static class ApiServerExtensions
    {
        /// <summary>
        /// Throws an exception if the EventInfo is null and returns false otherwise.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="eventInfo"></param>
        /// <returns></returns>
        public static bool AssertEventInfoNotNull(this ApiServer server, EventInfo eventInfo)
        {
            if (eventInfo == null)
            {
                ApiError.Throw(ApiErrorCode.InvalidArgument, server, "Invalid argument: EventInfo was null.");
                return true;
            }
            return false;
        }

        /// <summary>
        /// returns false if everything is ok.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool AssertServerSetup(this ApiServer server, ApiRequest request)
        {
            if (server == null)
            {
                return true;
            }
            server.RequestId = request.RequestId;
            return false;
        }

        /// <summary>
        /// Throws an exception if the account is not null and returns false otherwise.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static bool AssertAccountNull(this ApiServer server)
        {
            if (server.Account != null)
            {
                ApiError.Throw(ApiErrorCode.InvalidContext, server, "The requested action is invalid in this context.");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Throws an exception if the account is null and returns false otherwise.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static bool AssertAccountNotNull(this ApiServer server)
        {
            if (server.Account == null)
            {
                ApiError.Throw(ApiErrorCode.InvalidContext, server, "The requested action is invalid in this context.");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Throws an exception if the userid is not set and returns false otherwise.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static bool AssertUserIdSet(this ApiServer server)
        {
            if (server.AssertAccountInfoNotNull())
            {
                return true;
            }
            if (string.IsNullOrEmpty(server.Account.AccountInfo.UserId))
            {
                ApiError.Throw(ApiErrorCode.InvalidContext, server, "The requested action is invalid in this context: userid was null");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Throws an exception if the code is Invalid and returns false otherwise.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool AssertAuthenticationCodeInvalid(this ApiServer server, string code)
        {
            // TODO: Implement max retry count.
            if (server.AssertAccountNotNull())
            {
                return true;
            }
            if (string.IsNullOrEmpty(server.Account.AuthenticationCode))
            {
                ApiError.Throw(ApiErrorCode.InvalidContext, server, "The requested action is invalid in this context: server unaware of authentication event.");
                return true;
            }
            if (!code.Equals(server.Account.AuthenticationCode))
            {
                ApiError.Throw(ApiErrorCode.InvalidCode, server, "The provided authentication code was incorrect.");
                return true;
            }
            if (server.Account.AuthenticationId != server.RequestId)
            {
                ApiError.Throw(ApiErrorCode.InvalidContext, server, "The requested action is invalid in this context: authentication event does not match.");
                return true;
            }
            if (server.Account.AuthenticationTime + MainServer.Config.WamsrvSecurityConfig.TwoFactorExpirationTime < DatabaseEssentials.GetTimeStamp())
            {
                ApiError.Throw(ApiErrorCode.ExpiredCode, server, "The provided authentication code has expired.");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Throws an exception if the user is online and returns false otherwise.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static bool AssertUserOffline(this ApiServer server)
        {
            if (server.AssertAccountNotNull())
            {
                return true;
            }
            if (server.Account.IsOnline)
            {
                ApiError.Throw(ApiErrorCode.InvalidContext, server, "The requested action is invalid in this context: already logged in.");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Throws an exception if the user is offline and returns false otherwise.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static bool AssertUserOnline(this ApiServer server)
        {
            if (server.AssertAccountNotNull())
            {
                return true;
            }
            if (!server.Account.IsOnline)
            {
                ApiError.Throw(ApiErrorCode.InvalidContext, server, "The requested action is invalid in this context: not logged in.");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Throws an exception if the account info is null and returns false otherwise.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static bool AssertAccountInfoNotNull(this ApiServer server)
        {
            if (server.AssertAccountNotNull())
            {
                return true;
            }
            if (server.Account.AccountInfo == null)
            {
                ApiError.Throw(ApiErrorCode.InvalidState, server, "An unexpected error occurred: AccountInfo was null.");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Throws an exception if the id is not set and returns false otherwise.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static bool AssertIdSet(this ApiServer server)
        {
            if (server.AssertAccountNotNull())
            {
                return true;
            }
            if (string.IsNullOrEmpty(server.Account.Id))
            {
                ApiError.Throw(ApiErrorCode.InvalidContext, server, "The requested action is invalid in this context: internal user id not set.");
                return true;
            }
            return false;
        }

        public static bool AssertEmailSet(this ApiServer server)
        {
            if (server.AssertAccountNotNull() || server.AssertAccountInfoNotNull())
            {
                return true;
            }
            if (string.IsNullOrEmpty(server.Account.AccountInfo.Email))
            {
                ApiError.Throw(ApiErrorCode.InvalidState, server, "An unexpected error occurred: Email not set.");
                return true;
            }
            return false;
        }

        public static bool AssertPasswordSet(this ApiServer server)
        {
            if (server.AssertAccountNotNull())
            {
                return true;
            }
            if (string.IsNullOrEmpty(server.Account.Password))
            {
                ApiError.Throw(ApiErrorCode.InvalidState, server, "An unexpected error occurred: Password not set.");
                return true;
            }
            return false;
        }
    }
}