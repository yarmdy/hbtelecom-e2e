using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTCCGoods.Controllers
{
    public class EnumDescriptionAttribute : Attribute
    {
        private string m_strDescription;
        public EnumDescriptionAttribute(string strPrinterName)
        {
            m_strDescription = strPrinterName;
        }

        public string Description
        {
            get { return m_strDescription; }
        }
    }
    /// <summary>
    /// 借货单状态枚举
    /// </summary>
    public enum orderstatus {
        /// <summary>
        /// 新建
        /// </summary>
        [EnumDescription("新建")]
        create=0,
        /// <summary>
        /// 提交
        /// </summary>
        [EnumDescription("提交")]
        submit=1,
        /// <summary>
        /// 省公司审核
        /// </summary>
        [EnumDescription("省公司审核")]
        sgsverify=2,
        /// <summary>
        /// 厂家确认
        /// </summary>
        [EnumDescription("厂家确认")]
        cjconfirm=3,
        /// <summary>
        /// 设计院审核
        /// </summary>
        [EnumDescription("设计院审核")]
        sjyverify=4,
        /// <summary>
        /// 厂家发货
        /// </summary>
        [EnumDescription("厂家发货")]
        cjsend=5,
        /// <summary>
        /// 厂家督导收货
        /// </summary>
        [EnumDescription("厂家督导收货")]
        cjreceive=6,
        /// <summary>
        /// 地市确认收货
        /// </summary>
        [EnumDescription("地市确认收货")]
        dsreceive=7,
        /// <summary>
        /// 单据完成
        /// </summary>
        [EnumDescription("单据完成")]
        complete=8,
        /// <summary>
        /// 单据关闭
        /// </summary>
        [EnumDescription("单据关闭")]
        close=-1,
        [EnumDescription("已作废")]
        bevoid=-2
    }
    /// <summary>
    /// 角色枚举
    /// </summary>
    public enum crole {
        /// <summary>
        /// 管理员
        /// </summary>
        [EnumDescription("管理员")]
        admin = 0,
        /// <summary>
        /// 地市
        /// </summary>
        [EnumDescription("地市")]
        city = 1,
        /// <summary>
        /// 设计院
        /// </summary>
        [EnumDescription("设计院")]
        design = 2,
        /// <summary>
        /// 厂家
        /// </summary>
        [EnumDescription("厂家")]
        manufactor = 3,

        [EnumDescription("督导")]
        supervisor = 4
    }
    /// <summary>
    /// 审批状态
    /// </summary>
    public enum verifystatus { 
        /// <summary>
        /// 同意
        /// </summary>
        [EnumDescription("同意")]
        agree = 1,
        /// <summary>
        /// 不同意
        /// </summary>
        [EnumDescription("不同意")]
        disagree = 0
    }
    /// <summary>
    /// 借货单model
    /// </summary>
    public class corder
    {
        /// <summary>
        /// id
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 审批流id 暂时没用
        /// </summary>
        public int? vid { get; set; }
        /// <summary>
        /// 创建人id
        /// </summary>
        public int? createuid { get; set; }
        /// <summary>
        /// 创建人名
        /// </summary>
        public string createuname { get; set; }
        public int? wid { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createtime { get; set; }
        /// <summary>
        /// 厂家id
        /// </summary>
        public int? receiveuid { get; set; }
        /// <summary>
        /// 厂家名
        /// </summary>
        public string receiveuname { get; set; }
        /// <summary>
        /// 单据状态
        /// </summary>
        public orderstatus? status { get; set; }
        /// <summary>
        /// 最后一次操作时间
        /// </summary>
        public DateTime? statustime { get; set; }
        /// <summary>
        /// 审批流步骤号 暂时没用
        /// </summary>
        public int? verifyno { get; set; }
        /// <summary>
        /// 审批流角色 暂时没用
        /// </summary>
        public crole? verifyrole { get; set; }
        /// <summary>
        /// 借货明细
        /// </summary>
        public Dictionary<int,corder_goods> goods { get; set; }
        /// <summary>
        /// 审批明细
        /// </summary>
        public Dictionary<int,corder_verifyflow> verifies { get; set; }

    }
    /// <summary>
    /// 借货明细
    /// </summary>
    public class corder_goods {
        /// <summary>
        /// 借货明细id
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// 借货单id
        /// </summary>
        public int? oid { get; set; }
        /// <summary>
        /// 产品id
        /// </summary>
        public int? gid { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string gname { get; set; }
        /// <summary>
        /// 借货数量
        /// </summary>
        public int? gnum { get; set; }
    }
    /// <summary>
    /// 借货单审批明细
    /// </summary>
    public class corder_verifyflow {
        /// <summary>
        /// 审批明细id
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// 借货单id
        /// </summary>
        public int? oid { get; set; }
        /// <summary>
        /// 审批人id
        /// </summary>
        public int? uid { get; set; }
        /// <summary>
        /// 审批人名
        /// </summary>
        public string uname { get; set; }
        public string ucontacts { get; set; }
        public string utel { get; set; }
        /// <summary>
        /// 审批类型（订单状态枚举）
        /// </summary>
        public orderstatus? verifyno { get; set; }
        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? createtime { get; set; }
        /// <summary>
        /// 审批状态（同意，不同意）
        /// </summary>
        public verifystatus? status { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        public string des { get; set; }
        /// <summary>
        /// 结束时间 暂时没用
        /// </summary>
        public DateTime? endtime { get; set; }

        /// <summary>
        /// 审批连带发货明细
        /// </summary>
        public Dictionary<int,corder_sendgoods> goods { get; set; }
        /// <summary>
        /// 审批连带附件
        /// </summary>
        public Dictionary<int,cattachment> attachments { get; set; }

        public Dictionary<int, corder_verifyflow> verifies { get; set; }

        public int? duid { get; set; }
        public string duname { get; set; }
        public orderstatus? localstatus { get; set; }
        public string sendname { get; set; }
        public DateTime? plantime { get; set; }
    }
    /// <summary>
    /// 发货明细
    /// </summary>
    public class corder_sendgoods {
        /// <summary>
        /// 发货明细id
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// 审批明细id
        /// </summary>
        public int? ovid { get; set; }
        /// <summary>
        /// 产品id
        /// </summary>
        public int? gid { get; set; }
        /// <summary>
        /// 产品名
        /// </summary>
        public string gname { get; set; }
        /// <summary>
        /// 发货数量
        /// </summary>
        public int? gnum { get; set; }
        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? sendtime { get; set; }
        /// <summary>
        /// 确认收货时间
        /// </summary>
        public DateTime? receivetime { get; set; }
    }
    /// <summary>
    /// 附件明细
    /// </summary>
    public class cattachment {
        /// <summary>
        /// 附件明细id
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// 审批明细id
        /// </summary>
        public int? ovid { get; set; }
        /// <summary>
        /// 附件名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 附件地址
        /// </summary>
        public string url { get; set; }
    }

    /// <summary>
    /// 发货单操作类
    /// </summary>
    public class corderfactory
    {
        #region 构造函数
        /// <summary>
        /// 调用后插入数据库并返回数据到属性order
        /// </summary>
        /// <param name="o">借货单model</param>
        /// <param name="gs">借货明细</param>
        public corderfactory(corder o, List<corder_goods> gs)
        {
            if (o == null || gs == null || gs.Count <= 0)
            {
                throw new Exception("参数不能为null");
            }
            DateTime ctime = DateTime.Now;
            if (!o.createuid.HasValue
                || !o.receiveuid.HasValue)
            {
                throw new Exception("必填项不完整");
            }
            foreach (var g in gs)
            {
                if (!g.gid.HasValue || !g.gnum.HasValue)
                {
                    throw new Exception("必填项不完整");
                }
            }
            if (gs.Count != gs.Select(a => a.gid.Value).Distinct().Count())
            {
                throw new Exception("借货产品重复");
            }
            DB db = new DB();
            try
            {
                var sqlinsertorder = string.Format("INSERT INTO corder(code,vid,createuid,createtime,receiveuid,status,statustime,verifyno,verifyrole)VALUES('{0}',null,{1},'{2}',{3},{4},'{5}',null,null)", ctime.ToString("yyyyMMddHHmmss") + ctime.Millisecond.ToString().PadLeft(3, '0'), o.createuid, ctime.ToString("yyyy-MM-dd HH:mm:ss"), o.receiveuid, (int)orderstatus.create, ctime.ToString("yyyy-MM-dd HH:mm:ss"));
                var oid = db.Insertobj(sqlinsertorder);
                foreach (var g in gs)
                {
                    db.Execobj("insert into corder_goods(oid,gid,gnum)values(" + oid + "," + g.gid + "," + g.gnum + ")");
                }
                db.End(true);
                load(oid);
            }
            catch (Exception ex)
            {
                db.End(false);
                throw ex;
            }
        }
        /// <summary>
        /// 调用后根据id查询结果返回到属性order
        /// </summary>
        /// <param name="id"></param>
        public corderfactory(int id)
        {
            load(id);
        }
        #endregion
        #region 私有
        #region 成员
        /// <summary>
        /// 借货单私有成员
        /// </summary>
        private corder _order;
        #endregion
        #region 方法
        /// <summary>
        /// 根据id加载借货单并返回属性order
        /// </summary>
        /// <param name="id">借货单id</param>
        private void load(int id)
        {
            var corderdic = DB.QueryOne(@"select o.id,o.code,o.vid,o.createuid,cu.name createuname,cu.wid,o.createtime,o.receiveuid,ru.name receiveuname,o.status,o.statustime,o.verifyno,o.verifyrole 
from corder o
left join cuser cu on o.createuid=cu.id
left join cuser ru on o.receiveuid=ru.id
where o.id=" + id);
            if (corderdic == null)
            {
                throw new Exception("单据不存在");
            }
            _order = new corder();
            _order.id = O2.O2I(corderdic["id"]);
            _order.code = corderdic["code"].ToString();
            _order.vid = O2.O2I(corderdic["vid"]);
            _order.createuid = O2.O2I(corderdic["createuid"]);
            _order.createuname = corderdic["createuname"].ToString();
            _order.wid = O2.O2I(corderdic["wid"]);
            _order.createtime = O2.O2DT(corderdic["createtime"]);
            _order.receiveuid = O2.O2I(corderdic["receiveuid"]);
            _order.receiveuname = corderdic["receiveuname"].ToString();
            _order.status = (orderstatus)O2.O2I(corderdic["status"]);
            _order.statustime = O2.O2DT(corderdic["statustime"]);
            _order.verifyno = O2.O2I(corderdic["verifyno"]);
            _order.verifyrole = (crole)O2.O2I(corderdic["verifyrole"]);

            _order.goods = new Dictionary<int, corder_goods>();
            var goodsdics = DB.QueryAsDics("select og.id,og.oid,og.gid,g.name gname,og.gnum from corder_goods og left join cgoods g on og.gid=g.id where oid=" + id+" order by og.id");
            if (goodsdics != null)
            {
                foreach (var dic in goodsdics)
                {
                    var good = new corder_goods();
                    good.id = O2.O2I(dic["id"]);
                    good.oid = O2.O2I(dic["oid"]);
                    good.gid = O2.O2I(dic["gid"]);
                    good.gname = dic["gname"].ToString();
                    good.gnum = O2.O2I(dic["gnum"]);
                    order.goods[good.id.Value] = good;
                }
            }

            order.verifies = new Dictionary<int, corder_verifyflow>();
            var verifiesdics = DB.QueryAsDics("select ov.id,ov.oid,ov.uid,u.name uname,u.contacts ucontacts,u.tel utel,ov.verifyno,ov.createtime,ov.status,ov.des,ov.endtime,ov.ovid,ov.duid,du.name duname,ov.localstatus,ov.sendname,ov.plantime from corder_verifyflow ov left join cuser u on ov.uid=u.id left join cuser du on ov.duid=du.id where ov.oid=" + id + " and ov.verifyno<=" + ((int)orderstatus.cjsend) + " order by ov.id");
            if (verifiesdics != null)
            {
                foreach (var dic in verifiesdics)
                {
                    var verify = new corder_verifyflow();
                    verify.id = O2.O2I(dic["id"]);
                    verify.oid = O2.O2I(dic["oid"]);
                    verify.uid = O2.O2I(dic["uid"]);
                    verify.uname = dic["uname"].ToString();
                    verify.ucontacts = dic["ucontacts"].ToString();
                    verify.utel = dic["utel"].ToString();
                    verify.verifyno = (orderstatus)O2.O2I(dic["verifyno"]);
                    verify.createtime = O2.O2DT(dic["createtime"]);
                    verify.status = (verifystatus)O2.O2I(dic["status"]);
                    verify.des = dic["des"].ToString();
                    verify.endtime = O2.O2DT(dic["endtime"]);
                    verify.duid = O2.O2I(dic["duid"]);
                    verify.duname = dic["duname"].ToString();
                    verify.localstatus = (orderstatus)O2.O2I(dic["localstatus"]);
                    verify.sendname = dic["sendname"].ToString();
                    verify.plantime = O2.O2DT(dic["plantime"]);

                    verify.goods = new Dictionary<int, corder_sendgoods>();
                    var sendgoodsdics = DB.QueryAsDics("SELECT osg.id,osg.ovid,osg.gid,g.name gname,osg.gnum,osg.sendtime,osg.receivetime FROM corder_sendgoods osg left join cgoods g on osg.gid=g.id where osg.ovid=" + verify.id + " order by osg.id");
                    if (sendgoodsdics != null)
                    {
                        foreach (var dic2 in sendgoodsdics)
                        {
                            var sendgood = new corder_sendgoods();
                            sendgood.id = O2.O2I(dic2["id"]);
                            sendgood.ovid = O2.O2I(dic2["ovid"]);
                            sendgood.gid = O2.O2I(dic2["gid"]);
                            sendgood.gname = dic2["gname"].ToString();
                            sendgood.gnum = O2.O2I(dic2["gnum"]);
                            sendgood.sendtime = O2.O2DT(dic2["sendtime"]);
                            sendgood.receivetime = dic2["receivetime"]==DBNull.Value?(DateTime?)null: O2.O2DT(dic2["receivetime"]);
                            verify.goods[sendgood.id.Value] = sendgood;
                        }
                    }

                    verify.attachments = new Dictionary<int, cattachment>();
                    var attachmentsdics = DB.QueryAsDics("SELECT a.id,a.ovid,a.name,a.url FROM cattachment a where ovid=" + verify.id+" order by a.id");
                    if (attachmentsdics != null)
                    {
                        foreach (var dic2 in attachmentsdics)
                        {
                            var attachment = new cattachment();
                            attachment.id = O2.O2I(dic2["id"]);
                            attachment.ovid = O2.O2I(dic2["ovid"]);
                            attachment.name = dic2["name"].ToString();
                            attachment.url = dic2["url"].ToString();
                            verify.attachments[attachment.id.Value] = attachment;
                        }
                    }
                    verify.verifies = new Dictionary<int, corder_verifyflow>();
                    if (verify.verifyno == orderstatus.cjsend)
                    {
                        var vverifiesdics = DB.QueryAsDics("select ov.id,ov.oid,ov.uid,u.name uname,u.contacts ucontacts,u.tel utel,ov.verifyno,ov.createtime,ov.status,ov.des,ov.endtime from corder_verifyflow ov left join cuser u on ov.uid=u.id where ov.ovid=" + verify.id + " order by ov.id");
                        if (vverifiesdics != null)
                        {
                            foreach (var dic2 in vverifiesdics)
                            {
                                var vverify = new corder_verifyflow();
                                vverify.id = O2.O2I(dic2["id"]);
                                vverify.oid = O2.O2I(dic2["oid"]);
                                vverify.uid = O2.O2I(dic2["uid"]);
                                vverify.uname = dic2["uname"].ToString();
                                vverify.ucontacts = dic2["ucontacts"].ToString();
                                vverify.utel = dic2["utel"].ToString();
                                vverify.verifyno = (orderstatus)O2.O2I(dic2["verifyno"]);
                                vverify.createtime = O2.O2DT(dic2["createtime"]);
                                vverify.status = (verifystatus)O2.O2I(dic2["status"]);
                                vverify.des = dic2["des"].ToString();
                                vverify.endtime = O2.O2DT(dic2["endtime"]);
                                //vverify.duid = O2.O2I(dic2["duid"]);
                                //vverify.duname = dic2["duname"].ToString();
                                //vverify.localstatus = (orderstatus)O2.O2I(dic2["localstatus"]);
                                //vverify.sendname = dic2["sendname"].ToString();
                                //vverify.plantime = O2.O2DT(dic2["plantime"]);

                                vverify.goods = new Dictionary<int, corder_sendgoods>();
                                var vsendgoodsdics = DB.QueryAsDics("SELECT osg.id,osg.ovid,osg.gid,g.name gname,osg.gnum,osg.sendtime,osg.receivetime FROM corder_sendgoods osg left join cgoods g on osg.gid=g.id where osg.ovid=" + vverify.id + " order by osg.id");
                                if (vsendgoodsdics != null)
                                {
                                    foreach (var dic3 in vsendgoodsdics)
                                    {
                                        var sendgood = new corder_sendgoods();
                                        sendgood.id = O2.O2I(dic3["id"]);
                                        sendgood.ovid = O2.O2I(dic3["ovid"]);
                                        sendgood.gid = O2.O2I(dic3["gid"]);
                                        sendgood.gname = dic3["gname"].ToString();
                                        sendgood.gnum = O2.O2I(dic3["gnum"]);
                                        sendgood.sendtime = O2.O2DT(dic3["sendtime"]);
                                        sendgood.receivetime = dic3["receivetime"] == DBNull.Value ? (DateTime?)null : O2.O2DT(dic2["receivetime"]);
                                        vverify.goods[sendgood.id.Value] = sendgood;
                                    }
                                }

                                vverify.attachments = new Dictionary<int, cattachment>();
                                var vattachmentsdics = DB.QueryAsDics("SELECT a.id,a.ovid,a.name,a.url FROM cattachment a where ovid=" + vverify.id + " order by a.id");
                                if (vattachmentsdics != null)
                                {
                                    foreach (var dic3 in vattachmentsdics)
                                    {
                                        var attachment = new cattachment();
                                        attachment.id = O2.O2I(dic3["id"]);
                                        attachment.ovid = O2.O2I(dic3["ovid"]);
                                        attachment.name = dic3["name"].ToString();
                                        attachment.url = dic3["url"].ToString();
                                        vverify.attachments[attachment.id.Value] = attachment;
                                    }
                                }
                                vverify.verifies = new Dictionary<int, corder_verifyflow>();
                                verify.verifies[vverify.id.Value] = vverify;
                            }
                        }
                    }
                    order.verifies[verify.id.Value] = verify;
                }
            }
        }
        #endregion
        #endregion
        #region 公共
        #region 属性
        /// <summary>
        /// 借货单model
        /// </summary>
        public corder order { get { return _order; } }
        #endregion
        #region 方法
        /// <summary>
        /// 修改借货单和借货明细
        /// </summary>
        /// <returns>成功失败</returns>
        public bool update()
        {
            var res = false;
            DB db = new DB();
            try
            {
                if (_order.status.Value != orderstatus.create)
                {
                    throw new Exception("非新建状态，无法修改");
                }
                db.Execobj("update corder set receiveuid=" + _order.receiveuid + " where id=" + _order.id);
                var goodsdics = DB.QueryAsDics("select * from corder_goods where oid=" + _order.id);
                foreach (var dic in goodsdics)
                {
                    if (_order.goods.ContainsKey(O2.O2I(dic["id"])))
                    {
                        var tgood = _order.goods[O2.O2I(dic["id"])];
                        db.Execobj("update corder_goods set gid=" + tgood.gid + ",gnum=" + tgood.gnum + " where id=" + dic["id"]);
                    }
                    else
                    {
                        db.Execobj("delete corder_goods where id=" + dic["id"]);
                    }
                }
                foreach (var dic in _order.goods)
                {
                    if (!dic.Value.id.HasValue)
                    {
                        db.Execobj("insert into corder_goods(oid,gid,gnum)values(" + _order.id + "," + dic.Value.gid + "," + dic.Value.gnum + ")");
                    }
                }

                db.End(true);

            }
            catch (Exception ex)
            {
                db.End(false);
                throw ex;
            }
            load(_order.id.Value);
            res = true;
            return res;
        }
        /// <summary>
        /// 提交借货单
        /// </summary>
        /// <param name="uid">提交人id</param>
        /// <returns>成功失败</returns>
        public bool submit(int uid, List<cattachment> attachments=null)
        {
            var res = false;
            var db = new DB();
            try
            {
                if (_order.status.Value != orderstatus.create)
                {
                    throw new Exception("非新建状态，无法提交");
                }
                if (attachments == null || attachments.Count() <= 0) {
                    throw new Exception("提交必须上传附件。");
                }
                var ctime = DateTime.Now;
                db.Execobj("update corder set status=" + ((int)orderstatus.submit) + ",statustime='" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "' where id=" + _order.id);
                var ovid= db.Insertobj("insert into corder_verifyflow(oid,uid,verifyno,createtime,status,des,endtime)values(" + _order.id + "," + uid + "," + ((int)orderstatus.submit) + ",'" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "'," + ((int)verifystatus.agree) + ",null,null)");
                if (attachments != null)
                {
                    foreach (var atta in attachments)
                    {
                        db.Execobj("insert into cattachment(ovid,name,url)values(" + ovid + ",'" + atta.name + "','" + atta.url + "')");
                    }
                }
                db.End(true);
            }
            catch (Exception ex)
            {
                db.End(false);
                throw ex;
            }
            load(_order.id.Value);
            res = true;
            return res;
        }
        /// <summary>
        /// 省公司审核
        /// </summary>
        /// <param name="uid">审核人id</param>
        /// <param name="agree">同意不同意</param>
        /// <param name="desc">审批意见</param>
        /// <returns>成功失败</returns>
        public bool sgsvarify(int uid, bool agree, string desc)
        {
            var res = false;
            var db = new DB();
            try
            {
                if (_order.status.Value != orderstatus.submit)
                {
                    throw new Exception("非提交状态，无法审核");
                }
                var ctime = DateTime.Now;
                db.Execobj("update corder set status=" + (agree ? (int)orderstatus.sgsverify : (int)orderstatus.create) + ",statustime='" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "' where id=" + _order.id);
                db.Execobj("insert into corder_verifyflow(oid,uid,verifyno,createtime,status,des,endtime)values(" + _order.id + "," + uid + "," + ((int)orderstatus.sgsverify) + ",'" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "'," + (agree ? (int)verifystatus.agree : (int)verifystatus.disagree) + ",'" + desc + "',null)");
                db.End(true);
            }
            catch (Exception ex)
            {
                db.End(false);
                throw ex;
            }
            load(_order.id.Value);
            res = true;
            return res;
        }
        /// <summary>
        /// 厂家确认
        /// </summary>
        /// <param name="uid">确认人id</param>
        /// <param name="agree">同意不同意</param>
        /// <param name="desc">审批意见</param>
        /// <param name="attachments">附件</param>
        /// <returns>成功失败</returns>
        public bool cjconfirm(int uid, bool agree, string desc, List<cattachment> attachments)
        {
            var res = false;
            var db = new DB();
            try
            {
                if (_order.status.Value != orderstatus.sgsverify)
                {
                    throw new Exception("非省公司审核状态，无法确认");
                }
                //if (attachments == null || attachments.Count() <= 0)
                //{
                //    throw new Exception("确认必须上传附件。");
                //}
                var ctime = DateTime.Now;
                db.Execobj("update corder set status=" + (agree ? (int)orderstatus.cjconfirm : (int)orderstatus.create) + ",statustime='" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "' where id=" + _order.id);
                var ovid = db.Insertobj("insert into corder_verifyflow(oid,uid,verifyno,createtime,status,des,endtime)values(" + _order.id + "," + uid + "," + ((int)orderstatus.cjconfirm) + ",'" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "'," + (agree ? (int)verifystatus.agree : (int)verifystatus.disagree) + ",'" + desc + "',null)");
                if (attachments != null)
                {
                    foreach (var atta in attachments)
                    {
                        db.Execobj("insert into cattachment(ovid,name,url)values(" + ovid + ",'" + atta.name + "','" + atta.url + "')");
                    }
                }
                db.End(true);
            }
            catch (Exception ex)
            {
                db.End(false);
                throw ex;
            }
            load(_order.id.Value);
            res = true;
            return res;
        }
        /// <summary>
        /// 设计院审核
        /// </summary>
        /// <param name="uid">审核人id</param>
        /// <param name="agree">同意不同意</param>
        /// <param name="desc">审批意见</param>
        /// <returns>成功失败</returns>
        public bool sjyvarify(int uid, bool agree, string desc, int wid)
        {
            var res = false;
            var db = new DB();
            try
            {
                if (_order.status.Value != orderstatus.cjconfirm)
                {
                    throw new Exception("非厂家确认状态，无法审核");
                }
                var ctime = DateTime.Now;
                db.Execobj("update corder set status=" + (agree ? (int)orderstatus.sjyverify : (int)orderstatus.sgsverify) + ",statustime='" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "' where id=" + _order.id);
                db.Execobj("insert into corder_verifyflow(oid,uid,verifyno,createtime,status,des,endtime)values(" + _order.id + "," + uid + "," + ((int)orderstatus.sjyverify) + ",'" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "'," + (agree ? (int)verifystatus.agree : (int)verifystatus.disagree) + ",'" + desc + "',null)");
                foreach (var good in _order.goods) {
                    string sql = "insert into cstockio (wid,gid,ionumber,purchased,createtime,uid,stype,oid,require) values ("+wid+","+good.Value.gid+",0,0,'"+ ctime.ToString("yyyy-MM-dd HH:mm:ss") + "',"+uid+",0,"+ _order.id+ ", "+ good.Value.gnum + ")";
                    db.Execobj(sql);
                    var result = db.QueryAsDicsobj("select id from cstock where wid = "+wid+" and gid = "+ good.Value.gid);
                    if (result == null) {
                        sql = "insert into cstock (wid,gid,stock,purchased,require) values ("+wid+","+good.Value.gid+",0,0,"+good.Value.gnum+")";
                    }else
                    {
                        var sid = O2.O2I(result[0]["id"]);
                        sql = "update cstock set require = require + "+good.Value.gnum+" where id = " + sid;
                    }
                    db.Execobj(sql);
                }
                db.End(true);
            }
            catch (Exception ex)
            {
                db.End(false);
                throw ex;
            }
            load(_order.id.Value);
            res = true;
            return res;
        }
        /// <summary>
        /// 厂家发货
        /// </summary>
        /// <param name="uid">发货人id</param>
        /// <param name="desc">审批意见</param>
        /// <param name="sendgoods">发货明细</param>
        /// <param name="attachments">附件</param>
        /// <returns>成功失败</returns>
        public bool cjsend(int uid, string desc, List<corder_sendgoods> sendgoods, List<cattachment> attachments,int duid,string sendname,DateTime? plantime)
        {
            var res = false;
            var db = new DB();
            try
            {
                if (_order.status.Value != orderstatus.sjyverify && _order.status.Value != orderstatus.cjsend && _order.status.Value != orderstatus.cjreceive)
                {
                    throw new Exception("当前状态，无法发货（需要状态，" + O2.GED(orderstatus.sjyverify) + "、" + O2.GED(orderstatus.cjsend) + "、" + O2.GED(orderstatus.cjreceive) + "）");
                }
                if (duid == -1) {
                    throw new Exception("没有督导信息，无法发货");
                }
                if (string.IsNullOrWhiteSpace(sendname)) {
                    throw new Exception("请输入货期名称");
                }
                if (!plantime.HasValue) {
                    throw new Exception("请输入计划到货时间");
                }
                if (sendgoods.Count != sendgoods.Select(a => a.gid.Value).Distinct().Count())
                {
                    throw new Exception("发货产品重复，无法发货");
                }
                var cansend = GetCanSendGoodsNum(_order.id.Value);
                if (cansend == null)
                {
                    throw new Exception("无借货信息，无法发货");
                }
                var sendall = true;
                foreach (var sgood in sendgoods)
                {
                    if (!cansend.ContainsKey(sgood.gid.Value))
                    {
                        throw new Exception("借货产品不存在，无法发货");
                    }
                    if (cansend[sgood.gid.Value].gnum < sgood.gnum.Value)
                    {
                        throw new Exception("发货数量大于可发货数量，无法发货");
                    }
                    if (cansend[sgood.gid.Value].gnum > sgood.gnum.Value) {
                        sendall = false;
                    }
                }
                if (cansend.Count(a=>a.Value.gnum.Value>0) > sendgoods.Count) {
                    sendall = false;
                }

                var ctime = DateTime.Now;
                db.Execobj("update corder set status=" + ((int)orderstatus.cjsend) + ",statustime='" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "',sendall='"+(sendall?1:0)+"' where id=" + _order.id);
                var ovid = db.Insertobj("insert into corder_verifyflow(oid,uid,verifyno,createtime,status,des,endtime,duid,localstatus,sendname,plantime)values(" + _order.id + "," + uid + "," + ((int)orderstatus.cjsend) + ",'" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "'," + ((int)verifystatus.agree) + ",'" + desc + "',null," + duid + "," + ((int)orderstatus.cjsend) + ",'" + sendname + "','" + plantime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "')");
                if (sendgoods != null)
                {
                    foreach (var sgood in sendgoods)
                    {
                        db.Execobj("insert into corder_sendgoods(ovid,gid,gnum,sendtime)values(" + ovid + "," + sgood.gid + "," + sgood.gnum + ",'" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "')");
                    }
                }
                if (attachments != null)
                {
                    foreach (var atta in attachments)
                    {
                        db.Execobj("insert into cattachment(ovid,name,url)values(" + ovid + ",'" + atta.name + "','" + atta.url + "')");
                    }
                }
                db.End(true);
            }
            catch (Exception ex)
            {
                db.End(false);
                throw ex;
            }
            load(_order.id.Value);
            res = true;
            return res;
        }
        /// <summary>
        /// 厂家督导确认
        /// </summary>
        /// <param name="uid">督导人id</param>
        /// <param name="desc">督导意见</param>
        /// <returns>成功失败</returns>
        public bool cjreceive(int uid, string desc,int ovid,List<cattachment> attachments)
        {
            var res = false;
            var db = new DB();
            try
            {
                if (_order.verifies[ovid] == null) {
                    throw new Exception("找不到发货信息，无法督导确认");
                }
                if (_order.verifies[ovid].localstatus.Value != orderstatus.cjsend)
                {
                    throw new Exception("非发货状态，无法督导确认");
                }
                if (attachments == null || attachments.Count <= 0) {
                    throw new Exception("督导确认请添加附件");
                }
                var ctime = DateTime.Now;
                db.Execobj("update corder set status=" + ((int)orderstatus.cjreceive) + ",statustime='" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "' where id=" + _order.id);
                db.Execobj("update corder_verifyflow set localstatus="+((int)orderstatus.cjreceive)+" where id="+ovid);
                var ovid2=db.Insertobj("insert into corder_verifyflow(oid,uid,verifyno,createtime,status,des,endtime,ovid)values(" + _order.id + "," + uid + "," + ((int)orderstatus.cjreceive) + ",'" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "'," + ((int)verifystatus.agree) + ",'" + desc + "',null,"+ovid+")");
                if (attachments != null)
                {
                    foreach (var atta in attachments)
                    {
                        db.Execobj("insert into cattachment(ovid,name,url)values(" + ovid2 + ",'" + atta.name + "','" + atta.url + "')");
                    }
                }
                db.End(true);
            }
            catch (Exception ex)
            {
                db.End(false);
                throw ex;
            }
            load(_order.id.Value);
            res = true;
            return res;
        }
        /// <summary>
        /// 收货确认
        /// </summary>
        /// <param name="uid">收货人id</param>
        /// <param name="desc">收货意见</param>
        /// <param name="wid">入库的仓库id</param>
        /// <returns>成功失败</returns>
        public bool dsreceive(int uid, string desc, int wid,int ovid)
        {
            var res = false;
            var db = new DB();
            try
            {
                if (_order.verifies[ovid] == null)
                {
                    throw new Exception("找不到发货信息，无法确认收货");
                }
                if (_order.verifies[ovid].localstatus.Value != orderstatus.cjreceive)
                {
                    throw new Exception("厂家未督导确认，无法确认收货");
                }
                var ctime = DateTime.Now;
                string discompletesql = string.Format(@"select og.gid,g.name gname,og.gnum-isnull(osg.gnum,0) gnum from(
select og.gid,sum(og.gnum)gnum from corder_goods og
where og.oid={0}
group by og.gid
) og
left join(
select osg.gid,sum(osg.gnum)gnum from corder_verifyflow ov
join corder_sendgoods osg on ov.id=osg.ovid
where ov.oid={0} and (osg.receivetime is not null or ov.id={1})
group by osg.gid
)osg on osg.gid=og.gid
left join cgoods g on og.gid=g.id
where og.gnum-isnull(osg.gnum,0)>0", _order.id.Value,ovid);
                var discomplete = db.Queryobj(discompletesql);
                var complete = discomplete == null || discomplete.Rows.Count <= 0;

                db.Execobj("update corder set status=" + (complete ? (int)orderstatus.complete : (int)orderstatus.cjsend) + ",statustime='" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "' where id=" + _order.id);
                db.Execobj("update corder_verifyflow set localstatus=" + ((int)orderstatus.dsreceive) + " where id=" + ovid);
                db.Execobj("insert into corder_verifyflow(oid,uid,verifyno,createtime,status,des,endtime,ovid)values(" + _order.id + "," + uid + "," + ((int)orderstatus.dsreceive) + ",'" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "'," + ((int)verifystatus.agree) + ",'" + desc + "',null,"+ovid+")");

                //var receivegoods = GetSendgoods(true);
                var receivegoods = _order.verifies[ovid].goods;
                var stockdics = db.QueryAsDicsobj("select id,wid,gid,stock,purchased from cstock where wid=" + wid);
                foreach (var goods in receivegoods)
                {
                    if (stockdics == null || stockdics.Count(a => O2.O2I(a["gid"]) == goods.Value.gid.Value) <= 0)
                    {
                        db.Execobj("insert into cstock(wid,gid,stock,purchased,require)values(" + wid + "," + goods.Value.gid + "," + goods.Value.gnum + ",0,"+ goods.Value.gnum + ")");
                    }
                    else
                    {
                        var sid = stockdics.FirstOrDefault(a => O2.O2I(a["gid"]) == goods.Value.gid.Value)["id"];
                        var sqlcs = "update cstock set stock=stock+" + goods.Value.gnum + " where id=" + sid;
                        db.Execobj(sqlcs);
                    }
                    db.Execobj("insert into cstockio(wid,gid,ionumber,purchased,createtime,uid,stype,oid,require)values(" + wid + "," + goods.Value.gid + "," + goods.Value.gnum + ",0,'" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "'," + uid + ",0," + _order.id + ",0)");
                }

                db.Execobj("update osg set osg.receivetime='" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "' from corder_sendgoods osg left join corder_verifyflow ov on osg.ovid=ov.id where ov.oid=" + _order.id+" and ov.id="+ovid);
                //入库接口
                db.End(true);
            }
            catch (Exception ex)
            {
                db.End(false);
                throw ex;
            }
            load(_order.id.Value);
            res = true;
            return res;
        }
        /// <summary>
        /// 关闭单据
        /// </summary>
        /// <param name="uid">关闭人id</param>
        /// <returns>成功失败</returns>
        public bool close(int uid)
        {
            var res = false;
            var db = new DB();
            try
            {
                if (_order.status.Value == orderstatus.close)
                {
                    throw new Exception("单据已关闭，无法再次关闭");
                }
                if (_order.status.Value == orderstatus.complete)
                {
                    throw new Exception("单据已完成，无法关闭");
                }
                if (_order.status.Value == orderstatus.bevoid)
                {
                    throw new Exception("单据已作废，无法关闭");
                }
                if (_order.status.Value >= orderstatus.sjyverify)
                {
                    throw new Exception("设计院审核后无法关闭");
                }
                var ctime = DateTime.Now;
                db.Execobj("update corder set status=" + ((int)orderstatus.close) + ",statustime='" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "' where id=" + _order.id);
                db.Execobj("insert into corder_verifyflow(oid,uid,verifyno,createtime,status,des,endtime)values(" + _order.id + "," + uid + "," + ((int)orderstatus.close) + ",'" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "'," + ((int)verifystatus.disagree) + ",'关闭',null)");
                db.End(true);
            }
            catch (Exception ex)
            {
                db.End(false);
                throw ex;
            }
            load(_order.id.Value);
            res = true;
            return res;
        }

        public bool tovoid(int uid)
        {
            var res = false;
            var db = new DB();
            try
            {
                if (_order.status.Value == orderstatus.bevoid)
                {
                    throw new Exception("单据已作废，无法作废");
                }
                var ctime = DateTime.Now;
                db.Execobj("update corder set status=" + ((int)orderstatus.bevoid) + ",statustime='" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "' where id=" + _order.id);
                db.Execobj("insert into corder_verifyflow(oid,uid,verifyno,createtime,status,des,endtime)values(" + _order.id + "," + uid + "," + ((int)orderstatus.bevoid) + ",'" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "'," + ((int)verifystatus.disagree) + ",'作废',null)");
                db.Execobj(@"update s set s.stock=s.stock+sio.ionumber, s.require = s.require+sio.require ,s.purchased=s.purchased+sio.purchased
from cstock s left join
(select wid,gid,-sum(ionumber)ionumber,-sum(require) require,-sum(purchased) purchased from cstockio sio where oid=" + _order.id + " and stype=0 group by wid,gid) sio on s.wid=sio.wid and s.gid=sio.gid where sio.ionumber is not null");
                db.Execobj(@"insert into cstockio (wid,gid,ionumber,purchased,createtime,uid,stype,oid,require) select wid,gid,-ionumber,-purchased,'" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "'," + uid + ",0,oid,-require from cstockio where oid=" + _order.id + " and stype=0");
                db.End(true);
            }
            catch (Exception ex)
            {
                db.End(false);
                throw ex;
            }
            load(_order.id.Value);
            res = true;
            return res;
        }
        public bool delete() {
            var res = false;
            var db = new DB();
            try {
                if (_order.status.Value != orderstatus.bevoid) {
                    throw new Exception("只能删除作废的单据");
                }
                var filelist=new List<string>();
                foreach (var ver in _order.verifies) {
                    foreach (var ac in ver.Value.attachments) {
                        filelist.Add(ac.Value.url);
                    }
                    db.Execobj("delete cattachment where ovid="+ver.Key);
                    db.Execobj("delete corder_sendgoods where ovid="+ver.Key);
                    db.Execobj("delete corder_verifyflow where id="+ver.Key);
                }
                db.Execobj("delete corder_goods where oid="+_order.id);
                db.Execobj("delete corder where id="+_order.id);
                db.Execobj("delete cstockio where stype=0 and oid="+_order.id);

                db.End(true);
                foreach (var file in filelist) {
                    try
                    {
                        System.IO.File.Delete(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, file.Substring(1)));
                    }
                    catch { }
                }
            }
            catch (Exception ex) {
                db.End(false);
                throw ex;
            }
            res = true;
            return res;
        }
        /// <summary>
        /// 获取发货统计信息
        /// </summary>
        /// <param name="unreceived">是否统计已收货</param>
        /// <returns></returns>
        public Dictionary<int, corder_sendgoods> GetSendgoods(bool unreceived)
        {
            var res = new Dictionary<int, corder_sendgoods>();
            foreach (var v in _order.verifies)
            {
                foreach (var g in v.Value.goods)
                {
                    if (!unreceived || !g.Value.receivetime.HasValue)
                    {
                        if (!res.ContainsKey(g.Value.gid.Value))
                        {
                            res[g.Value.gid.Value] = g.Value;
                        }
                        else
                        {
                            res[g.Value.gid.Value].gnum += g.Value.gnum;
                        }
                    }
                }
            }
            return res;
        }

        public Dictionary<int, corder_sendgoods> GetSendgoods()
        {
            var res = new Dictionary<int, corder_sendgoods>();
            foreach (var v in _order.verifies)
            {
                foreach (var g in v.Value.goods)
                {
                    if (g.Value.receivetime.HasValue)
                    {
                        if (!res.ContainsKey(g.Value.gid.Value))
                        {
                            res[g.Value.gid.Value] = g.Value;
                        }
                        else
                        {
                            res[g.Value.gid.Value].gnum += g.Value.gnum;
                        }
                    }
                }
            }
            return res;
        }
        #endregion
        #region 静态
        /// <summary>
        /// 获取可发货明细数量
        /// </summary>
        /// <param name="oid">借货单id</param>
        /// <returns>可发货明细字典，以产品id为key</returns>
        public static Dictionary<int, corder_goods> GetCanSendGoodsNum(int oid)
        {
            var sql = string.Format(@"select og.gid,g.name gname,og.gnum-isnull(osg.gnum,0) gnum from(
select og.gid,sum(og.gnum)gnum from corder_goods og
where og.oid={0}
group by og.gid
) og
left join(
select osg.gid,sum(osg.gnum)gnum from corder_verifyflow ov
join corder_sendgoods osg on ov.id=osg.ovid
where ov.oid={0}
group by osg.gid
)osg on osg.gid=og.gid
left join cgoods g on og.gid=g.id", oid);
            var dics = DB.QueryAsDics(sql);
            if (dics == null)
            {
                return null;
            }
            var res = new Dictionary<int, corder_goods>();
            foreach (var dic in dics)
            {
                var good = new corder_goods();
                good.gid = O2.O2I(dic["gid"]);
                good.gname = dic["gname"].ToString();
                good.gnum = O2.O2I(dic["gnum"]);
                res[good.gid.Value] = good;
            }
            return res;
        }
        /// <summary>
        /// 查询借货单分页
        /// </summary>
        /// <param name="sql">where条件</param>
        /// <param name="limit">页面大小</param>
        /// <param name="offset">页码</param>
        /// <returns>借货单列表字典</returns>
        public static Dictionary<string, object>[] Select(string sql, int limit, int offset)
        {
            var rows = DB.QueryAsDics(@"select top " + limit + @" * from (
select ROW_NUMBER()over(order by o.id desc) top1,o.id,o.code,o.vid,o.createuid,cu.name createuname,o.createtime,o.receiveuid,ru.name receiveuname,o.status,o.statustime,o.verifyno,o.verifyrole 
from corder o
left join cuser cu on o.createuid=cu.id
left join cuser ru on o.receiveuid=ru.id
" + sql
+ @"
)a where top1>" + offset);
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    row["createtime"] = ((DateTime)row["createtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                    row["statustime"] = ((DateTime)row["statustime"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            return rows;
        }
        /// <summary>
        /// 查询借货单不分页
        /// </summary>
        /// <param name="sql">where条件</param>
        /// <returns>借货单列表字典</returns>
        public static Dictionary<string, object>[] Select(string sql)
        {
            var rows = DB.QueryAsDics(@"select * from (
select ROW_NUMBER()over(order by o.id desc) top1,o.id,o.code,o.vid,o.createuid,cu.name createuname,o.createtime,o.receiveuid,ru.name receiveuname,o.status,o.statustime,o.verifyno,o.verifyrole 
from corder o
left join cuser cu on o.createuid=cu.id
left join cuser ru on o.receiveuid=ru.id
" + sql
+ @"
)a");
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    row["createtime"] = ((DateTime)row["createtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                    row["statustime"] = ((DateTime)row["statustime"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            return rows;
        }
        /// <summary>
        /// 查询借货单总数量
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int SelectTotal(string sql)
        {
            var rows = DB.QueryOne(@"select count(0) total
from corder o
left join cuser cu on o.createuid=cu.id
left join cuser ru on o.receiveuid=ru.id
" + sql);

            return rows == null ? 0 : O2.O2I(rows["total"]);
        }
        /// <summary>
        /// 获取空的corder对象
        /// </summary>
        /// <returns>返回corder对象</returns>
        public static corder CreateEmptyCorder() {
            var order = new corder();
            order.goods = new Dictionary<int, corder_goods>();
            order.verifies = new Dictionary<int, corder_verifyflow>();
            return order;
        }
        #endregion
        #endregion
    }
}