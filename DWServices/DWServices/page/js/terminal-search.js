﻿$(function () {
    //1.初始化Table
    var oTable = new TableInit();
    oTable.Init();
    laydate.render({
        elem: '#starttime'
    });
    laydate.render({
        elem: '#endtime'
    });
    // 事件绑定
    bindEvents();

    // 获取所有品牌和型号
    getBrandsAndTypes(function (data) {
        var brands = data.brands;
        window.models = data.models;
        var brand = '';
        for (var i = 0; i < brands.length; i++) {
            brand += '<option value="' + brands[i] + '">' + brands[i] + '</option>';
        }
        $('#brand').append($(brand));
    });

    $('#search').on('click', function (e) {
        $('#table').bootstrapTable("removeAll").bootstrapTable("refresh");
    });
    $('#export').on('click', function () {
        $('#exportbut').trigger('click');
    });

    var chart = echarts.init(document.getElementById('chart'));
    $.ajax({
        url: '../services/VolteSingleSearch.ashx?type=getterminalchart',
        type: 'get',
        dataType: 'json',
        success: function (res) {
            if (res.ok) {
                var sum = res.sum;
                var data = res.data;
                var brands = [];
                var rates = [];
                for (var i = 0; i < data.length;i++) {
                    brands.push(data[i].BRAND);
                    rates.push(convertPercentage(data[i].SUM / sum, 4));
                }
                var option = {
                    title: {
                        text: '终端品牌占比'
                    },
                    tooltip: {},
                    //legend: {
                    //    data: ['品牌']
                    //},
                    xAxis: {
                        data: brands
                    },
                    yAxis: {},
                    series: [{
                        name: '品牌占比',
                        type: 'bar',
                        data: rates,
                        label: {
                            normal: {
                                show: true,
                                position: 'top',
                                color: '#000'
                            }
                        },
                        itemStyle: {
                            normal: {
                                color: '#f00'
                            }
                        }
                    }]
                };
                chart.setOption(option);
            }
        }
    })
});
function convertPercentage(num, n) {
    var nums = num.toString().split('.');
    if (nums[1]) {
        nums[1] = nums[1].substring(0, n);
    }
    if (nums[0] == 0) {
        nums[0] = '';
    }
    return nums[0] + ~~nums[1].substring(0, 2) + '.' + nums[1].substring(2, n);
}
var TableInit = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $('#table').bootstrapTable({
            url: '../services/VolteSingleSearch.ashx?type=terminaldata',         //请求后台的URL（*）
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
            showExport: true,
            exportTypes: ['csv'],
            exportDataType: "all",
            exportOptions: {
                fileName: 'volte-terminal',  //文件名称设置
                worksheetName: 'sheet1',  //表格工作区名称
                tableName: 'volte-terminal',
                excelstyles: ['background-color', 'color', 'font-size', 'font-weight']
            },
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
                    field: 'BRAND',
                    title: '品牌'
                }, {
                    field: 'SUMCOUNT',
                    title: '通话总次数'
                }, {
                    field: 'BADCOUNT',
                    title: '质差次数'
                }, {
                    field: 'RATE',
                    title: '通话优良率'
                }, {
                    field: 'RINGDELAY',
                    title: '振铃时延(ms)平均值'
                }, {
                    field: 'DOWNMOS',
                    title: '下行MOS均值'
                }, {
                    field: 'UPMOS',
                    title: '上行MOS均值'
                }, {
                    field: 'UPSINGLETIMERTP',
                    title: '上行单通时长(RTP)(ms)均值'
                }, {
                    field: 'DOWNSINGLETIMERTP',
                    title: '下行单通时长(RTP)(ms)均值'
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
            brand: $('#brand').val(),
            model: $('#model').val()
        };
        return temp;
    };
    return oTableInit;
};



function getBrandsAndTypes(callback) {
    $.ajax({
        url: '../services/VolteSingleSearch.ashx?type=brands',
        type: 'get',
        dataType: 'json',
        success: function (res) {
            if (res.ok) {
                callback && callback({brands:res.brands, models: res.models});
            }
        }
    });
}


function bindEvents() {
    $('#brand').on('change', function (e) {
        if ($('#brand').val() == -1) {
            $('#model').attr('disabled', 'disabled');
        } else {
            var model = '<option value="-1">请选择</option>';
            var brand = $('#brand').val();
            for (var i = 0; i < window.models[brand].length;i++) {
                model += '<option value="' + window.models[brand][i] +'">'+window.models[brand][i]+'</option>';
            }

            $('#model').empty().append($(model)).removeAttr('disabled');
        }
    });
}