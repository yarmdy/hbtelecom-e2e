$(function () {
    //1.初始化Table
    var oTable = new TableInit();
    oTable.Init();

    laydate.render({
        elem: '#starttime'
    });
    laydate.render({
        elem: '#endtime'
    });

    $('#search').on('click', function (e) {
        $('#table').bootstrapTable("removeAll").bootstrapTable("refresh");
    });
    $('#export').on('click', function () {
        var start = $('#starttime').val();
        var end = $('#endtime').val();
        var num = $('#number').val();
        $('#realexport').parent().attr('href', '/services/VolteSingleSearch.ashx?type=download&start=' + start + '&end=' + end + '&num=' + num);
        $('#realexport').trigger('click');
    });
});

var TableInit = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $('#table').bootstrapTable({
            url: '../services/VolteSingleSearch.ashx?type=alldata',         //请求后台的URL（*）
            method: 'get',                      //请求方式（*）
            //height: 600,
            toolbar: '#tbaction',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "asc",                   //排序方式
            queryParams: oTableInit.queryParams,//传递参数（*）
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            pageNumber: 1,                       //初始化加载第一页，默认第一页
            pageSize: 15,                       //每页的记录行数（*）
            showExport: false,
            exportTypes: ['csv'],
            exportDataType: "all",
            exportOptions: {
                //ignoreColumn: [0, 1],  //忽略某一列的索引
                fileName: '用户信息报表',  //文件名称设置
                worksheetName: 'sheet1',  //表格工作区名称
                tableName: '用户信息报表',
                excelstyles: ['background-color', 'color', 'font-size', 'font-weight']
            },
            pageList: [10, 25, 50, 100],        //可供选择的每页的行数（*）
            search: false,                       //是否显示表格搜索，此搜索是客户端搜索，不会进服务端，所以，个人感觉意义不大
            strictSearch: false,
            showColumns: false,                  //是否显示所有的列
            showRefresh: false,                  //是否显示刷新按钮
            minimumCountColumns: 2,             //最少允许的列数
            clickToSelect: true,                //是否启用点击选中行                        //行高，如果没有设置height属性，表格自动根据记录条数觉得表格高度
            //uniqueId: "MSISDN",                     //每一行的唯一标识，一般为主键列
            showToggle: false,                    //是否显示详细视图和列表视图的切换按钮
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: [{
                field: 'STARTTIME',
                title: '开始时间',
                width: 100
            }, {
                field: 'ENDTIME',
                title: '结束时间'
            }, {
                field: 'BUSINESSTYPE',
                title: '业务类型'
            }, {
                field: 'BUSINESSSTATUS',
                title: '业务状态'
            }, {
                field: 'MSISDN2',
                title: 'MSISDN'
            }, {
                field: 'IMSI',
                title: 'IMSI'
            }, {
                field: 'IMEI',
                title: 'IMEI'
            }, {
                field: 'BRAND',
                title: '终端品牌'
            }, {
                field: 'MODEL',
                title: '终端型号'
            }, {
                field: 'VOLTEOS',
                title: 'VoLTE终端OS版本'
            }, {
                field: 'WARNINGTEXT',
                title: 'Warning Text'
            }, {
                field: 'REASONTEXT',
                title: 'Warning Text'
            }, {
                field: 'FIRSTNETTYPE',
                title: '第一拆线网元类型'
            }, {
                field: 'FIRSTNETIP',
                title: '第一拆线网元IP'
            }, {
                field: 'FIRSTNET',
                title: '第一拆线网元'
            }, {
                field: 'FIRSTREASON',
                title: '第一拆线原因'
            }, {
                field: 'INNETTYPE',
                title: '接入网类型'
            }, {
                field: 'PROVINCE',
                title: '省'
            }, {
                field: 'CITY',
                title: '市'
            }, {
                field: 'START4G',
                title: '初始4G小区'
            }, {
                field: 'TRACKAREA',
                title: '跟踪区'
            }, {
                field: 'PCSCFIP',
                title: '型号PCSCF IP'
            }, {
                field: 'PCSCFNAME',
                title: 'PCSCF名称'
            }, {
                field: 'ICSCFIP',
                title: 'ICSCF IP'
            }, {
                field: 'ICSCFNAME',
                title: 'ICSCF名称'
            }, {
                field: 'MMEIP',
                title: 'MME IP'
            }, {
                field: 'MMENAME',
                title: 'MME 名称'
            }, {
                field: 'SGWIP',
                title: 'SGW IP'
            }, {
                field: 'SGWNAME',
                title: 'SGW 名称'
            }, {
                field: 'ENODEBIP',
                title: 'eNodeB IP'
            }, {
                field: 'ENODEBNAME',
                title: 'eNodeB名称'
            }, {
                field: 'MEDIATYPE',
                title: '媒体类型'
            }, {
                field: 'STARTECGI',
                title: '初始ECGI'
            }, {
                field: 'ENDECGI',
                title: '结束ECGI'
            }, {
                field: 'CALLTYPE',
                title: '主被叫标识'
            }, {
                field: 'CALLNUMBER',
                title: '主叫号码'
            }, {
                field: 'CALLEDNUMBER',
                title: '被叫号码'
            }, {
                field: 'RINGDELAY',
                title: '振铃时延(ms)'
            }, {
                field: 'TALKDELAY',
                title: '应答时延(ms)'
            }, {
                field: 'TALKTIME',
                title: '通话时长(s)'
            }, {
                field: 'STARTENODEB',
                title: '初始eNodeB'
            }, {
                field: 'ENDENODEB',
                title: '结束eNodeB'
            }, {
                field: 'INTERFACETYPE',
                title: '接口类型'
            }, {
                field: 'OTHERCALLTYPE',
                title: '对端呼叫类型'
            }, {
                field: 'CODETYPE',
                title: '编解码类型'
            }, {
                field: 'UPCODERATE',
                title: '上行编解码速率(kbps)'
            }, {
                field: 'DOWNCODERATE',
                title: '下行编解码速率(kbps)'
            }, {
                field: 'UPVIDEORESOLUTION',
                title: '上行视频分辨率'
            }, {
                field: 'DOWNVIDEORESOLUTION',
                title: '下行视频分辨率'
            }, {
                field: 'UPVIDEOFPS',
                title: '上行视频帧率(fps)'
            }, {
                field: 'DOWNVIDEOFPS',
                title: '下行视频帧率(fps)'
            }, {
                field: 'UPMOS',
                title: '上行MOS均值'
            }, {
                field: 'UPRTCPAVG',
                title: '上行抖动均值(RTCP)(ms)'
            }, {
                field: 'UPRTCPBAG',
                title: '上行RTP期望包数(RTCP)'
            }, {
                field: 'UPRTCPLOSTBAG',
                title: '上行RTP丢包数(RTCP)'
            }, {
                field: 'DOWNMOS',
                title: '下行MOS均值'
            }, {
                field: 'DOWNRTCPAVG',
                title: '下行抖动均值(RTCP)(ms)'
            }, {
                field: 'DOWNRTCPBAG',
                title: '下行RTP期望包数(RTCP)'
            }, {
                field: 'DOWNRTCPLOSTBAG',
                title: '下行RTP丢包数(RTCP)'
            }, {
                field: 'UPMOSBADCYCLE',
                title: '上行MOS差周期数'
            }, {
                field: 'UPMOSALLCYCLE',
                title: '上行MOS统计周期数'
            }, {
                field: 'DOWNMOSBADCYCLE',
                title: '下行MOS差周期数'
            }, {
                field: 'DOWNMOSALLCYCLE',
                title: '下行MOS统计周期数'
            }, {
                field: 'ROLLDELAYAVG',
                title: '环路时延均值(ms)'
            }, {
                field: 'UPIPMOSAVG',
                title: '上行IPMOS均值'
            }, {
                field: 'UPJITTERAVG',
                title: '上行抖动均值(ms)'
            }, {
                field: 'UPRTPBAG',
                title: '上行RTP期望包数'
            }, {
                field: 'UPRTPLOSTBAG',
                title: '上行RTP丢包数'
            }, {
                field: 'DOWNIPMOSAVG',
                title: '下行IPMOS均值'
            }, {
                field: 'DOWNJITTERAVG',
                title: '下行抖动均值(ms)'
            }, {
                field: 'DOWNRTPBAG',
                title: '下行RTP期望包数'
            }, {
                field: 'DOWNRTPLOSTBAG',
                title: '下行RTP丢包数'
            }, {
                field: 'UPIPMOSBADCYCLE',
                title: '上行IPMOS差周期数'
            }, {
                field: 'UPIPMOSALLCYCLE',
                title: '上行IPMOS统计周期数'
            }, {
                field: 'DOWNIPMOSBADCYCLE',
                title: '下行IPMOS差周期数'
            }, {
                field: 'DOWNIPMOSALLCYCLE',
                title: '下行RTP丢包数'
            }, {
                field: 'SINGLEIDENTITY',
                title: '单通标识'
            }, {
                field: 'UPSINGLETIMERTP',
                title: '上行单通时长(RTP)(ms)'
            }, {
                field: 'DOWNSINGLETIMERTP',
                title: '下行单通时长(RTP)(ms)'
            }, {
                field: 'UPSINGLETIMERTCP',
                title: '上行单通时长(RTCP)(ms)'
            }, {
                field: 'DOWNSINGLETIMERTCP',
                title: '下行单通时长(RTCP)(ms)'
            }, {
                field: 'UPLOSTWORDTIME',
                title: '上行吞字时长(ms)'
            }, {
                field: 'UPONOFFTIME',
                title: '上行断续时长(ms)'
            }, {
                field: 'DOWNLOSTWORDTIME',
                title: '下行吞字时长(ms)'
            }, {
                field: 'DOWNONOFFTIME',
                title: '下行断续时长(ms)'
            }, {
                field: 'SWICHNUM',
                title: '切换请求次数'
            }, {
                field: 'SWITCHSUCCESSNUM',
                title: '切换成功次数'
            }, {
                field: 'CLASSNAME',
                title: 'class-name'
            }, {
                field: 'SCENENAME',
                title: 'scene-name'
            }
            ]
        });
    };

    //得到查询的参数
    oTableInit.queryParams = function (params) {
        var temp = {   //这里的键的名字和控制器的变量名必须一直，这边改动，控制器也需要改成一样的
            limit: params.limit,   //页面大小
            offset: params.offset,  //页码
            start: $('#starttime').val(),
            end: $('#endtime').val(),
            num: $('#number').val()
        };
        return temp;
    };
    return oTableInit;
};