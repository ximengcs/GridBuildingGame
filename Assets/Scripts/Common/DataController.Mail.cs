using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pt;
using R3;
using SgFramework.Net;
using SgFramework.RedPoint;
using SgFramework.Utility;
using UI;
using UnityEngine;

namespace Common
{
    public partial class DataController
    {
        public static readonly Subject<List<Mail>> MailUpdate = new Subject<List<Mail>>();

        public static void RefreshMailRedPoint()
        {
            RedPointManager.Instance.FindNode("mail").SetValue(Mathf.Max(GetMailNoReadCount(), GetMailNoClaimCount()));

            MailUpdate.OnNext(Archive.MailList.Values.ToList());
        }

        public static List<Mail> GetMailList()
        {
            var list = Archive.MailList.Values.ToList();
            return list;
        }

        public static int GetMailNoReadCount()
        {
            return Archive.MailList.Sum(x => !x.Value.IsRead ? 1 : 0);
        }

        public static int GetMailNoClaimCount()
        {
            return Archive.MailList.Sum(x => x.Value.Rewards.Count > 0 && !x.Value.IsClaimed ? 1 : 0);
        }

        public static void AddNewMail(Mail mail)
        {
            Archive.MailList.Add(mail.Uuid, mail);
            RefreshMailRedPoint();
        }

        public static void DeleteMail(string mailId)
        {
            Archive.MailList.Remove(mailId);
            RefreshMailRedPoint();
        }

        public static void UpdateMail(PushUpdateMail rsp)
        {
            foreach (var mail in rsp.NewMails)
            {
                Archive.MailList.Add(mail.Uuid, mail);
            }

            foreach (var mailId in rsp.DelMailIds)
            {
                Archive.MailList.Remove(mailId);
            }

            RefreshMailRedPoint();
        }

        public static async UniTask MailDeleteAll()
        {
            var list = Archive.MailList.Values
                .Where(x => x.IsRead &&
                            (x.Rewards.Count == 0 || x.Rewards.Count > 0 && x.IsClaimed) ||
                            x.ExpiredAt < SgUtility.Now).ToList();
            if (list.Count == 0)
            {
                UIToast.Instance.ShowToast("no mail to delete").Forget();
                return;
            }

            var msg = new MailDeleteMsg();
            foreach (var mail in list)
            {
                msg.MailIds.Add(mail.Uuid);
            }

            var rsp = await NetManager.Shared.Request(msg);
            if (rsp is not Ok)
            {
                return;
            }

            foreach (var mail in list)
            {
                Archive.MailList.Remove(mail.Uuid);
            }

            RefreshMailRedPoint();
        }

        public static async UniTask MailClaim(string id)
        {
            var rsp = await NetManager.Shared.Request(new MailClaimMsg
            {
                MailIds = { id }
            });
            if (rsp is not Ok)
            {
                return;
            }

            var mail = Archive.MailList[id];
            mail.IsClaimed = true;
            mail.IsRead = true;
            RefreshMailRedPoint();
        }

        public static async UniTask MailClaimAll()
        {
            var list = Archive.MailList.Values.Where(x => x.Rewards.Count != 0 && !x.IsClaimed).ToList();
            if (list.Count == 0)
            {
                UIToast.Instance.ShowToast("no mail to claim").Forget();
                return;
            }

            var msg = new MailClaimMsg();
            foreach (var mail in list)
            {
                msg.MailIds.Add(mail.Uuid);
            }

            var rsp = await NetManager.Shared.Request(msg);
            if (rsp is not Ok)
            {
                return;
            }

            foreach (var mail in list)
            {
                mail.IsClaimed = true;
                mail.IsRead = true;
            }

            RefreshMailRedPoint();
        }

        public static async UniTaskVoid MailRead(string id)
        {
            var rsp = await NetManager.Shared.Request(new MailMarkReadMsg
            {
                MailIds = { id }
            });
            if (rsp is not Ok)
            {
                return;
            }

            var mail = Archive.MailList[id];
            mail.IsRead = true;
            RefreshMailRedPoint();
        }

        public static async UniTask MailReadAll()
        {
            var list = Archive.MailList.Values.Where(x => !x.IsRead).ToList();
            if (list.Count == 0)
            {
                UIToast.Instance.ShowToast("no mail to read").Forget();
                return;
            }

            var msg = new MailMarkReadMsg();
            foreach (var mail in list)
            {
                msg.MailIds.Add(mail.Uuid);
            }

            var rsp = await NetManager.Shared.Request(msg);
            if (rsp is not Ok)
            {
                return;
            }

            foreach (var mail in list)
            {
                mail.IsRead = true;
            }

            RefreshMailRedPoint();
        }
    }
}