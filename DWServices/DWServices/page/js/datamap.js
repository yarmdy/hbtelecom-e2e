//启动
$(function () {
    showHebei();
    showCity();
    showDistrict();
    hideCityMap();
    //hideDistrictMap();
    weekMapData();
    currentMapData();
    moreMapData();
    dayMapData();
    longMapData();
})

/* ***************************** */

var hebeiMap;
var cityMap;
var districtMap;
var cityData;
var cityVal;
//展示河北省地图
function showHebei() {
    hebeiMap = echarts.init(document.getElementById("hebei"));
    var option = {
        geo: {
            map: "河北",
            label: {
                normal: {
                    show: true
                },
                emphasis: {
                    show: true,
                    textStyle: {
                        color: "white"
                    }
                }
            },
            itemStyle: {
                normal: {
                    color: "rgb(135,206,250)",
                    borderColor: "white"
                },
                emphasis: {
                    color: "rgb(253,233,196)",
                }
            }
        },
        visualMap: {
            min: 0,
            max: 300,
            calculable: true,
            inRange: {
                color: ['#50a3ba', '#eac736', '#d94e5d']
            },
            textStyle: {
                color: '#000'
            }
        },
        series: [
            {
                name: "hebei",
                type: "map",
                geoIndex: 0,
                data: []
            }
        ]
    }
    hebeiMap.setOption(option);
    defaultMapData();
}
var mapstatus = -1;
//展示城市地图
function showCity() {
    cityMap = echarts.init(document.getElementById("city"));
    hebeiMap.on("click", function (param) {
        event.stopPropagation();
        var mapName = param.name;
        city_option = {
            geo: {
                map: mapName,
                label: {
                    normal: {
                        show: true
                    }
                },
                itemStyle: {
                    normal: {
                        color: "rgb(135,206,250)"
                    },
                    emphasis: {
                        color: "rgb(253,233,196)",
                    }
                }
            },
            series: [
                {
                    name: "zcxq",
                    type: "effectScatter",
                    rippleEffect: { scale: 2.5, brushType: 'stroke' },
                    symbolSize: 5,
                    coordinateSystem: "geo",
                    data: [],
                }
            ],
            tooltip: {
                show: true,
                trigger: "item",
                formatter: function (params) {
                    return params.data.jzmc;
                }
            }
        }
        cityMap.setOption(city_option);
        $("#district").hide();
        $(".districtshadow").hide();
        $(".shadow").show();
        $("#city").show();
        loadDataCity(mapName.substr(0, mapName.length - 1));
        mapstatus = 0;
    })
}

function showDistrict() {
    districtMap = echarts.init(document.getElementById("district"));
    cityMap.on("click", function (param) {
        event.stopPropagation();
        var mapName = "";
        var cityName = param.name;
        mapName = cityMap.getOption().geo[0].map+'-'+param.name;
        district_option = {
            geo: {
                map: mapName,
                label: {
                    normal: {
                        show: true
                    }
                },
                itemStyle: {
                    normal: {
                        color: "rgb(135,206,250)"
                    },
                    emphasis: {
                        color: "rgb(253,233,196)",
                    }
                }
            },
            series: [
                {
                    name: "zcxq",
                    type: "effectScatter",
                    rippleEffect:{scale:3,brushType:'stroke'},
                    symbolSize:10,
                    coordinateSystem: "geo",
                    label: { normal: { show: true }, emphasis: {show:false} },
                    data: [],
                }
            ],
            tooltip: {
                show: true,
                trigger: "item",
                formatter: function (params) {
                    return params.data.jzmc;
                }
            }
        }
        districtMap.setOption(district_option);
        $(".shadow").hide();
        $("#city").hide();
        $(".districtshadow").show();
        $("#district").show();
        var cityname = cityMap.getOption().geo[0].map;
        loadDataCity(cityname.substr(0, cityname.length - 1), true);
        mapstatus = 1;
    })
}

//为河北地图加载数据
function loadDataHebei() {
    //hebeiMap.showLoading();
    if (!cityVal)
    {
        cityVal = [];
    }
    hebeiMap.setOption({
        series: [
            {
                name: "hebei",
                data: cityVal
            }
        ]
    });
    //hebeiMap.hideLoading();
}

//为城市地图加载数据
function loadDataCity(mapName,notcity) {
    
    if (notcity) {
        if (!cityData) {
            cityData = [];
        }
        var newCityData = [];
        for (i = 0; i < cityData.length; i++) {
            if (cityData[i].name == mapName) {
                newCityData.push(cityData[i]);
            }
        }
        districtMap.setOption({
            series: [
                {
                    name: "zcxq",
                    data: newCityData,
                    label: { normal: { formatter: function (params) { return params.data.jzmc; }, color: "rgb(255,0,0)",position:"bottom" } }
                }
            ]
        });
    } else {
        if (!cityData) {
            cityData = [];
        }
        var newCityData = [];
        for (i = 0; i < cityData.length; i++) {
            if (cityData[i].name == mapName) {
                newCityData.push(cityData[i]);
            }
        }
        cityMap.setOption({
                series: [
                    {
                        name: "zcxq",
                        data: newCityData
                    }
                ]
            });
    }
}

