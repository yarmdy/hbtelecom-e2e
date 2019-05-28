//启动加载
$(function () {
    getdata();
    getdate();
    //定时器每秒调用一次fnDate()
    setInterval(function () {
        getdate();
    }, 1000 * 60);
})

function getdate() {
    var myDate = new Date();
    var str = "";
    var month = parseInt(myDate.getMonth()) + 1;
    var xq = myDate.getDay();
    if (xq == 0) {
        str = "星期日";
    } else if (xq == 1) {
        str = "星期一";
    } else if (xq == 2) {
        str = "星期二";
    } else if (xq == 3) {
        str = "星期三";
    } else if (xq == 4) {
        str = "星期四";
    } else if (xq == 5) {
        str = "星期五";
    } else if (xq == 6) {
        str = "星期六";
    };
    var date = myDate.getFullYear() + "年" + month + "月" + myDate.getDate() + "日 " + str + " " + gw_now_addzero(myDate.getHours()) + ":" + gw_now_addzero(myDate.getMinutes());
    $("#time").html(date);
}
function gw_now_addzero(temp) {
    if (temp < 10) return "0" + temp;
    else return temp;
}
//ajax获取数据
function getdata() {
    $.ajax({
        url: "/Req",
        type: "post",
        success: function (result) {
            var dataJson = eval('(' + result + ')');
            if (dataJson.ok) {
                var data = dataJson.Table;
                $("#stghwl").html(data[0]["PROTGTRA"]);
                $("#sfgll").html(parseFloat(data[1]["TGOPERFAC"])+"%");
                $("#sprbll").html(data[0]["BFLOW"]);
                $("#sfgmrfg").html(data[0]["ALARM"]);
                DrawBar(data)
            } else {
                alert("没有数据!");
            }
        },
        error: function () {
            alert("加载数据失败,请重试！");
        },
    });
}
//生成柱状图
function DrawBar(data) {
    //最忙时
    busyTime(data);
    //4G流量柱状图
    fourGBar(data);
    //RPB利用率柱状图
    rpbBar(data);
    //MR覆盖率柱状图
    mrFglBar(data);
    //2G话务量柱状图
    twoHwlBar(data);
    //2G资源利用率柱状图
    twoZyBar(data);
    //4G网络告警
    alarmBar(data);
}

function getMax(data,key) {
    var max =data[1][key];
    for (var i = 2; i < data.length;i++){
        if (data[i][key] > max) {
            max = data[i][key];
        }
    }
}

