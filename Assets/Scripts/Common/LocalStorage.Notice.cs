using System.Collections.Generic;
using Newtonsoft.Json;

namespace Common
{
    public enum ENoticeStatus
    {
        None,
        IsRead
    }

    public partial class LocalStorage
    {
        private const string NoticeKey = "notice_data";
        private static Dictionary<int, ENoticeStatus> _noticeStatus;

        public static ENoticeStatus GetNoticeStatus(int id)
        {
            if (_noticeStatus != null)
            {
                return _noticeStatus.GetValueOrDefault(id);
            }

            var json = GetString(NoticeKey, "{}");
            _noticeStatus = JsonConvert.DeserializeObject<Dictionary<int, ENoticeStatus>>(json);
            return _noticeStatus.GetValueOrDefault(id);
        }

        public static void SetNoticeStatus(int id, ENoticeStatus status)
        {
            _noticeStatus[id] = status;
            var json = JsonConvert.SerializeObject(_noticeStatus);
            SetString(NoticeKey, json);
        }
    }
}