//点击城市地图隐藏
function hideCityMap() {
    $("body").click(function () {
        if (mapstatus == 0) {
            $(".shadow").hide();
            $("#city").hide();
            $("#district").hide();
            mapstatus = -1;
        }
        if (mapstatus == 1) {
            $(".shadow").show();
            $("#city").show();
            $("#district").hide();
            $(".districtshadow").hide();
            mapstatus = 0;
        }
    });
}
//function hideDistrictMap() {
//    $(".districtshadow").click(function () {
//        $(".shadow").show();
//        $("#city").show();
//        $("#district").hide();
//        $(".districtshadow").hide();
//    });
//}

//查询地图数据的请求
function mapDataRequest(mapId) {
    $.ajax({
        url: "../services/DecisionMap.ashx",
        type: "get",
        data: { id: mapId},
        dataType: "json",
        success: function (result) {
            if (result.ok) {
                processData(result);
            } else {
                loadDataHebei();
                console.log("the mapdate is null");
            }
            cityNum();
        },
        error: function () {
            alert("The map data failed to load.");
        }
    })
}

//默认展示地图数据
function defaultMapData() {
    var id = "week";
    mapDataRequest(id);
}

//查询准实时地图数据
function currentMapData() {
    $("#cmap").click(function () {
        cityData = [];
        cityVal = [];
        var id = "current";
        $("#mmap img").attr("src", "images/circle1.png");
        $("#dmap img").attr("src", "images/circle1.png");
        $("#wmap img").attr("src", "images/circle1.png");
        $("#lmap img").attr("src", "images/circle1.png");
        $("#cmap img").attr("src", "images/circle.png");
        mapDataRequest(id);
    });
    
}

//查询多时段地图数据
function moreMapData() {
    $("#mmap").click(function () {
        cityData = [];
        cityVal = [];
        var id = "more";
        $("#cmap img").attr("src", "images/circle1.png");
        $("#dmap img").attr("src", "images/circle1.png");
        $("#wmap img").attr("src", "images/circle1.png");
        $("#lmap img").attr("src", "images/circle1.png");
        $("#mmap img").attr("src", "images/circle.png");
        mapDataRequest(id)
    });
}

//查询天地图数据
function dayMapData() {
    $("#dmap").click(function () {
        cityData = [];
        cityVal = [];
        var id = "day";
        $("#cmap img").attr("src", "images/circle1.png");
        $("#mmap img").attr("src", "images/circle1.png");
        $("#wmap img").attr("src", "images/circle1.png");
        $("#lmap img").attr("src", "images/circle1.png");
        $("#dmap img").attr("src", "images/circle.png");
        mapDataRequest(id);
    });
}

//查询周地图数据
function weekMapData() {
    $("#wmap").click(function () {
        cityData = [];
        cityVal = [];
        var id = "week";
        $("#cmap img").attr("src", "images/circle1.png");
        $("#dmap img").attr("src", "images/circle1.png");
        $("#mmap img").attr("src", "images/circle1.png");
        $("#lmap img").attr("src", "images/circle1.png");
        $("#wmap img").attr("src", "images/circle.png");
        mapDataRequest(id);
    });
}

//查询长期地图数据
function longMapData() {
    $("#lmap").click(function () {
        cityData = [];
        cityVal = [];
        var id = "long";
        $("#cmap img").attr("src", "images/circle1.png");
        $("#dmap img").attr("src", "images/circle1.png");
        $("#wmap img").attr("src", "images/circle1.png");
        $("#mmap img").attr("src", "images/circle1.png");
        $("#lmap img").attr("src", "images/circle.png");
        mapDataRequest(id);
    });
}

//处理返回的数据
function processData(result) {
    cityData = result.data;
    var arrtmp = [];
    for (i = 0; i < cityData.length; i++) {
        if (arrtmp.indexOf(cityData[i].name) == -1) {
            arrtmp.push(cityData[i].name);
        }
    }
    var count = 0;
    cityVal = [];
    for (j = 0; j < arrtmp.length; j++) {
        for (i = 0; i < cityData.length; i++) {
            if (arrtmp[j] == cityData[i].name) {
                count++;
            }
        }
        var temp = { name: arrtmp[j] + "市", value: count };
        cityVal.push(temp);
        count = 0;
    }
    loadDataHebei();
}

function cityNum() {
    if (cityVal.length==0) {
        var bs = document.getElementById("citynum").getElementsByTagName("b");
        for (var i = 0; i < bs.length; i++) {
            {
                bs[i].innerHTML = "0";
            }
        }
    } else {
        var div = document.getElementById("citynum");
        $(div).empty();
        var child = "";
        for (var i = 0; i < cityVal.length; i++){
            var name = cityVal[i].name + "：";
            var val = cityVal[i].value;
            child += "<span>" + name + "<b>" + val + "</b></span>&nbsp;&nbsp;"
            if ((i+1)/4===0){
                child += "<br/>";
            }
        }
        $(div).append(child);
    }
}