function busyTime(data) {
    var myChart = echarts.init(document.getElementById("cmtime"));
    option = {
        title: {
            text: '各地市4G超忙小区-15分钟',
            textStyle: {
                fontSize: 14,
                //////fontWeight: 'lighter',
                color: '#0F0F0F'
            }

        },
        color: ['#3398DB'],
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        xAxis: [
            {

                type: 'category',
                data: [data[1]["CITY"], data[2]["CITY"], data[3]["CITY"], data[4]["CITY"], data[5]["CITY"], data[6]["CITY"], data[7]["CITY"], data[8]["CITY"], data[9]["CITY"], data[10]["CITY"], data[11]["CITY"], data[12]["CITY"]],
                axisTick: {
                    alignWithLabel: true
                },
                axisLabel: {
                    textStyle: {
                        color: "#0F0F0F"
                    }
                }
            }
        ],
        yAxis: [
            {
                //inverse: true,
                type: 'value',
                min: 0,
                max: getMax(data),
                data: ['6', '12', '18', '24', '30'],
                axisLabel: {
                    textStyle: {
                        color: "#0F0F0F"
                    }
                }
            }
        ],
        series: [
            {
                name: '各地市4G超忙小区-15分钟',
                type: 'bar',
                barWidth: '60%',
                data: [parseFloat(data[1]["BFLOW"]), parseFloat(data[2]["BFLOW"]), parseFloat(data[3]["BFLOW"]), parseFloat(data[4]["BFLOW"]), parseFloat(data[5]["BFLOW"]), parseFloat(data[6]["BFLOW"]), parseFloat(data[7]["BFLOW"]), parseFloat(data[8]["BFLOW"]), parseFloat(data[9]["BFLOW"]), parseFloat(data[10]["BFLOW"]), parseFloat(data[11]["BFLOW"]), parseFloat(data[12]["BFLOW"])]
            }
        ]
    };
    myChart.setOption(option);
    myChart.on('click', function (param) {
        var name = param.name;
        detailedData(name);
    });
}
function fourGBar(data) {
    var myChart = echarts.init(document.getElementById("qfgll"));
    option = {
        title: {
            text: '各地市4G流量(TB)',
            textStyle: {
                fontSize: 14,
                ////fontWeight: 'lighter',
                color: '#0F0F0F'
            }
        },
        color: ['#3398DB'],
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        xAxis: [
            {

                type: 'category',
                data: [ data[1]["CITY"], data[2]["CITY"], data[3]["CITY"], data[4]["CITY"], data[5]["CITY"], data[6]["CITY"], data[7]["CITY"], data[8]["CITY"], data[9]["CITY"], data[10]["CITY"], data[11]["CITY"], data[12]["CITY"]],
                axisTick: {
                    alignWithLabel: true
                },
                axisLabel: {
                    textStyle: {
                        color: "#0F0F0F"
                    }
                }
            }
        ],
        yAxis: [
            {
                //inverse: true,
                type: 'value',
                min: 0,
                max: getMax(data),
                data: ['20000', '50000', '100000', '500000', '1000000'],
                axisLabel: {
                    textStyle: {
                        color: "#0F0F0F"
                    }
                }
            }
        ],
        series: [
            {
                name: '各地市4G流量',
                type: 'bar',
                barWidth: '60%',
                data: [parseFloat(data[1]["PROFGFLOW"]) / 1024/1024, parseFloat(data[2]["PROFGFLOW"]) / 1024/1024, parseFloat(data[3]["PROFGFLOW"]) / 1024/1024, parseFloat(data[4]["PROFGFLOW"]) / 1024/1024, parseFloat(data[5]["PROFGFLOW"]) / 1024/1024, parseFloat(data[6]["PROFGFLOW"]) / 1024/1024, parseFloat(data[7]["PROFGFLOW"]) / 1024/1024, parseFloat(data[8]["PROFGFLOW"]) / 1024/1024, parseFloat(data[9]["PROFGFLOW"]) / 1024/1024, parseFloat(data[10]["PROFGFLOW"]) / 1024/1024, parseFloat(data[11]["PROFGFLOW"]) / 1024/1024, parseFloat(data[12]["PROFGFLOW"]) / 1024/1024]
            }
        ]
    };
    myChart.setOption(option);
}
function rpbBar(data) {
    var myChart = echarts.init(document.getElementById("qprbll"));
    option = {
        title: {
            text: '各地市RPB利用率',
            textStyle: {
                fontSize: 14,
                ////fontWeight: 'lighter',
                color: '#0F0F0F'
            }
        },
        color: ['#3398DB'],
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        xAxis: [
            {
                type: 'category',
                data: [data[1]["CITY"], data[2]["CITY"], data[3]["CITY"], data[4]["CITY"], data[5]["CITY"], data[6]["CITY"], data[7]["CITY"], data[8]["CITY"], data[9]["CITY"], data[10]["CITY"], data[11]["CITY"], data[12]["CITY"]],
                axisTick: {
                    alignWithLabel: true
                },
                axisLabel: {
                    textStyle: {
                        color: "#0F0F0F"
                    }
                }
            }
        ],
        yAxis: [
            {
                type: 'value',
                min: 0,
                max: 1,
                data: ['0.2', '0.4', '0.6', '0.8', '1'],
                axisLabel: {
                    textStyle: {
                        color: "#0F0F0F"
                    }
                }
            }
        ],
        series: [
            {
                name: '各地市RPB利用率',
                type: 'bar',
                barWidth: '60%',
                data: [parseFloat(data[1]["PRBOPERFAC"]), parseFloat(data[2]["PRBOPERFAC"]), parseFloat(data[3]["PRBOPERFAC"]), parseFloat(data[4]["PRBOPERFAC"]), parseFloat(data[5]["PRBOPERFAC"]), parseFloat(data[6]["PRBOPERFAC"]), parseFloat(data[7]["PRBOPERFAC"]), parseFloat(data[8]["PRBOPERFAC"]), parseFloat(data[9]["PRBOPERFAC"]), parseFloat(data[10]["PRBOPERFAC"]), parseFloat(data[11]["PRBOPERFAC"]), parseFloat(data[12]["PRBOPERFAC"])]
            }
        ]
    };
    myChart.setOption(option);
}
function mrFglBar(data) {
    var myChart = echarts.init(document.getElementById("qmrfg"));
    option = {
        title: {
            text: '各地市MR覆盖率',
            textStyle: {
                fontSize: 14,
                //fontWeight: 'lighter',
                color: '#0F0F0F'
            }
        },
        color: ['#3398DB'],
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        xAxis: [
            {
                type: 'category',
                data: [data[1]["CITY"], data[2]["CITY"], data[3]["CITY"], data[4]["CITY"], data[5]["CITY"], data[6]["CITY"], data[7]["CITY"], data[8]["CITY"], data[9]["CITY"], data[10]["CITY"], data[11]["CITY"], data[12]["CITY"]],
                axisTick: {
                    alignWithLabel: true
                },
                axisLabel: {
                    textStyle: {
                        color: "#0F0F0F"
                    }
                }
            }
        ],
        yAxis: [
            {
                type: 'value',
                min: 0,
                max: 1,
                data: ['0.2', '0.4', '0.6', '0.8', '1'],
                axisLabel: {
                    textStyle: {
                        color: "#0F0F0F"
                    }
                }
            }
        ],
        series: [
            {
                name: '各地市MR覆盖率',
                type: 'bar',
                barWidth: '60%',
                data: [parseFloat(data[1]["MRCOVER"]), parseFloat(data[2]["MRCOVER"]), parseFloat(data[3]["MRCOVER"]), parseFloat(data[4]["MRCOVER"]), parseFloat(data[5]["MRCOVER"]), parseFloat(data[6]["MRCOVER"]), parseFloat(data[7]["MRCOVER"]), parseFloat(data[8]["MRCOVER"]), parseFloat(data[9]["MRCOVER"]), parseFloat(data[10]["MRCOVER"]), parseFloat(data[11]["MRCOVER"]), parseFloat(data[12]["MRCOVER"])]
            }
        ]
    };
    myChart.setOption(option);
}
function twoHwlBar(data) {
    var myChart = echarts.init(document.getElementById("qtghwl"));
    option = {
        title: {
            text: '各地市2G话务量',
            textStyle: {
                fontSize: 14,
                //fontWeight: 'lighter',
                color: '#0F0F0F'
            }
        },
        color: ['#3398DB'],
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        xAxis: [
            {
                type: 'category',
                data: [data[1]["CITY"], data[2]["CITY"], data[3]["CITY"], data[4]["CITY"], data[5]["CITY"], data[6]["CITY"], data[7]["CITY"], data[8]["CITY"], data[9]["CITY"], data[10]["CITY"], data[11]["CITY"], data[12]["CITY"]],
                axisTick: {
                    alignWithLabel: true
                },
                axisLabel: {
                    textStyle: {
                        color: "#0F0F0F"
                    }
                }
            }
        ],
        yAxis: [
            {
                type: 'value',
                min: 0,
                max: 1,
                data: ['0.2', '0.4', '0.6', '0.8', '1'],
                axisLabel: {
                    textStyle: {
                        color: "#0F0F0F"
                    }
                }
            }
        ],
        series: [
            {
                name: '各地市2G话务量',
                type: 'bar',
                barWidth: '60%',
                data: [parseFloat(data[1]["PROTGTRA"]), parseFloat(data[2]["PROTGTRA"]), parseFloat(data[3]["PROTGTRA"]), parseFloat(data[4]["PROTGTRA"]), parseFloat(data[5]["PROTGTRA"]), parseFloat(data[6]["PROTGTRA"]), parseFloat(data[7]["PROTGTRA"]), parseFloat(data[8]["PROTGTRA"]), parseFloat(data[9]["PROTGTRA"]), parseFloat(data[10]["PROTGTRA"]), parseFloat(data[11]["PROTGTRA"]), parseFloat(data[12]["PROTGTRA"])]
            }
        ]
    };
    myChart.setOption(option);
}
function twoZyBar(data) {
    var myChart = echarts.init(document.getElementById("qzyll"));
    option = {
        title: {
            text: '各地市2G资源利用率',
            textStyle: {
                fontSize: 14,
                //fontWeight: 'lighter',
                color: '#0F0F0F'
            }
        },
        color: ['#3398DB'],
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        xAxis: [
            {
                type: 'category',
                data: [data[1]["CITY"], data[2]["CITY"], data[3]["CITY"], data[4]["CITY"], data[5]["CITY"], data[6]["CITY"], data[7]["CITY"], data[8]["CITY"], data[9]["CITY"], data[10]["CITY"], data[11]["CITY"], data[12]["CITY"]],
                axisTick: {
                    alignWithLabel: true
                },
                axisLabel: {
                    textStyle: {
                        color: "#0F0F0F"
                    }
                }
            }
        ],
        yAxis: [
            {
                type: 'value',
                min: 0,
                max: 1,
                data: ['0.2', '0.4', '0.6', '0.8', '1'],
                axisLabel: {
                    textStyle: {
                        color: "#0F0F0F"
                    }
                }
            }
        ],
        series: [
            {
                name: '各地市2G资源利用率',
                type: 'bar',
                barWidth: '60%',
                data: [parseFloat(data[1]["TGOPERFAC"]), parseFloat(data[2]["TGOPERFAC"]), parseFloat(data[3]["TGOPERFAC"]), parseFloat(data[4]["TGOPERFAC"]), parseFloat(data[5]["TGOPERFAC"]), parseFloat(data[6]["TGOPERFAC"]), parseFloat(data[7]["TGOPERFAC"]), parseFloat(data[8]["TGOPERFAC"]), parseFloat(data[9]["TGOPERFAC"]), parseFloat(data[10]["TGOPERFAC"]), parseFloat(data[11]["TGOPERFAC"]), parseFloat(data[12]["TGOPERFAC"])]
            }
        ]
    };
    myChart.setOption(option);
}

