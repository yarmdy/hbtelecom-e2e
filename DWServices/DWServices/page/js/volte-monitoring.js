$(function () {
    queryHeaderInfo(function (data) {
        $('#allCount').text(data.allcount);
        $('#badCount').text(data.badcount);
        $('#badRate').text(convertPercentage(data.rate, 4)+'%');
        $('#coreRate').text(convertPercentage(data.corerate, 4) + '%');
        $('#wifiRate').text(convertPercentage(data.wifirate, 4) + '%');
        $('#terminalRate').text(convertPercentage(data.terrate, 4) + '%');
    });

    var coreChart = echarts.init(document.getElementById('coreChart'));
    var coreChart2 = echarts.init(document.getElementById('coreChart2'));
    var wifiChart = echarts.init(document.getElementById('wifiChart'));
    var terChart = echarts.init(document.getElementById('terminalChart'));
    var terChart2 = echarts.init(document.getElementById('terminalChart2'));
    loadChart(function (data) {
        data.core.params = {
            title: '核心网通话优良率',
            legend: [{ name: '核心网通话优良率', icon: 'roundRect' }],
            seriesName: '核心网通话优良率',
            color: '#fa5400'
        };
        data.core2.params = {
            title: '全部核心网原因分布',
            legend: [{ name: '数量', icon: 'roundRect' }],
            seriesName: '数量',
            color: '#fa5400'
        };
        data.wifi.params = {
            title: '数量',
            legend: [{ name: '数量', icon: 'roundRect' }],
            seriesName: '数量',
            color: '#ca4589'
        };
        data.terminal.params = {
            title: '终端通话优良率',
            legend: [{ name: '终端通话优良率', icon: 'roundRect' }],
            seriesName: '终端通话优良率',
            color: '#dbf470'
        };
        data.terminal2.params = {
            title: '终端型号通话优良率',
            legend: [{ name: '终端型号通话优良率', icon: 'roundRect' }],
            seriesName: '终端型号通话优良率',
            color: '#abc520'
        };
        renderChart(coreChart, data.core);
        renderChart(coreChart2, data.core2);
        renderChart(wifiChart, data.wifi);
        renderChart(terChart, data.terminal);
        renderChart(terChart2, data.terminal2);
    });
    var wifiCity = '全省';
    wifiChart.on('click', function (params) {
        wifiCity = params.name;
        $('#wifitable').bootstrapTable('removeAll').bootstrapTable("refresh", {
            query: {
                city: params.name
            }
        });
        $('#wifiTableTitle').text(params.name + '无线小区');
        if (params.name === '全省') {
            $('#wifitable').bootstrapTable("showColumn", "CITY");
        }
        else {
            $('#wifiData #wifitable').bootstrapTable("hideColumn", "CITY");
        }
    });
    coreChart.on('click', function (params) {
        mmeName = params.name;
        $.ajax({
            url: '../services/VolteSingleSearch.ashx?type=corereason',
            data: { 'mmename': mmeName},
            dataType: 'json',
            success: function (res) {
                if (res.ok) {
                    res.data.params = {
                        title: '[' + mmeName + ']核心网原因分布',
                        legend: [{ name: '数量', icon: 'roundRect' }],
                        seriesName: '数量',
                        color: '#fa5400'
                    };
                    renderChart(coreChart2, res.data);
                }
            }
        });
    });
    $('#coreexport').on('click', function () {
        $('#exportbut').trigger('click');
    });
    $('#wifiexport').on('click', function () {

        $('#realexport').parent().attr('href', '/services/VolteSingleSearch.ashx?type=downloadwifi&wificity=' + wifiCity);
        $('#realexport').trigger('click');
    });
    // wifitable
    var oTable = new TableInit();
    oTable.Init();

});

// 柱状图设置工厂
function optionFactory(params) {
    var option = {
        title: {
            text: params.title
        },
        tooltip: {},
        legend: {
            data: params.legend
        },
        xAxis: {
            data: params.xaxis
        },
        yAxis: {},
        series: [{
            name: params.seriesName,
            type: 'bar',
            data: params.data,
            label: {
                normal: {
                    show: true,
                    position: 'top',
                    color: '#000'
                }
            },
            itemStyle: {
                normal: {
                    color: params.color
                }
            }
        }]
    };
    return option;
}

