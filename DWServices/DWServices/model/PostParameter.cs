using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWServices.model
{
    public class PostParameter
    {
        //查询时间
        String m_quertyTime;
        public String QuertyTime
        {
            get { return m_quertyTime; }
            set { m_quertyTime = value; }
        }
        //查询类型
        string m_quertyType;
        public string QuertyType
        {
            get { return m_quertyType; }
            set { m_quertyType = value; }
        }
        //查询尺度天级别:D、小时级别：H
        string m_quertyScale;
        public string QuertyScale
        {
            get { return m_quertyScale; }
            set { m_quertyScale = value; }
        }
        //小区标识
        string m_eci;
        public string ECI
        {
            get { return m_eci; }
            set { m_eci = value; }
        }
        //小区编号
        string m_lcrid;
        public string LCRID
        {
            get { return m_lcrid; }
            set { m_lcrid = value; }
        }
        //基站标识
        string m_enbid;
        public string ENBID
        {
            get { return m_enbid; }
            set { m_enbid = value; }
        }

        public static PostParameter getParameter(HttpContext context)
        {
            PostParameter parameter = new PostParameter();
            try
            {
                if (!string.IsNullOrEmpty(context.Request["key"]))
                    parameter.QuertyType = context.Request["key"].ToLower();
                if (!string.IsNullOrEmpty(context.Request["scale"]))
                    parameter.QuertyScale = context.Request["scale"].ToUpper();
                parameter.LCRID = context.Request["cellId"];
                parameter.ECI = context.Request["eECI"];
                parameter.ENBID = context.Request["eNBID"];
                parameter.QuertyTime = context.Request["datePicker"];
            }
            catch
            {
                throw;
            }
            return parameter;
        }
    }
}