function alarmBar(data) {
    var myChart = echarts.init(document.getElementById("alarm"));
    option = {
        title: {
            text: '各地市4G网络告警-15分钟',
            textStyle: {
                fontSize: 14,
                //fontWeight: 'lighter',
                color: '#0F0F0F'
            }
        },
        color: ['#3398DB'],
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        xAxis: [
            {
                type: 'category',
                data: [data[1]["CITY"], data[2]["CITY"], data[3]["CITY"], data[4]["CITY"], data[5]["CITY"], data[6]["CITY"], data[7]["CITY"], data[8]["CITY"], data[9]["CITY"], data[10]["CITY"], data[11]["CITY"], data[12]["CITY"]],
                axisTick: {
                    alignWithLabel: true
                },
                axisLabel: {
                    textStyle: {
                        color: "#0F0F0F"
                    }
                }
            }
        ],
        yAxis: [
            {
                //inverse: true,
                type: 'value',
                min: 0,
                max: getMax(data),
                data: ['2000', '4000', '6000', '8000', '10000'],
                axisLabel: {
                    textStyle: {
                        color: "#0F0F0F"
                    }
                }
            }
        ],
        series: [
            {
                name: '各地市4G网络告警-15分钟',
                type: 'bar',
                barWidth: '60%',
                data: [parseFloat(data[1]["ALARM"]), parseFloat(data[2]["ALARM"]), parseFloat(data[3]["ALARM"]), parseFloat(data[4]["ALARM"]), parseFloat(data[5]["ALARM"]), parseFloat(data[6]["ALARM"]), parseFloat(data[7]["ALARM"]), parseFloat(data[8]["ALARM"]), parseFloat(data[9]["ALARM"]), parseFloat(data[10]["ALARM"]), parseFloat(data[11]["ALARM"]), parseFloat(data[12]["ALARM"])]
            }
        ]
    };
    myChart.setOption(option);
    myChart.on('click', function (param) {
        var name = param.name;
        alarmData(name);
    });
}

