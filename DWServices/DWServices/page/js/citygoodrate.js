/**
 * 初始化图标
 */
var e1, e2, e3, e4, o1, o2, o3, o4;
var selectCity = "全省";
var selectLegend = "网页";
var selectCity2 = "全省";
var selectLegend2 = "网页";
var colorLegend = { "网页": "#82a6f5", "视频": "#495A80", "游戏": "#376956", "即时通讯": "#1DB0B8" };
var legendType = { "网页": "WEB", "视频": "VIDEO", "游戏": "GAME", "即时通讯": "IM" };
function init() {
    e1 = echarts.init(document.getElementById("e1"));
    e2 = echarts.init(document.getElementById("e2"));
    e3 = echarts.init(document.getElementById("e3"));
    e4 = echarts.init(document.getElementById("e4"));

    o1 = {
        title: {
            text:"地市小时优良率",
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        color: ["#82a6f5", "#495A80", "#376956", "#1DB0B8"],
        legend: {
            data: ["网页", "视频", "游戏", "即时通讯"],
            selectedMode: "single",
        },
        xAxis: {
            data: [],
        },
        yAxis: {
            type: 'value',
        },
        series: [
            {
                name:"网页",
                type: "bar",
                data:[],
            },
            {
                name: "视频",
                type: "bar",           
                data: [],
            },
            {
                name: "游戏",
                type: "bar",
                data: [],
            },
            {
                name: "即时通讯",
                type: "bar",
                data: [],
            }
        ]
    }
    o2 = {
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        xAxis: {
            data: [],
        },
        yAxis: {
            type: 'value',
        },
        series: [
            
        ]
    }
    o3 = o1;
    o4 = o2;

    e1.setOption(o1);
    e2.setOption(o2);
    e3.setOption(o3);
    e4.setOption(o4);
}

function getServerDateTime() {
    var serverDateTime = [];
    $.ajax({
        url: "../services/Citygoodrate.ashx?type=datetime",
        type: "get",
        dataType: "json",
        async: false,
        success: function (result) {
            if (result) {
                $("#date").val(result.year);
                $("#time").val(result.time);
                $("#date2").val(result.year);
                $("#date3").val(result.year);
                serverDateTime.push(result.year.replace(/-/g, '/'));
                serverDateTime.push(result.time);
                //serverDateTime.push("2018/05/08");
                //serverDateTime.push("17:00");
            }
        },
        error: function () {
            console.log("服务器时间获取失败");
        }
    });
    return serverDateTime;
}

/**
 * 加载首次数据
 */
function init_data() {
    //获取服务端时间
    var serverDateTime = getServerDateTime();
    //请求数据
    query1(serverDateTime[0], serverDateTime[1]);
    //query2('2018/05/01', '2018/05/08');
    query2(serverDateTime[0], serverDateTime[0]);
}

/**
 * 第一组点击
 */
function clickSearch1() {
    $("#search").click(function () {
        var date = $("#date").val();
        var time = $("#time").val();
        //待留此处添加判断
        query1(date,time);
    });
}
/**
 * 第一组查询
 * @param {any} date
 * @param {any} time
 */
var city_cache1, timer_cache1;
function query1(date,time) {
    $.ajax({
        url: "../services/Citygoodrate.ashx?type=one",
        type: "post",
        data: { "date": date, "time": time },
        dataType: "json",
        success: function (result) {
            if (result.ok) {
                city_cache1 = result.city;
                timer_cache1 = result.timer;
                var newCityData = extractionCityData(city_cache1);
                loadCityData(e1, o1, newCityData,"地市小时优良率");
                var newTimerData = extractionDayData(timer_cache1, selectCity, legendType[selectLegend]);
                loadDayData(e2, o2, newTimerData, colorLegend[selectLegend], selectCity, selectLegend);
            } else {
                console.log("缺失对应数据");
            }
        },
        error: function () {
            console.log("第一组查询失败");
        }
    });
}

function loadCityData(obj,option,data,title) {
    option = {
        title: {
            text: title,
        },
        xAxis: {
            data: data[0],
        },
        series: [
            {
                name: "网页",
                type: "bar",
                data: data[1],
                label: {
                    normal: {
                        show: true,
                        position:"top"
                    },
                    emphasis: {
                        show: true,
                        position: "top",
                    }
                }
            },
            {
                name: "视频",
                type: "bar",
                data: data[2],
                label: {
                    normal: {
                        show: true,
                        position: "top"
                    },
                    emphasis: {
                        show: true,
                        position: "top",
                    }
                }
            },
            {
                name: "游戏",
                type: "bar",
                data: data[3],
                label: {
                    normal: {
                        show: true,
                        position: "top"
                    },
                    emphasis: {
                        show: true,
                        position: "top",
                    }
                }
            },
            {
                name: "即时通讯",
                type: "bar",
                data: data[4],
                label: {
                    normal: {
                        show: true,
                        position: "top"
                    },
                    emphasis: {
                        show: true,
                        position: "top",
                    }
                }
                
            }
        ]
    }

    obj.setOption(option);
}


function loadDayData(obj, option, data,color,sc,sl) {
    option = {
        title: {
            text: sc + sl + "信息",
        },
        xAxis: {
            data: data[0],
        },
        series: [
            {
                type: "bar",
                data: data[1],
                itemStyle: {
                    normal: {
                        color: color,
                    }
                },
                label: {
                    normal: {
                        show: true,
                        position: "top",
                    },
                    emphasis: {
                        show: true,
                        position: "top",
                    }
                }
            }
        ],
    }
    obj.setOption(option);
}

/**
 * 第二组点击
 */
function clickSearch2() {
    $("#search2").click(function() {
        var date = $("#date2").val().replace(/-/g,"/");
        var date2 = $("#date3").val().replace(/-/g, "/");
        //待留此处添加判断
        query2(date,date2);
    });
}
var city_cache2, timer_cache2;
function query2(date1,date2) {
    $.ajax({
        url: "../services/Citygoodrate.ashx?type=two",
        type: "post",
        data: { "dateo": date1, "datet": date2 },
        dataType: "json",
        success: function (result) {
            if (result.ok) {
                city_cache2 = result.city2;
                timer_cache2 = result.timer2;
                var newCityData = extractionCityData(city_cache2);
                loadCityData(e3, o3, newCityData,"地市天优良率");
                var newTimerData = extractionDayData(timer_cache2, selectCity2, legendType[selectLegend2]);
                loadDayData(e4, o4, newTimerData, colorLegend[selectLegend2], selectCity2, selectLegend2);
            } else {
                console.log("缺失对应数据");
            }
        },
        error: function () {
            console.log("第二组查询失败");
        }
    });
}
/**
 * 提取数据
 */
function extractionCityData(data) {
    var city = [];
    var web_rate = [];
    var video_rate = [];
    var game_rate = [];
    var im_rate = [];
    for (var i = 0; i < data.length;i++) {
        city.push(data[i]["CITY"]);
        web_rate.push({ value: data[i]["WEB"], itemStyle: { normal: { color: selectColor("#82a6f5", data[i]["WEBBAD"]) } } });
        video_rate.push({ value: data[i]["VIDEO"], itemStyle: { normal: { color: selectColor("#495A80", data[i]["VIDEOBAD"]) } } });
        game_rate.push({ value: data[i]["GAME"], itemStyle: { normal: { color: selectColor("#376956", data[i]["GAMEBAD"]) } } });
        im_rate.push({ value: data[i]["IM"], itemStyle: { normal: { color: selectColor("#1DB0B8", data[i]["IMBAD"]) } } });
    }
    //console.log(city);
    return [city, web_rate, video_rate, game_rate, im_rate];
}

function selectColor(co, flag) {
    if (flag == "1") {
        co = "#FF0000";
    }
    return co;
}

/**
 * 提取数据
 * @param {any} data
 */
function extractionDayData(data,cityName,type) {
    var type_rate = [];
    var xitem = [];
    for (var i = 0; i < data.length; i++) {
        if (data[i]["CITY"] == cityName) {
            type_rate.push(data[i][type]);
            xitem.push(data[i]["CTIME"].split(" ")[0]);
        }
    }
    //console.log(type_rate);
    return [xitem,type_rate];
}


function e1LegendSwitch() {
    e1.on("legendselectchanged", function (params) {
        var name = params.name;
        selectLegend = name;
        var newTimerData = extractionDayData(timer_cache1, selectCity, legendType[selectLegend]);
        loadDayData(e2, o2, newTimerData, colorLegend[selectLegend], selectCity, selectLegend);
    });
}

function e1CitySwitch() {
    e1.on("click", function (params) {
        var name = params.name;
        selectCity = name;
        var newTimerData = extractionDayData(timer_cache1, selectCity, legendType[selectLegend]);
        loadDayData(e2, o2, newTimerData, colorLegend[selectLegend], selectCity, selectLegend);
    });
}

function e3LegendSwitch() {
    e3.on("legendselectchanged", function (params) {
        var name = params.name;
        selectLegend2 = name;
        console.log(selectLegend2);
        var newTimerData = extractionDayData(timer_cache2, selectCity2, legendType[selectLegend2]);
        loadDayData(e4, o4, newTimerData, colorLegend[selectLegend2], selectCity2, selectLegend2);
    });
}

function e3CitySwitch() {
    e3.on("click", function (params) {
        console.log(selectLegend2);
        var name = params.name;
        selectCity2 = name;
        var newTimerData = extractionDayData(timer_cache2, selectCity2, legendType[selectLegend2]);
        loadDayData(e4, o4, newTimerData, colorLegend[selectLegend2], selectCity2, selectLegend2);
    });
}
