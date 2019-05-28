/**
 * 发送请求，获取数据
 */
function getData(date) {
    if (date == "now") {
        var d = new Date();
        d.setTime(d.getTime() - 24 * 60 * 60 * 1000);
        var month = (d.getMonth() + 1 < 10) ? "0" + (d.getMonth() + 1) : d.getMonth() + 1;
        var day = (d.getDate() < 10 ? "0" + (d.getDate()) : d.getDate());
        date = d.getFullYear() + "-" + month + "-" + day;
    }
    $.ajax({
        url: "../services/Performance.ashx",
        type: "get",
        data: { "date": date },
        dataType: "json",
        success: function (result) {
            if (result) {
                var datas = processData(result);
                loadData(datas);
            } else {
                console.log("数据错误");
            }
        },
        error: function () {
            console.log("数据请求失败！");
        }
    });
}
function getData2(date) {
    if (date == "now") {
        var d = new Date();
        d.setTime(d.getTime() - 24 * 60 * 60 * 1000);
        var month = (d.getMonth() + 1 < 10) ? "0" + (d.getMonth() + 1) : d.getMonth() + 1;
        var day = (d.getDate() < 10 ? "0" + (d.getDate()) : d.getDate());
        date = d.getFullYear() + "-" + month + "-" + day;
    }
    $.ajax({
        url: "../services/Performance.ashx",
        type: "get",
        data: { "date": date,"rtp":1 },
        dataType: "json",
        success: function (result) {
            if (result) {
                var datas = processData2(result);
                loadData2(datas);
            } else {
                console.log("数据错误");
            }
        },
        error: function () {
            console.log("数据请求失败！");
        }
    });
}
/**
 * 点击查询
 */
function search() {
    $("#search").click(function () {
        var date = $("#date").val();
        var error_msg = $("#error_msg");
        if (!date) {
            error_msg.html("日期不可为空");
            setTimeout(function () {
                error_msg.empty();
            }, 5000);
            return false;
        }
        var re = /\d{4}-\d{2}-\d{2}/;
        if (!re.test(date)) {
            error_msg.html("日期格式不正确");
            setTimeout(function () {
                error_msg.empty();
            }, 5000);
            return false;
            return false;
        }
        getData(date);
        getData2(date);
    });

}

/**
 * 加载基础图形
 */