function alarmData(name) {
    $.ajax({
        url: "/Detailed/getAlarmData",
        data: { "name": name },
        type: "post",
        success: function (result) {
            var dataJson = eval('(' + result + ')');
            if (dataJson.ok) {
                var data = dataJson.Table;
                var tal = AlarmTable(data);
                $("#tableid").empty();
                $("#tableid").append(tal);
                $("#myModalLabel").html("各地市4G网络告警-15分钟");
                $('#myModal').modal({
                    keyboard: true
                })

            } else {
                alert("没有数据!");
            }
        },
        error: function () {
            alert("加载数据失败,请重试！");
        },
    });
}

function detailedData(name) {
    $.ajax({
        url: "/Detailed/getDetailedData",
        data: { "name": name },
        type: "post",
        success: function (result) {
            var dataJson = eval('(' + result + ')');
            if (dataJson.ok) {
                var data = dataJson.Table;
                var tal = GeneratingTable(data);
                $("#tableid").empty();
                $("#tableid").append(tal);
                $("#myModalLabel").html("各地市4G超忙小区-15分钟");
                $('#myModal').modal({
                    keyboard: true
                })

            } else {
                alert("没有数据!");
            }
        },
        error: function () {
            alert("加载数据失败,请重试！");
        },
    });
}
function GeneratingTable(data) {
    var content = "";
    var table = data;
    content = "<tr><td>开始时间</td><td>城市名称</td><td>ECGI</td><td>小区名称</td><td>覆盖热点类型</td><td>热点名称</td><td>小区频段</td><td>流量</td></tr>";
    for (row = 0; row < table.length; row++) {
        var pd = parseInt(parseInt(table[row].ECGI) % 256 / 16);
        var pds = "";
        if (pd == 1 || pd == 9) {
            pds = "800M";
        } else if (pd == 3 || pd == 0xb) {
            pds = "1.8G";
        } else if (pd == 0 || pd == 8) {
            pds = "2.1G";
        } else if (pd == 6) {
            pds = "2.6G";
        } else if (pd == 5 || pd==0xd) {
            pds = "NB-IoT";
        }
        content += ("<tr><td>" + table[row].START_TIME + "</td><td>" + table[row].CITYNAME + "</td><td>"
            + table[row].ECGI + "</td><td>" + table[row].SC_NAME + "</td><td>" + table[row].HOTSPOTCLASS + "</td><td>" + table[row].HOTSPOTNAME + "</td><td>"+pds+"</td><td>" + table[row].BFLOW_G + "</td></tr>");
    }
    return content;
}

function AlarmTable(data) {
    var content = "";
    var table = data;
    content = "<tr><td>城市</td><td>所属区域</td><td>小区名称</td><td>ECI</td><td>覆盖热点类型</td><td>热点名称</td></tr>";
    for (row = 0; row < table.length; row++) {

        content += ("<tr><td>" + table[row].CITY + "</td><td>" + table[row].BELONG_AREA + "</td><td>"
            + table[row].CELL_NAME + "</td><td>" + table[row].ECI + "</td><td>" + table[row].WIFI_TYPE + "</td><td>" + table[row].WIFI_NAME + "</td></tr>");
    }
    return content;
}