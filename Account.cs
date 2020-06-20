using System;
using qsrv.ApiRequests;

namespace qsrv
{
    /// <summary>
    /// Account management object
    /// </summary>
    public class Account
    {
        public AccountInfo AccountInfo { get; set; } = null;
        public bool IsEncrypted { get; set; } = false;
        public string AuthenticationCode { get; set; } = string.Empty;
        public ApiRequestId AuthenticationId { get; set; } = ApiRequestId.Invalid;
        public int AuthenticationTime { get; set; } = -1;
        public string Password { get; set; } = string.Empty;
        public bool IsOnline { get; set; } = false;
        public string Id { get; set; } = string.Empty;

        public Account(AccountInfo info, bool isEncrypted, string id)
        {
            AccountInfo = info;
            IsEncrypted = isEncrypted;
            Id = id;
        }

        public Account()
        {
        }
    }

    [Flags]
    public enum Permission
    {
        INVALID = 0x0,
        NONE = 0x001,
        CREATE_EVENT = 0x002,
        DELETE_USER = 0x004,
        BAN_USER = 0x008,
        EDIT_CONFIG = 0x010,
        QUERY_STATISTICS = 0x020,
        EDIT_MATCHES = 0x040,
        QUERY_USER_INFO = 0x080,
        ADJUST_PRIVILEGES = 0x100,
        IMPERSONATE_USER = 0x200,
        QUERY_EVENT_INFO = 0x400,
        ALL_ACCESS = CREATE_EVENT | DELETE_USER | BAN_USER | EDIT_CONFIG | QUERY_STATISTICS | EDIT_MATCHES | QUERY_USER_INFO | ADJUST_PRIVILEGES | IMPERSONATE_USER | QUERY_EVENT_INFO,
    }
}