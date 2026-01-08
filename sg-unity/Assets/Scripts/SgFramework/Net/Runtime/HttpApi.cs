using System.Collections.Generic;
using Newtonsoft.Json;

namespace SgFramework.Net
{
    public abstract class HttpApi
    {
        public class HttpResp
        {
            [JsonProperty("error_code")] public string ErrorCode;
            [JsonProperty("error_value")] public string ErrorValue;
            [JsonProperty("status")] public bool Status;
        }

        public class LoginSgGuest
        {
            [JsonProperty("uid")] public string Uid;
            [JsonProperty("sign")] public string Sign;
            [JsonProperty("timestamp")] public string Timestamp;
            [JsonProperty("device")] public string Device;
            [JsonProperty("device_os")] public string DeviceOS;
            [JsonProperty("device_id")] public string DeviceID;
            [JsonProperty("version")] public string Version;
            [JsonProperty("pkg_version")] public string PkgVersion;
            [JsonProperty("device_memory")] public string DeviceMemory;
            [JsonProperty("server_id")] public string ServerID; //uuid
            [JsonProperty("chuXinData")] public string ChuXinData;
            [JsonProperty("platForm")] public int PlatForm;
            [JsonProperty("langCode")] public int LangCode;
        }

        public class LoginSgResp : HttpResp
        {
            [JsonProperty("accountId")] public string AccountId; // "MMAccount:1:test",
            [JsonProperty("country")] public string Country; // "0",
            [JsonProperty("createAt")] public string CreateAt; // 1736215021,
            [JsonProperty("curVersion")] public string CurVersion; // "1.0.0",
            [JsonProperty("host")] public string Host; // "192.168.20.222",
            [JsonProperty("loginMark")] public string LoginMark; // "ctu91m5m6cmo7ut4a5fg",
            [JsonProperty("port")] public string Port; // "8000",
            [JsonProperty("roleId")] public string RoleId; // "202431",
            [JsonProperty("serverId")] public string ServerId; // "1",
            [JsonProperty("sessionToken")] public string SessionToken; // "xsCVhpEvvxyFaNyECQHkmBEJYecmOzVp",
            [JsonProperty("uid")] public string Uid; // "test"
        }

        public class ServerConfig
        {
            [JsonProperty("uuid")] public string Uuid; // "1"
            [JsonProperty("name")] public string Name; // "1服"
            [JsonProperty("gate_url")] public string GateURL; // ""
        }

        public class QueryServerListResp : HttpResp
        {
            [JsonProperty("servers")] public List<ServerConfig> Servers = new List<ServerConfig>();
        }

        public class MaintainInfo
        {
            [JsonProperty("mt_time")] public long MtTime { get; set; }
            [JsonProperty("notice")] public NavNotice Notice { get; set; }
        }

        public class NavNotice
        {
            [JsonProperty("id")] public int Id { get; set; }
            [JsonProperty("title")] public string Title { get; set; }
            [JsonProperty("content")] public string Content { get; set; }
            [JsonProperty("start_time")] public long StartTime { get; set; }
            [JsonProperty("end_time")] public long EndTime { get; set; }
            [JsonProperty("status")] public int Status { get; set; }
            [JsonProperty("order")] public int Order { get; set; }
            [JsonProperty("banner")] public string Banner { get; set; }
            [JsonProperty("category")] public int Category { get; set; }
        }
    }
}