function renderChart(obj, data) {
    var x = Object.keys(data).filter(function (x) { return x !== 'params';});
    var datas = [];
    for (var i = 0; i < x.length; i++) {
        if (x[i] !== 'params') {
            datas.push(data[x[i]]);
        }
    }
    data.params.xaxis = x.reverse();
    data.params.data = datas.reverse();
    var option = optionFactory(data.params);
    obj.setOption(option);
}

function queryHeaderInfo(callback) {
    $.ajax({
        url: '../services/VolteSingleSearch.ashx?type=volteheader',
        type: 'get',
        dataType: 'json',
        success: function (res) {
            if (res.ok) {
                callback && callback(res.data);
            }
        }
    });
}

function loadChart(callback) {
    $.ajax({
        url: '../services/VolteSingleSearch.ashx?type=chartdata',
        type: 'get',
        dataType: 'json',
        success: function (res) {
            if (res.ok) {
                callback && callback(res.data);
            }
        }
    });
}

function convertPercentage(num, n) {
    var nums = num.toString().split('.');
    if (nums[1]) {
        nums[1] = nums[1].substring(0,n);
    }
    if (nums[0] == 0) {
        nums[0] = '';
    }
    return nums[0] + ~~nums[1].substring(0, 2) + '.' + nums[1].substring(2,n);
}

function getCoreTable(callback) {
    $.ajax({
        url: '../services/VolteSingleSearch.ashx?type=coretable',
        type: 'get',
        dataType: 'json',
        success: function (res) {
            if (res.ok) {
                callback && callback(res.data);
            }
        }
    });
}

function getWifiTable(callback) {
    $.ajax({
        url: '../services/VolteSingleSearch.ashx?type=wifitable',
        type: 'get',
        dataType: 'json',
        success: function (res) {
            if (res.ok) {
                callback && callback(res.data);
            }
        }
    });
}

var TableInit = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $('#wifitable').bootstrapTable({
            url: '../services/VolteSingleSearch.ashx?type=wifitable',         //请求后台的URL（*）
            method: 'get',                      //请求方式（*）
            toolbar: '#tbaction',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "asc",                   //排序方式
            queryParams: oTableInit.queryParams,//传递参数（*）
            sidePagination: "client",           //分页方式：client客户端分页，server服务端分页（*）
            pageNumber: 1,                       //初始化加载第一页，默认第一页
            pageSize: 15,                       //每页的记录行数（*）
            //showExport: true,
            //exportTypes: ['csv'],
            //exportDataType: "all",
            //exportOptions: {
            //    fileName: '无线',  //文件名称设置
            //    worksheetName: 'sheet1',  //表格工作区名称
            //    tableName: '无线',
            //    //excelstyles: ['background-color', 'color', 'font-size', 'font-weight']
            //},
            pageList: [10, 25, 50, 100],        //可供选择的每页的行数（*）
            search: false,                       //是否显示表格搜索，此搜索是客户端搜索，不会进服务端，所以，个人感觉意义不大
            strictSearch: false,
            showColumns: false,                  //是否显示所有的列
            showRefresh: false,                  //是否显示刷新按钮
            minimumCountColumns: 2,             //最少允许的列数
            clickToSelect: true,                //是否启用点击选中行
            //height: 600,                        //行高，如果没有设置height属性，表格自动根据记录条数觉得表格高度
            //uniqueId: "id",                     //每一行的唯一标识，一般为主键列
            showToggle: false,                    //是否显示详细视图和列表视图的切换按钮
            cardView: false,                    //是否显示详细视图
            detailView: false,                   //是否显示父子表
            columns: [{
                    field: 'CITY',
                    title: '城市'
                }, {
                    field: 'START4G',
                    title: '小区'
                }, {
                    field: 'UPMOS',
                    title: '上行MOS质差数'
                }, {
                    field: 'DOWNMOS',
                    title: '下行MOS质差数'
                }, {
                    field: 'FIRSTREASON',
                    title: '非200 ok 质差数'
                }
            ]
        });
    };

    //得到查询的参数
    oTableInit.queryParams = function (params) {
        var temp = {   //这里的键的名字和控制器的变量名必须一直，这边改动，控制器也需要改成一样的
            limit: params.limit,   //页面大小
            offset: params.offset  //页码
        };
        return temp;
    };
    return oTableInit;
};