function loadBar() {
    window.cmcc = echarts.init(document.getElementById("cmcc"));
    window.ctcc = echarts.init(document.getElementById("ctcc"));
    window.cucc = echarts.init(document.getElementById("cucc"));

    var option = {
        title: {
            text: "",
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        color: ["#376956", "#495A80", "#EDE387", "#F17C67", "#1DB0B8", "#f031f5", "#3180f5", "#f58031"],
        legend: {
            data: ['web优良率', '视频优良率', '即时通讯优良率', '游戏优良率', '异常事件优良率', '综合优良率', '五高一地优良率', '非五高一地优良率'],
        },
        xAxis: {
            data: [],
        },
        yAxis: {
            type: 'value',
        },
        series: [
            {
                name: "web优良率",
                type: "bar",
                data: []
            },
            {
                name: "视频优良率",
                type: "bar",
                data: []
            },
            {
                name: "即时通讯优良率",
                type: "bar",
                data: []
            },
            {
                name: "游戏优良率",
                type: "bar",
                data: []
            },
            {
                name: "异常事件优良率",
                type: "bar",
                data: []
            },
            {
                name: "综合优良率",
                type: "bar",
                data: []
            },
			{
			    name: "五高一地优良率",
			    type: "bar",
			    data: []
			},
			{
			    name: "非五高一地优良率",
			    type: "bar",
			    data: []
			},
        ]
    }

    cmcc.setOption(option);
    ctcc.setOption(option);
    cucc.setOption(option);
}

function loadBar2() {
    window.cmcc2 = echarts.init(document.getElementById("cmcc2"));
    window.ctcc2 = echarts.init(document.getElementById("ctcc2"));
    window.cucc2 = echarts.init(document.getElementById("cucc2"));

    var option = {
        title: {
            text: "",
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        color: ["#376956"],
        legend: {
            data: ['异常事件率'],
        },
        xAxis: {
            data: [],
        },
        yAxis: {
            type: 'value',
        },
        series: [
            {
                name: "异常事件率",
                type: "bar",
                data: []
            }
        ]
    }

    cmcc2.setOption(option);
    ctcc2.setOption(option);
    cucc2.setOption(option);
}

/**
 * 提取数据
 */
function extractionData(data, name) {
    var cities = [];
    var webs = [];
    var videos = [];
    var ims = [];
    var games = [];
    var evts = [];
    var finals = [];
    var wugao = [];
    var feiwugao = [];
    for (var i = 0; i < data.length; i++) {
        cities.push(data[i]["地市"]);
        webs.push(Math.round(data[i]["web优良率"] * 10000) / 100);
        videos.push(Math.round(data[i]["视频优良率"] * 10000) / 100);
        ims.push(Math.round(data[i]["即时通讯优良率"] * 10000) / 100);
        games.push(Math.round(data[i]["游戏优良率"] * 10000) / 100);
        evts.push(Math.round(data[i]["异常事件优良率"] * 10000) / 100);
        finals.push(Math.round(data[i]["综合带异常事件优良率"] * 10000) / 100);
        wugao.push(Math.round(data[i]["五高一地带异常事件优良率"] * 10000) / 100);
        feiwugao.push(Math.round(data[i]["非五高一地带异常事件优良率"] * 10000) / 100);
    }
    arr = {
        "name": name,
        "cities": cities,
        "videos": videos,
        "webs": webs,
        "ims": ims,
        "games": games,
        "evts":evts,
        "finals": finals,
        "wugao": wugao,
        "feiwugao": feiwugao
    }
    return arr;
}
function extractionData2(data, name) {
    var cities = [];
    var evts = [];
    for (var i = 0; i < data.length; i++) {
        cities.push(data[i]["地市"]);
        evts.push(Math.round(data[i]["异常事件率"] * 10000) / 100);
    }
    arr = {
        "name": name,
        "cities": cities,
        "evts": evts
    }
    return arr;
}
/**
 * 处理数据
 */
function processData(result) {
    var cmcc_data = result.cmcc;
    var ctcc_data = result.ctcc;
    var cucc_data = result.cucc;
    var cmcc_obj = extractionData(cmcc_data, "cmcc");
    var ctcc_obj = extractionData(ctcc_data, "ctcc");
    var cucc_obj = extractionData(cucc_data, "cucc");
    return [cmcc_obj, ctcc_obj, cucc_obj];
}
function processData2(result) {
    var cmcc_data = result.cmcc;
    var ctcc_data = result.ctcc;
    var cucc_data = result.cucc;
    var cmcc_obj = extractionData2(cmcc_data, "cmcc");
    var ctcc_obj = extractionData2(ctcc_data, "ctcc");
    var cucc_obj = extractionData2(cucc_data, "cucc");
    return [cmcc_obj, ctcc_obj, cucc_obj];
}
/**
 * 加载数据
 */
function loadData(datas) {
    for (var i = 0; i < datas.length; i++) {
        window[datas[i].name].setOption({
            title: {
                text: datas[i].name,
            },
            xAxis: {
                data: datas[i].cities,
            },
            series: [
                {
                    name: "web优良率",
                    data: datas[i].webs
                },
                {
                    name: "视频优良率",
                    data: datas[i].videos
                },
                {
                    name: "即时通讯优良率",
                    data: datas[i].ims
                },
                {
                    name: "游戏优良率",
                    data: datas[i].games
                },
                {
                    name: "异常事件优良率",
                    data: datas[i].evts
                },
                {
                    name: "综合优良率",
                    data: datas[i].finals
                },
				{
				    name: "五高一地优良率",
				    data: datas[i].wugao
				},
				{
				    name: "非五高一地优良率",
				    data: datas[i].feiwugao
				},
            ]
        });
    }
}

function loadData2(datas) {
    for (var i = 0; i < datas.length; i++) {
        window[datas[i].name+"2"].setOption({
            title: {
                text: datas[i].name,
            },
            xAxis: {
                data: datas[i].cities,
            },
            series: [
                {
                    name: "异常事件率",
                    data: datas[i].evts
                }
            ]
        });
    }
}