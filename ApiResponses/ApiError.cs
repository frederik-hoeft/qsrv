﻿using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using qsrv.ApiRequests;

namespace qsrv.ApiResponses
{
    public sealed class ApiError
    {
        public readonly ApiErrorCode ApiErrorCode;
        public readonly ApiRequestId OriginalRequestId;
        public readonly string Message;
        public readonly TargetSite TargetSite;

        private ApiError(ApiErrorCode apiErrorCode, ApiRequestId originalRequestId, string message, TargetSite targetSite)
        {
            ApiErrorCode = apiErrorCode;
            OriginalRequestId = originalRequestId;
            Message = message;
            TargetSite = targetSite;
        }

        public static ApiError Create(ApiErrorCode errorCode, ApiRequestId requestId, string message)
        {
            return new ApiError(errorCode, requestId, message, null);
        }

        public static ApiError Create(ApiErrorCode errorCode, ApiRequestId requestId, string message, TargetSite targetSite)
        {
            return new ApiError(errorCode, requestId, message, targetSite);
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static void Throw(ApiErrorCode errorCode, ApiServer server, string message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            TargetSite targetSite = null;
            if (MainServer.Config.DebuggingEnabled)
            {
                targetSite = new TargetSite(memberName, sourceFilePath, sourceLineNumber);
            }
            ApiError apiError = new ApiError(errorCode, server.RequestId, message, targetSite);
            string json = apiError.Serialize();
            Debug.WriteLine("xx " + json.Replace("\\\\", "\\"));
            if (server == null)
            {
                return;
            }
            SerializedApiResponse apiResponse = SerializedApiResponse.Create(ResponseId.Error, json);
            server.Send(apiResponse.Serialize());
            server.UnitTesting.MethodSuccess = false;
            server.UnitTesting.ErrorCode = errorCode;
        }
    }

    public enum ApiErrorCode
    {
        Ok = 200,
        InvalidUser = 401,
        Forbidden = 403,
        NotFound = 404,
        InvalidArgument = 422,
        InternalServerError = 500,
        DatabaseException = 503,
    }
}